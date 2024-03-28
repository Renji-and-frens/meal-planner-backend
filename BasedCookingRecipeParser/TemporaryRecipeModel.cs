using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasedCookingRecipeParser
{
    public class TemporaryRecipeModel
    {
        public string?      Title        { get; set; }
        public string?      Tags         { get; set; }
        public string?      DateCreated  { get; set; }
        public string?      Author       { get; set; }
        public string?      Description  { get; set; }
        public List<string> Ingredients  { get; set; } = new List<string>();
        public List<string> Instructions { get; set; } = new List<string>();
    }
}
