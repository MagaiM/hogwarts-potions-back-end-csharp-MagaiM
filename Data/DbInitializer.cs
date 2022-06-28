using HogwartsPotions.Models;
using System;
using System.Linq;

namespace HogwartsPotions.Data

{
    public static class DbInitializer
    {
        public static void Initialize(HogwartsContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Students.Any())
            {
                return;   // DB has been seeded
            }


        }
    }
}
