using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BasedCookingRecipeParser
{
    public static class Extensions
    {
        public static void RegisterDependencies(this HostApplicationBuilder builder)
        {
            //builder.Services.AddScoped<OpenAIClient>();
        }
    }
}
