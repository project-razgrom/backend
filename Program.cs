using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
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


app.MapPost("login", async (LoginDto dto,IUsersRepository repo, HttpContext ctx) =>
{
    var allUsers = await repo.GetAll();
    var c = allUsers.FirstOrDefault(user => user.Email == dto.Email && user.Password == dto.Password);
    if (c is null) return Results.NotFound();
    ctx.Response.Cookies.Append("name", c.Email);
    ctx.Response.Cookies.Append("password", c.Password);
    return Results.Ok();
});

app.MapPost("registr", async (CreateUserDto dto, IUsersRepository repo, HttpContext ctx) =>
{
    var newUser = await repo.Create(dto);
    ctx.Response.Cookies.Append("name", newUser.Email);
    ctx.Response.Cookies.Append("password", newUser.Password);
    return newUser;

});

app.MapGet("my", async (HttpContext ctx, IUsersRepository repo) =>
{
    var nameFromCookies = ctx.Request.Cookies["name"];
    var passwordFromCookies = ctx.Request.Cookies["password"];
    var allUsers = await repo.GetAll();
    return allUsers.FirstOrDefault(user => user.Email == nameFromCookies && user.Password == passwordFromCookies);
});

app.MapPost("logout", async (HttpContext ctx) => 
{
    ctx.Response.Cookies.Delete("name");
    ctx.Response.Cookies.Delete("password");
    return Results.Ok();
});

app.MapGet("banners", async (IBannersRepository repo) =>
{
    var allBanners = await repo.GetAll();
    return Results.Ok(allBanners);
});

app.MapGet("banners/current", async (IBannersRepository repo) =>
{
    return Results.Ok(await repo.GetByTimeEnd(DateTime.Now));
});

app.MapGet("banners/{id}", async (IBannersRepository repo, string id) =>
{
    return Results.Ok(await repo.GetById(new Guid(id)));
});

app.MapGet("banners/{id}/items", async (ILinkersRepository linksRepo, string id, IBannersRepository bannerRepo) =>
{
    var guid = new Guid(id);
    var banner = await bannerRepo.GetById(guid);
    var links = await linksRepo.GetByBanner(banner);
    var items = links.Select(link => link.Item).ToList();
    return Results.Ok(items);
});

app.MapGet("banners/{id}/history", async (IRollsRepository rollRepo, string id, IBannersRepository bannerRepo) =>
{
    var guid = new Guid(id);
    var banner = await bannerRepo.GetById(guid);
    var history = await rollRepo.GetBannerHistory(banner);
    return Results.Ok(history);
});

app.MapGet("history", async (IRollsRepository rollRepo) =>
{
    var history = await rollRepo.GetLastRolls(10);
    return Results.Ok(history);
});

app.MapGet("history/inventory", async (HttpContext ctx, IRollsRepository rollRepo, IUsersRepository userRepo) =>
{
    var nameFromCookies = ctx.Request.Cookies["name"] ?? string.Empty;
    var passwordFromCookies = ctx.Request.Cookies["password"] ?? string.Empty;
    var user = await userRepo.GetByLoginInfo(nameFromCookies, passwordFromCookies);
    if (user is null) return Results.NotFound();
    var userHistory = await rollRepo.GetUserHistory(user);
    return Results.Ok(userHistory);
});

app.MapPost("get_gacha", async () =>
{
    return ;
});
app.Run();

