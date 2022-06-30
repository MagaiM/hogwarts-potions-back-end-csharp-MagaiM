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
        public async Task<Potion> BrewPotion(CreatePotion potion)
        {
            return await _context.BrewPotion(potion);
        }

        [HttpGet("/potions/{studentId:long}")]
        public async Task<List<Potion>> GetAllPotionByStudent(long studentId)
        {
            return await _context.GetAllPotionByStudent(studentId);
        }
    }
}
