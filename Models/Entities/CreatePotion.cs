using System.Collections.Generic;

namespace HogwartsPotions.Models.Entities
{
    public class CreatePotion
    {
        public string Name { get; set; }
        public string StudentId { get; set; }
        public HashSet<Ingredient> Ingredients { get; set; }
    }
}
