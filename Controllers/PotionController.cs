using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Models;
using HogwartsPotions.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/potions")]
    public class PotionController : Controller
    {
        private readonly HogwartsContext _context;

        public PotionController(HogwartsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<Potion>> GetAllPotions()
        {
            return await _context.GetAllPotion();
        }

        [HttpPost]
        public async Task<Potion> AddPotion(CreatePotion potion)
        {
            return await _context.AddPotion(potion);
        }

        [HttpGet("/potions/{studentId:long}")]
        public async Task<List<Potion>> GetAllPotionByStudent(long studentId)
        {
            return await _context.GetAllPotionByStudent(studentId);
        }

        [HttpPost("/potions/brew")]
        public async Task<Potion> BrewPotion(BrewPotion potion)
        {
            return await _context.BrewPotion(potion);
        }

        [HttpPut("/potions/{potionId:long}/add")]
        public async Task<Potion> AddIngredientToPotion(long potionId, [FromBody] Ingredient ingredient)
        {
            return await _context.AddIngredientToPotion(potionId, ingredient);
        }

        [HttpGet("/potions/{potionId:long}/help")]
        public async Task<List<Recipe>> HelpFinishBrew(long potionId)
        {
            return await _context.HelpFinishBrew(potionId);
        }
    }
}
