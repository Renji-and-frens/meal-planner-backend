using System;
using System.IO;
using System.Collections.Generic;

namespace BasedCookingRecipeParse
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running based.cooking Recipe Parser...");
            Console.WriteLine("Please paste the path to the folder that has all the based.cooking recipe files to be parsed: ");

            string recipeParentPath = Console.ReadLine();

            Console.WriteLine($"Checking folder {recipeParentPath}...");
            List<string> recipeList = new List<string>();
            //find all recipes in parent folder and append into recipePaths
            string[] files = Directory.GetFiles(recipeParentPath);
            foreach (string file in files)
            {
                recipeList.Add(Path.GetFileName(file));
            }
            Console.WriteLine("Files in the parent directory:");
            foreach (string fileName in recipeList)
            {
                Console.WriteLine(fileName);
            }




            //read each recipe
        }
    }
}