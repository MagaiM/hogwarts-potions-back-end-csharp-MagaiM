using System.Collections.Generic;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Models.View_Models
{
    public class StudentView
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public HouseType HouseType { get; set; }
        public PetType PetType { get; set; }
        public Room Room { get; set; }

        public StudentView()
        {
        }

        public StudentView(Student student)
        {
            Id = student.Id;
            UserName = student.UserName;
            HouseType = student.HouseType;
            PetType = student.PetType;
            Room = student.Room;
        }
    }
}
