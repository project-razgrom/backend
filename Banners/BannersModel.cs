﻿using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class Banners : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public BannersType Type { get; set; } 
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string ImagePath { get; set; } = string.Empty;

    }
}
public enum BannersType
{
    Characters, Weapons, Standard, 
}