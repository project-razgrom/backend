using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project_Razgrom_v_9._184;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
//    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter a valid token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer",
        
//    });

});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetValue<string>("ConnectionStrings:Postgre"));
});

builder.Services.AddScoped<IUsersRepository, UserRepository>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IBannersRepository, BannerRepository>();
builder.Services.AddScoped<ILinkersRepository, LinkerRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentRepository>();
builder.Services.AddScoped<IRollsRepository, RollRepository>();
builder.Services
    .AddAuthentication();
builder.Services.AddScoped<IItemsRepository, ItemRepository>();
builder.Services.AddTransient<GameAdminService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
            policy =>
            {
                policy.WithOrigins("http://localhost:3000");
                policy.WithMethods(new string[] { "GET", "POST", "PUT" });
                policy.WithHeaders(new string[] { "Content-Type", "Authorization" });
                policy.AllowCredentials();
            });
}
);

    
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();


app.MapPost("login", async ([FromBody] LoginDto dto,IUsersRepository repo, HttpContext ctx) =>
{
    var allUsers = await repo.GetAll();
    var c = allUsers.FirstOrDefault(user => user.Email == dto.Email && user.Password == dto.Password);
    if (c is null) return Results.NotFound();
    ctx.Response.Cookies.Append("name", c.Email);
    ctx.Response.Cookies.Append("password", c.Password);
    return Results.Ok();
}).WithTags("Auth");

app.MapPost("register", async (CreateUserDto dto, IUsersRepository repo, HttpContext ctx) =>
{
    // Пароли не хэшим не солим, это mvp-demo-prealpha
    var newUser = await repo.Create(dto);
    ctx.Response.Cookies.Append("name", newUser.Email);
    ctx.Response.Cookies.Append("password", newUser.Password);
    return newUser;

}).WithTags("Auth");

app.MapGet("my", async (HttpContext ctx, IUsersRepository repo) =>
{
    var nameFromCookies = ctx.Request.Cookies["name"];
    var passwordFromCookies = ctx.Request.Cookies["password"];
    var allUsers = await repo.GetAll();
    return allUsers.FirstOrDefault(el => el.Email == nameFromCookies && el.Password == passwordFromCookies);
}).WithTags(new string[] { "Auth", "Authorized" });

app.MapPost("logout", async (HttpContext ctx) => 
{
    ctx.Response.Cookies.Delete("name");
    ctx.Response.Cookies.Delete("password");
    return Results.Ok();
}).WithTags("Auth");

app.MapGet("banners", async (IBannersRepository repo) =>
{
    var allBanners = await repo.GetAll();
    return Results.Ok(allBanners);
}).WithTags("Public");

app.MapGet("banners/current", async (IBannersRepository repo) =>
{
    var raw = await repo.GetByTimeEnd(DateTime.Now.ToUniversalTime());
    var result = raw.OrderBy(el => el.Type).ToList();

    return Results.Ok(result);
}).WithTags("Public");

app.MapGet("banners/{id}", async (IBannersRepository repo, string id) =>
{
    return Results.Ok(await repo.GetById(new Guid(id)));
}).WithTags("Public");

app.MapGet("banners/{id}/items", async (ILinkersRepository linksRepo, string id, IBannersRepository bannerRepo, IItemsRepository itemsRepository) =>
{
    var items = await itemsRepository.GetAll(); // ef cache
    var banners = await bannerRepo.GetAll();
    var guid = new Guid(id);
    var banner = await bannerRepo.GetById(guid);
    var links = await linksRepo.GetByBanner(banner);
    var fromStandard = await itemsRepository.GetAllFromStandard();

    var fromBanner = links.Select(link => link.Item).ToList();

    return Results.Ok(
        fromBanner
            .UnionBy(fromStandard, (el) => el?.Id)
            .Where(el => el is not null)
            .ToList()
   );
}).WithTags("Public");

app.MapGet("banners/{id}/history", async (IRollsRepository rollRepo, string id, IBannersRepository bannerRepo, IItemsRepository itemsRepository) =>
{
    var banners = await bannerRepo.GetAll();
    var items = await itemsRepository.GetAll();
    var rolls = await rollRepo.GetAll();
    var guid = new Guid(id);
    var banner = await bannerRepo.GetById(guid);
    var history = await rollRepo.GetBannerHistory(banner);
    return Results.Ok(history);
}).WithTags("Public");

app.MapGet("history", async (IRollsRepository rollRepo, IBannersRepository bannersRepository, IItemsRepository itemsRepository) =>
{
    var banners = await bannersRepository.GetAll();
    var items = await itemsRepository.GetAll();
    var history = await rollRepo.GetLastRolls(10);
    return Results.Ok(history);
}).WithTags("Public");

app.MapGet("history/inventory", async (HttpContext ctx, IRollsRepository rollRepo, IUsersRepository userRepo, IBannersRepository bannersRepository, IItemsRepository itemsRepository) =>
{
    var banners = await bannersRepository.GetAll();
    var items = await itemsRepository.GetAll();
    var nameFromCookies = ctx.Request.Cookies["name"] ?? string.Empty;
    var passwordFromCookies = ctx.Request.Cookies["password"] ?? string.Empty;
    var user = await userRepo.GetByLoginInfo(nameFromCookies, passwordFromCookies);
    if (user is null) return Results.NotFound();
    var userHistory = await rollRepo.GetUserHistory(user);
    return Results.Ok(userHistory);
}).WithTags("Authorized");

app.MapPost("banners/{bannerId}/gacha", async (
    string bannerId, 
    GameAdminService service, 
    HttpContext ctx, 
    IUsersRepository userRepo,
    IBannersRepository bannerRepo
    ) =>
{
    var name = ctx.Request.Cookies["name"] ?? string.Empty;
    var password = ctx.Request.Cookies["password"] ?? string.Empty;

    var users = await userRepo.GetAll();
    var user = users.FirstOrDefault(us => us.Email == name && us.Password == password);


    if (user is null) return Results.NotFound();

    var banner = await bannerRepo.GetById(new Guid(bannerId));

    if (banner is null) return Results.NotFound();

    return Results.Ok(await service.GetGacha(user, banner));
}).WithTags("Authorized");

if (app.Environment.IsDevelopment())
{
    app.MapPost("banners", async (CreateBannerDto dto, IBannersRepository repository) =>
    {
        return await repository.Create(dto);
    }).WithTags("Only Dev");
    app.MapPost("items", async (CreateItemDto dto, IItemsRepository repository) =>
    {
        return await repository.Create(dto);
    }).WithTags("Only Dev");
    app.MapPost("linkers", async (CreateLinkerDto dto, ILinkersRepository repository) => 
    {
        return await repository.Create(dto);
    }).WithTags("Only Dev");
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "static")),
    RequestPath = "/static"
});

app.UseCors();

app.Run();

