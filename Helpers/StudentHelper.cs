using System;
using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Helpers
{
    public class StudentHelper
    {
        private readonly Random _random = new();

        public HouseType GetRandomHouseType(HouseType preferredHouseType)
        {
            var randomHouseType = _random.Next(0, 6);
            if (randomHouseType >= 4) return preferredHouseType;
            return (HouseType) randomHouseType;
        }

        public HouseType GetRandomHouseType()
        {
            var randomHouseType = _random.Next(0, 4);
            return (HouseType) randomHouseType;
        }
    }
}
