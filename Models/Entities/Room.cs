using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Models.Entities
{
    public class Room
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public int Capacity { get; set; }

        public HashSet<Student> Residents { get; set; } = new HashSet<Student>();

        public HouseType RoomHouseType { get; set; }
    }
}
