using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Models;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Services
{
    public class PotionService
    {
        private readonly HogwartsContext _context;

        public PotionService(HogwartsContext context)
        {
            _context = context;
        }

        public Task<Student> GetStudent(long studentId)
        {
            var student = _context.Students.Include(student => student.Room).ToListAsync().Result
                .FirstOrDefault(student => student.ID == studentId);
            return Task.FromResult(student);
        }

        public Task<List<Potion>> GetAllPotion()
        {
            var potions = _context.Potions.Include(potion => potion.Ingredients).Include(potion => potion.Recipe)
                .Include(potion => potion.Student).ToListAsync().Result;

            return Task.FromResult(potions);
        }

        public async Task<Potion> AddPotion(CreatePotion createPotion)
        {
            var student = GetStudent(createPotion.StudentId).Result;
            var recipe = new Recipe();
            var ingredients = GetIngredients(createPotion.Ingredients);
            BrewingStatus brewingStatus;

            switch (createPotion.Ingredients.Count)
            {
                case <= 0:
                    return null;
                case < HogwartsContext.MaxIngredientsForPotions:
                    brewingStatus = BrewingStatus.Brew;
                    recipe = default;
                    break;
                case > HogwartsContext.MaxIngredientsForPotions:
                    return null;
                default:
                    {
                        var allRecipes = _context.Recipes.Include(r => r.Ingredients).Include(r => r.Student).ToListAsync().Result;
                        if (allRecipes.Any(r => r.Ingredients.SetEquals(ingredients)))
                        {
                            brewingStatus = BrewingStatus.Replica;
                            recipe = allRecipes.First(r => r.Ingredients.SetEquals(ingredients));
                        }
                        else
                        {
                            brewingStatus = BrewingStatus.Discovery;
                            var recipeName = GenerateRecipeName(student);
                            recipe = new Recipe
                            {
                                Ingredients = ingredients,
                                Name = recipeName,
                                Student = student
                            };
                            _context.Recipes.Add(recipe);
                            await _context.SaveChangesAsync();
                        }

                        break;
                    }
            }

            var potion = new Potion
            {
                Ingredients = ingredients,
                Student = student,
                BrewingStatus = brewingStatus,
                Name = createPotion.Name,
                Recipe = recipe
            };

            _context.Potions.Add(potion);
            await _context.SaveChangesAsync();

            return potion;
        }

        private HashSet<Ingredient> GetIngredients(HashSet<Ingredient> createPotionIngredients)
        {
            var allIngredients = _context.Ingredients.ToListAsync().Result;
            var ingredients = new HashSet<Ingredient>();
            foreach (var potionIngredient in createPotionIngredients)
            {
                ingredients.Add(allIngredients.Any(i => i.Name == potionIngredient.Name)
                    ? allIngredients.First(i => i.Name == potionIngredient.Name)
                    : potionIngredient);
            }
            return ingredients;
        }

        private string GenerateRecipeName(Student student)
        {
            var studentRecipes = _context.Recipes.ToListAsync().Result.Where(recipe => recipe.Student.ID == student.ID).ToList();
            return $"{student.Name}'s discovery #{studentRecipes.Count + 1}";
        }

        public Task<List<Potion>> GetAllPotionByStudent(long studentId)
        {
            var potions = _context.Potions.Include(potion => potion.Ingredients).Include(potion => potion.Recipe).Include(p => p.Student).ToListAsync()
                .Result.Where(p => p.Student.ID == studentId).ToList();

            return Task.FromResult(potions);
        }

        public async Task<Potion> BrewPotion(BrewPotion brewPotion)
        {
            var student = GetStudent(brewPotion.StudentId).Result;
            var potions = GetAllPotionByStudent(student.ID).Result;
            var brewingStatus = BrewingStatus.Brew;
            var ingredients = new HashSet<Ingredient>();
            Recipe recipe = null;
            var name = $"{student.Name}'s potion #{potions.Count + 1}";

            var potion = new Potion
            {
                Name = name,
                BrewingStatus = brewingStatus,
                Ingredients = ingredients,
                Student = student,
                Recipe = recipe
            };

            _context.Potions.Add(potion);
            await _context.SaveChangesAsync();

            return potion;
        }

        public async Task<Potion> AddIngredientToPotion(long potionId, Ingredient newIngredient)
        {
            var potion = _context.Potions.Include(p => p.Ingredients).Include(p => p.Recipe).Include(p => p.Student)
                .ToListAsync().Result.FirstOrDefault(potion => potion.ID == potionId);
            if (potion == null || potion.Ingredients.Count >= HogwartsContext.MaxIngredientsForPotions) return null;

            var ingredients = potion.Ingredients;
            ingredients.Add(newIngredient);
            ingredients = GetIngredients(ingredients);
            potion.Ingredients = ingredients;

            if (ingredients.Count == HogwartsContext.MaxIngredientsForPotions)
            {

                var allRecipes = _context.Recipes.Include(r => r.Ingredients).Include(r => r.Student).ToListAsync().Result;
                BrewingStatus brewingStatus;
                Recipe recipe;

                if (allRecipes.Any(r => r.Ingredients.SetEquals(ingredients)))
                {
                    brewingStatus = BrewingStatus.Replica;
                    recipe = allRecipes.First(r => r.Ingredients.SetEquals(ingredients));
                }
                else
                {
                    brewingStatus = BrewingStatus.Discovery;
                    var recipeName = GenerateRecipeName(potion.Student);
                    recipe = new Recipe
                    {
                        Ingredients = ingredients,
                        Name = recipeName,
                        Student = potion.Student
                    };
                    _context.Recipes.Add(recipe);
                    await _context.SaveChangesAsync();
                }

                potion.BrewingStatus = brewingStatus;
                potion.Recipe = recipe;
            }
            await _context.SaveChangesAsync();

            return potion;
        }

        public Task<List<Recipe>> HelpFinishBrew(long potionId)
        {
            var ingredients = _context.Potions.Include(p => p.Ingredients).ToListAsync().Result
                .FirstOrDefault(p => p.ID == potionId)
                ?.Ingredients;
            if (ingredients == null) return Task.FromResult<List<Recipe>>(null);

            var allRecipes = _context.Recipes.Include(r => r.Ingredients).Include(r => r.Student).ToListAsync().Result;
            var result = allRecipes.FindAll(r => r.Ingredients.IsSupersetOf(ingredients));

            return Task.FromResult(result);
        }
    }
}
