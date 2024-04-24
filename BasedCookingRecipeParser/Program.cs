using System.Text.RegularExpressions;
using BasedCookingRecipeParser;
using Microsoft.Extensions.Hosting;
using MPBackEnd.Common.Models;
using MPBackEnd.Common.Models.JsonModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BasedCookingRecipeParse
{
    public class Program
    {
        static async Task Main(string[] args)
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
                List<Tag> tagMenu = new List<Tag>();
                List<Tag> choseTag = new List<Tag>();
                List<Tag> ignoreTag = new List<Tag>();
                List<Recipe> recipeMenu = new List<Recipe>();

                Console.WriteLine("Files in the parent directory:");
                int successCount = 0;
                int totalCount = 0;

                var openAIClient = new OpenAIClient();
                var recipeFilter = new RecipeFilter();

                foreach (string filePath in filePaths)
                {
                    try
                    {
                        if (Path.GetExtension(filePath) != ".md")
                        {
                            continue;
                        }
                        totalCount++;
                        Console.WriteLine($"\n\n-------------------------\n" + 
                            $"#{successCount + 1} - Processing file: {Path.GetFileName(filePath)}...");
                        var recipe = ParseRecipeModel(filePath, openAIClient);
                        if (recipe != null)
                        {
                            recipeModels.Add(recipe);
                            foreach (var tag in recipe.Tags)
                            {
                                if (!tagMenu.Contains(tag))
                                {
                                    tagMenu.Add(tag);
                                }
                                foreach(var tagModel in tagMenu)
                                {
                                    if (string.Equals(tagModel.Name, tag.Name))
                                    {
                                        tagModel.UsedIn.Add(recipe.Name);
                                    }
                                }
                                
                            }
                            successCount++;
                        }
                        // show Tag menu to choose add tags or ignore tags
                        // push choose tags to choseTag, ignore tags to ignoreTags

                        if(choseTag.Count > 0 && ignoreTag.Count ==0)
                        {
                            recipeMenu = recipeFilter.RecipesWithFavTags(recipeModels, choseTag);
                        }
                        else if(choseTag.Count == 0 && ignoreTag.Count > 0) 
                        {
                            recipeMenu = recipeFilter.RecipesWithoutTags(recipeModels, ignoreTag);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex}");
                    }
                }
                Console.WriteLine($"\n\n\n\n-------------------------\n" + 
                    $"COMPLETED!\n" +
                    $"A total of {successCount} files processed and parsed out of {totalCount} files.");

                try
                {
                    Console.WriteLine($"Attempting to save JSON results to same recipe folder path...");
                    string json = JsonConvert.SerializeObject(recipeModels, Formatting.Indented);
                    string resultFilePath = $"{recipeParentPath}\\{DateTime.Now.ToString("yyyy-MM-dd HHmmss")}_ingredient_details.json";
                    File.WriteAllText(resultFilePath, json);
                    Console.WriteLine($"JSON result file saved successfully to {resultFilePath}.");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("JSON result file saving error.");
                    Console.WriteLine($"{ex.Message}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        static Recipe ParseRecipeModel(string markdownFilePath, OpenAIClient client)
        {
            string markdownText = File.ReadAllText(markdownFilePath);

            var splitText = markdownText.Split("---", StringSplitOptions.RemoveEmptyEntries);
            var metadataText = splitText.First().Trim();
            var content = string.Join("---", splitText.Skip(1)).Trim();

            var metadata = ExtractMetadata(metadataText);

            var recipeInfo = new Recipe();

            // Parse recipe name
            recipeInfo.Name = metadata.GetValueOrDefault("title", "NULL").Trim().Replace("\"", "");

            // Parse date
            recipeInfo.DateCreated = DateTime.Now;
            recipeInfo.DateLastModified = DateTime.Now;

            // Parse author and tags
            recipeInfo.Author = new Author() { Name = metadata.GetValueOrDefault("author", "") };
            var tagsAttached = metadata.GetValueOrDefault("tags", "").ToLower().Trim('[', ']').Split(',').ToList();
            foreach ( var tag in tagsAttached )
            {
                recipeInfo.Tags.Add(new Tag() { Name = tag });
            }

            // Parse prep time, cook time, and servings from content
            recipeInfo.PrepTime = ExtractPrepTime(content);
            recipeInfo.CookTime = ExtractCookTime(content);
            recipeInfo.Servings = ExtractServingSize(content);

            // TODO: Parse ingredients

            //AI prompting
            //var prompt = $"This is a list of ingredients from one of many recipes. Please ONLY add a label at the end of each " +
            //    $"valid ingredient line ' ||| <labelName>' and DO NOT alter ingredient details or format. This label is the" +
            //    $" category that the ingredient is in (for example, green apples and red apples will be labeled as ' - apple';" +
            //    $" basically no unnecessary adjectives). " + 
            //    $"Do not make ambiguous labels across recipes such as 'apples' vs 'Apple'. 'chicken wing' vs 'chicken thigh' and " +
            //    $"such details are fine. Don't be too generalized. The list of ingredients for this current recipe is:" +
            //    $"\n{ExtractIngredients(content)}";
            //var response = await client.GenerateResponse(prompt);
            //if (response.IsSuccessStatusCode)
            //{
            //    var jsonResponse = await response.Content.ReadAsStringAsync();
            //    var jsonObject = JObject.Parse(jsonResponse);

            //    string responseString = (string)jsonObject["choices"][0]["message"]["content"];
            //    recipeInfo.IngredientDetails = ParseIngredients(responseString);
            //}
            //else
            //{
            //    throw new Exception("OPENAI API CALL FAILED!!!");
            //}

            recipeInfo.IngredientDetails = ExtractIngredients(content);

            // Parse instructions
            recipeInfo.Instructions = ExtractInstructions(content);

            Console.WriteLine(JsonConvert.SerializeObject(recipeInfo, Formatting.Indented));

            return recipeInfo;
        }

        static int ExtractPrepTime(string content)
        {
            var result = ParseTimeToSeconds(ExtractFromContent(content, "Prep time:"));
            return result;
        }

        static int ExtractCookTime(string content)
        {
            var result = ParseTimeToSeconds(ExtractFromContent(content, "Cook time:"));
            return result;
        }

        static int ExtractServingSize(string content)
        {
            string servingsString = ExtractFromContent(content, "Servings:");
            var match = Regex.Match(servingsString, @"\d+-?");
            if (match.Success)
            {
                var result = match.Value.TrimEnd('-');
                return int.Parse(result);
            }
            return -1;
        }

        static List<string> ExtractInstructions(string content)
        {
            var instructionString = ExtractFromContent(content, "Directions", null);
            var instructions = SeparateListElements(instructionString);

            return instructions;
        }

        static List<string> ExtractIngredients(string content)
        {
            var result = ExtractFromContent(content, "Ingredients", "## ");

            return SeparateListElements(result);
        }

        public static List<IngredientDetails> ParseIngredients(string ingredientsString)
        {
            List<IngredientDetails> ingredientDetailsList = new List<IngredientDetails>();

            string[] lines = ingredientsString.Split('\n');

            foreach (string line in lines)
            {
                var trimmed = line.Trim('-').Trim();

                ingredientDetailsList.Add(new IngredientDetails
                {
                    Details = trimmed,
                });
            }

            return ingredientDetailsList;
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

            int hours = 0;
            int minutes = 0;
            Match match = Regex.Match(time, @"\d+h");
            if (match.Success)
            {
                var hourAmount = match.Groups[0].Value.TrimEnd('h');
                hours = int.Parse(hourAmount);
            }
            match = Regex.Match(time, @"\d+m");
            if (match.Success)
            {
                var minuteAmount = match.Groups[0].Value.TrimEnd('m');
                minutes = int.Parse(minuteAmount);
            }

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
            return "0";
        }

        static List<string> SeparateListElements(string input)
        {
            // Define regular expression pattern to match direction lines
            string pattern = @"^((\d+\.)|(\-))\s*(.+)$";

            // Match the pattern in the input string
            MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.Multiline);

            // Extract the matched direction lines
            List<string> directions = new List<string>();
            foreach (Match match in matches)
            {
                // Group 4 contains the text of the direction line
                string direction = match.Groups[4].Value.Trim();
                directions.Add(direction);
            }

            return directions;
        }

        //ignore logic of this function for now, to be fixed, WIP
        static bool AddToDatabase(List<Recipe> recipeModels)
        {

            return false;
        }
    }
}