using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace HogwartsPotions.Models.Entities
{
    public class Ingredient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public HashSet<Potion> Potions { get; set; }
        [JsonIgnore]
        public HashSet<Recipe> Recipes { get; set; }

    }
}
