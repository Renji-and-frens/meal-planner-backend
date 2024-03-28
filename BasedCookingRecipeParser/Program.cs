using System;

namespace BasedCookingRecipeParse
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running based.cooking Recipe Parser...");
            Console.WriteLine("Please paste the path to the folder that has all the based.cooking recipe files to be parsed: ");

            var recipeParentPath = Console.ReadLine();

            Console.WriteLine($"Checking folder {recipeParentPath}...");
            List<string> recipePaths = new List<string>();
            //find all recipes in parent folder and append into recipePaths





            //read each recipe
        }
    }
}