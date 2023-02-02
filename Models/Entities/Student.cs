using System;
using HogwartsPotions.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace HogwartsPotions.Models.Entities
{
    public class Student : IdentityUser
    {
        public HouseType HouseType { get; set; }
        public PetType PetType { get; set; }
        public Room Room { get; set; }
        public bool IsRoomless => Room == null;

        [JsonIgnore]
        public override bool EmailConfirmed { get; set; }

        [JsonIgnore]
        public override bool TwoFactorEnabled { get; set; }

        [JsonIgnore]
        public override string PhoneNumber { get; set; }

        [JsonIgnore]
        public override bool PhoneNumberConfirmed { get; set; }

        [JsonIgnore]
        public override string PasswordHash { get; set; }

        [JsonIgnore]
        public override string SecurityStamp { get; set; }

        [JsonIgnore]
        public override bool LockoutEnabled { get; set; }

        [JsonIgnore]
        public override int AccessFailedCount { get; set; }
	}
}
