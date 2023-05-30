namespace Project_Razgrom_v_9._184
{
    public class GameAdminService
    {
        const int MAX_GACHA_COUNT = 89;
        private IBannersRepository _bannersRepository;
        private IItemsRepository _itemsRepository;
        private ILinkersRepository _linksRepository;
        private IRollsRepository _rollsRepository;

        public GameAdminService(
            IBannersRepository bannersRepository, 
            IItemsRepository itemsRepository, 
            ILinkersRepository linksRepository, 
            IRollsRepository rollsRepository)
        {
            _bannersRepository = bannersRepository;
            _itemsRepository = itemsRepository;
            _linksRepository = linksRepository;
            _rollsRepository = rollsRepository;
        }

        public async Task<Rolls> GetGacha(Users user, Banners banner)
        {
            var last = await _rollsRepository.GetLastRollOfUser(user, banner);
            int counter = last?.Counter ?? 0;
            int nextCount = counter + 1;
            var rng = new Random();
            var percent = rng.Next(1, 100);
            //var rarity = percent > 30 
            //    ? Rarity.Rare 
            //    : percent == 1 
            //        ? Rarity.Legendary 
            //        : Rarity.Epic;
            var rarity = percent switch
            {
                1 => Rarity.Legendary,
                <= 30 => Rarity.Epic,
                _ => Rarity.Epic
            };
            if (counter >= MAX_GACHA_COUNT)
            {
                rarity = Rarity.Legendary;
            }

            if (rarity == Rarity.Legendary)
            {
                nextCount = 0;
            }

            //var isFromBanner = Convert.ToBoolean(rng.Next(0, 1)) 
            //    && rarity > Rarity.Rare; // todo: fix

            var isFromBanner = rarity switch
            {
                Rarity.Legendary or Rarity.Epic => Convert.ToBoolean(rng.Next(0, 1)),
                _ => false,
            };
            var newItem = await RandomizeItem(isFromBanner, banner, rarity);
            var roll = await _rollsRepository.Create(new CreateRollDto()
            {
                Banners = banner,
                Counter = nextCount,
                Item = newItem,
                Time = DateTime.Now,
                User = user,
                
            });

            return roll;
        }
        private async Task<Items> RandomizeItem(bool isFromBanner, Banners banner, Rarity rarity)
        {
            var rng = new Random();
            if (isFromBanner)
            {
                // заменить на GetByBanner

                //var links = await _linksRepository.GetAll();
                //var linksFromBanner = links.Where(it => 
                //    it.Banner.Id == banner.Id && it.Item.Type == rarity)
                //    .ToList();
                // /заменить на GetByBanner
                var _links = await _linksRepository.GetByBanner(banner);
                var links = _links.Where(link => link.Item.Type == rarity).ToList();


                var itemFromBanner = links[rng.Next(0, links.Count - 1)].Item 
                    ?? links[0].Item 
                    ?? throw new IndexOutOfRangeException();
                return itemFromBanner;
            }
            
            var _items = await _itemsRepository.GetAllFromStandard();
            var items = _items.Where(item => item.Type==rarity).ToList();
            var inx = rng.Next(0, items.Count - 1);
            var itemFromStandard = items[rng.Next(0, items.Count - 1)]
                ?? items[0]
                ?? throw new IndexOutOfRangeException();
            return itemFromStandard;

        }

    }
}
