using System.Text;
using System.Net.Http.Headers;

namespace BasedCookingRecipeParser
{
    public class OpenAIClient : 
                 HttpClient
    {
        public string RequestUri { get; set; } = "https://api.openai.com/v1/chat/completions";

        public OpenAIClient()
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "insertsecretkeyhere");
        }

        public async Task<HttpResponseMessage> GenerateResponse(string prompt)
        {
            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 256,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            return await PostAsync(RequestUri, jsonContent);
        }
    }
}
