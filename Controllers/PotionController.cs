using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Services;
using Microsoft.AspNetCore.Mvc;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/potions")]
    public class PotionController : Controller
    {
        private readonly PotionService _potionService;

        public PotionController(PotionService potionService)
        {
            _potionService = potionService;
        }

        [HttpGet]
        public async Task<List<Potion>> GetAllPotions()
        {
            return await _potionService.GetAllPotion();
        }

        [HttpPost]
        public async Task<Potion> AddPotion(CreatePotion potion)
        {
            return await _potionService.AddPotion(potion);
        }

        [HttpGet("/potions/{studentId}")]
        public async Task<List<Potion>> GetAllPotionByStudent(string studentId)
        {
            return await _potionService.GetAllPotionByStudent(studentId);
        }

        [HttpPost("/potions/brew")]
        public async Task<Potion> BrewPotion(BrewPotion potion)
        {
            return await _potionService.BrewPotion(potion);
        }

        [HttpPut("/potions/{potionId:long}/add")]
        public async Task<Potion> AddIngredientToPotion(long potionId, [FromBody] Ingredient ingredient)
        {
            return await _potionService.AddIngredientToPotion(potionId, ingredient);
        }

        [HttpGet("/potions/{potionId:long}/help")]
        public async Task<List<Recipe>> HelpFinishBrew(long potionId)
        {
            return await _potionService.HelpFinishBrew(potionId);
        }
    }
}
