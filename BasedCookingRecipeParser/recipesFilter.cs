using MPBackEnd.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasedCookingRecipeParser
{
    public class RecipeFilter
    {
        public List<Recipe> RecipesWithFavTags(List<Recipe> recipes, List<Tag> chosenTags)
        {
            var recipesList = new List<Recipe>();
            foreach(var tag in chosenTags)
            {
                foreach(var recipeName in tag.UsedIn)
                {
                    var recipe = recipes.FirstOrDefault(e =>  e.Name == recipeName);
                    if (!recipesList.Contains(recipe))
                    {
                        recipesList.Add(recipe);
                    }
                }
            }
            return recipesList;
        }
        public List<Recipe> RecipesWithoutTags(List<Recipe> recipes, List<Tag> ignoreTags)
        {
            foreach(var tag in ignoreTags)
            {
                foreach(var recipeName in tag.UsedIn)
                {
                    var recipe = recipes.FirstOrDefault(e => e.Name.Contains(recipeName));
                    if(recipe != null)
                    {
                        recipes.Remove(recipe);
                    }
                }
            }
            return recipes;
        }
    }
}
