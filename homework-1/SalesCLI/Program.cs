using DataAccess;
using DataAccess.Commands;

namespace SalesCLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var console = Startup.ConfigureAndGetCLI();
            console.Run();
        }
    }
}
