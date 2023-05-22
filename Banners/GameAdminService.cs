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
            var rarity = percent > 30 
                ? Rarity.Rare 
                : percent == 1 
                    ? Rarity.Legendary 
                    : Rarity.Epic;

            if (counter >= MAX_GACHA_COUNT)
            {
                rarity = Rarity.Legendary;
            }

            if (rarity == Rarity.Legendary)
            {
                nextCount = 0;
            }

            var isFromBanner = Convert.ToBoolean(rng.Next(0, 1)) 
                && rarity > Rarity.Rare; // todo: fix



            return null;
        }
        private async Task<Items> RandomizeItem(bool isFromBanner, Banners banner, Rarity rarity)
        {
            var rng = new Random();
            if (isFromBanner)
            {
                // заменить на GetByBanner
                var links = await _linksRepository.GetAll();
                var linksFromBanner = links.Where(it => 
                    it.Banner.Id == banner.Id && it.Item.Type == rarity)
                    .ToList();
                // /заменить на GetByBanner
                var itemFromBanner = linksFromBanner[rng.Next(0, linksFromBanner.Count - 1)].Item 
                    ?? linksFromBanner[0].Item 
                    ?? throw new IndexOutOfRangeException();
                return itemFromBanner;
            }
            var items = await _itemsRepository.GetAll();
            var itemsFromStandard = items.Where(it => it.IsInStandard).ToList();
            var itemFromStandard = itemsFromStandard[rng.Next(0, itemsFromStandard.Count - 1)]
                ?? itemsFromStandard[0]
                ?? throw new IndexOutOfRangeException();
            return itemFromStandard;
        }

    }
}
