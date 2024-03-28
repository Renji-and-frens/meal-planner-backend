using Markdig.Syntax;
using Markdig;
using BasedCookingRecipeParser;

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

            Console.WriteLine($"Checking folder {recipeParentPath}...");
            try
            {
                string[] filePaths = Directory.GetFiles(recipeParentPath);

                List<TemporaryRecipeModel> temporaryRecipeModels = new List<TemporaryRecipeModel>();

                Console.WriteLine("Files in the parent directory:");
                int count = 0;
                foreach (string filePath in filePaths)
                {
                    Console.WriteLine($"Processing file: {filePath}...");
                    temporaryRecipeModels.Add(ParseIntoRecipeModel(filePath));
                    count++;
                }
                Console.WriteLine($"Batch completed. A total of {count} files processed and parsed.");

                Console.WriteLine($"Attempting to upload to database...");
                if(AddToDatabase(temporaryRecipeModels))
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
                return;
            }
        }

        //ignore logic of this function for now, to be fixed, WIP
        static TemporaryRecipeModel ParseIntoRecipeModel(string markdownFilePath)
        {
            string markdownText = File.ReadAllText(markdownFilePath);

            var pipeline = new MarkdownPipelineBuilder().UseYamlFrontMatter().Build();
            var document = Markdown.Parse(markdownText, pipeline);

            var frontmatter = document.GetData<Dictionary<string, string>>();

            var recipeInfo = new TemporaryRecipeModel
            {
                Title = frontmatter["title"],
                Tags = frontmatter["tags"].Trim('[', ']').Split(',').ToList(),
                Date = DateTime.Parse(frontmatter["date"]),
                Author = frontmatter["author"],
                ShortDesc = frontmatter["shortdesc"],
                PrepTime = frontmatter["preptime"],
                CookTime = frontmatter["cooktime"],
                Servings = frontmatter["servings"],
                Ingredients = ExtractListItems(document, "Ingredients"),
                Directions = ExtractListItems(document, "Directions")
            };

            return recipeInfo;
        }


        //ignore logic of this function for now, to be fixed, WIP
        static List<string> ExtractListItems(Markdig.Syntax.MarkdownDocument document, string sectionHeading)
        {
            var listItems = new List<string>();

            foreach (var block in document)
            {
                if (block is Markdig.Syntax.ListBlock listBlock &&
                    listBlock.IsOrdered &&
                    listBlock.FirstChild is Markdig.Syntax.ListItemBlock listItem &&
                    listItem.Inline.FirstOrDefault() is Markdig.Syntax.ParagraphBlock paragraph &&
                    paragraph.Inline.FirstOrDefault() is Markdig.Syntax.HtmlTag htmlTag &&
                    htmlTag.Tag == "h2" &&
                    htmlTag.GetAttributes().First().Value == sectionHeading)
                {
                    foreach (var item in listBlock)
                    {
                        if (item is ListItemBlock listItemBlock)
                        {
                            foreach (var paragraphBlock in listItemBlock)
                            {
                                if (paragraphBlock is Markdig.Syntax.ParagraphBlock paragraphBlock2)
                                {
                                    listItems.Add(paragraphBlock2.Inline.ToString());
                                }
                            }
                        }
                    }
                }
            }

            return listItems;
        }

        //ignore logic of this function for now, to be fixed, WIP
        static bool AddToDatabase(List<TemporaryRecipeModel> recipeModels)
        {

            return false;
        }
    }
}