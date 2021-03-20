using TerraFetcher.Services;

namespace TerraFetcher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new AppService();
            app.Run();
        }
    }
}