﻿using System.Collections.Generic;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Models.View_Models
{
    public class RoomlessStudentView
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public HouseType HouseType { get; set; }
        public PetType PetType { get; set; }
        public List<Room> AvailableRooms { get; set; }

        public RoomlessStudentView()
        {
        }

        public RoomlessStudentView(Student student, List<Room> availableRooms)
        {
            Id = student.Id;
            UserName = student.UserName;
            HouseType = student.HouseType;
            PetType = student.PetType;
            AvailableRooms = availableRooms;
        }
    }
}
