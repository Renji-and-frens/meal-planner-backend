using System.Text.RegularExpressions;
using MPBackEnd.Common.Models;

namespace BasedCookingRecipeParse
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running based.cooking Recipe Parser...");
            Console.WriteLine("Please paste the path to the folder that has all the based.cooking recipe files to be parsed: ");
            
            string? recipeParentPath = null;

            while(recipeParentPath == null)
            {
                recipeParentPath = Console.ReadLine();
            }

            Console.WriteLine($"\n-------------------------\n" +
                $"Checking folder {recipeParentPath}...");
            try
            {
                string[] filePaths = Directory.GetFiles(recipeParentPath);

                List<Recipe> recipeModels = new List<Recipe>();

                Console.WriteLine("Files in the parent directory:");
                int count = 0;
                foreach (string filePath in filePaths)
                {
                    try
                    {
                        count++;
                        Console.WriteLine($"\n\n-------------------------\n" + 
                            $"#{count} - Processing file: {Path.GetFileName(filePath)}...");
                        recipeModels.Add(ParseRecipeModel(filePath));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}");
                    }
                }
                Console.WriteLine($"Batch completed. A total of {count} files processed and parsed.");

                Console.WriteLine($"Attempting to upload to database...");
                if(AddToDatabase(recipeModels))
                {
                    Console.WriteLine($"Upload succeeded!");
                }
                else
                {
                    Console.WriteLine($"Upload failed!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        //ignore logic of this function for now, to be fixed, WIP
        static Recipe ParseRecipeModel(string markdownFilePath)
        {
            string markdownText = File.ReadAllText(markdownFilePath);

            var splitText = markdownText.Split("---", StringSplitOptions.RemoveEmptyEntries);
            var metadataText = splitText.First().Trim();
            var content = string.Join("---", splitText.Skip(1)).Trim();

            var metadata = ExtractMetadata(metadataText);

            var recipeInfo = new Recipe();

            // Parse metadata
            recipeInfo.Name = metadata.GetValueOrDefault("title", "NULL").Trim().Replace("\"", "");
            var author = metadata.GetValueOrDefault("author", "");
            var tagsAttached = metadata.GetValueOrDefault("tags", "").Trim('[', ']').Split(',').ToList();

            // Parse prep time, cook time, and servings from content
            var prepTime = ExtractFromContent(content, "Prep time:");
            var cookTime = ExtractFromContent(content, "Cook time:");
            recipeInfo.PrepTime = ParseTimeToSeconds(prepTime);
            recipeInfo.CookTime = ParseTimeToSeconds(cookTime);
            recipeInfo.Servings = int.Parse(ExtractFromContent(content, "Servings:"));

            // Parse ingredients (WIP)


            // Parse instructions
            var instructionString = ExtractFromContent(content, "Directions", null);

            return recipeInfo;
        }

        static int ParseTimeToSeconds(string time)
        {
            // Remove special characters such as :, ~ EXCEPT -
            time = Regex.Replace(time, @"[~: ()]", "");

            // Match digits followed by "h" and digits followed by "m"
            var matches = Regex.Matches(time, @"\d+h|\d+m");

            // Concatenate matched substrings
            string trimmedTimeString = "";
            foreach (Match m in matches)
            {
                trimmedTimeString += m.Value;
            }

            // Extract hours and minutes
            int hours = 0;
            int minutes = 0;
            Match match = Regex.Match(time, @"\d+h");
            if (match.Success)
            {
                hours = int.Parse(match.Groups[0].Value);
            }
            match = Regex.Match(time, @"\d+m");
            if (match.Success)
            {
                var minuteAmount = match.Groups[0].Value.TrimEnd('m');
                minutes = int.Parse(minuteAmount);
            }

            // Convert hours and minutes to total seconds
            int totalSeconds = hours * 3600 + minutes * 60;
            return totalSeconds;
        }

        static Dictionary<string, string> ExtractMetadata(string metadataText)
        {
            var metadata = new Dictionary<string, string>();

            var lines = metadataText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    metadata[key] = value;
                }
            }

            return metadata;
        }

        static string ExtractFromContent(string content, string keyword, string endCharacterOfContent = "\n")
        {
            var index = content.IndexOf(keyword);
            if (index != -1)
            {
                var startIndex = index + keyword.Length;
                if (endCharacterOfContent == null)
                {
                    return content.Substring(startIndex);
                }
                var endIndex = content.IndexOf(endCharacterOfContent, startIndex);
                if (endIndex != -1)
                {
                    return content.Substring(startIndex, endIndex - startIndex).Trim();
                }
            }
            return "";
        }

        //ignore logic of this function for now, to be fixed, WIP
        static bool AddToDatabase(List<Recipe> recipeModels)
        {

            return false;
        }
    }
}