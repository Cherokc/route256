namespace SalesService.DataAccess;

internal static class FileUtilities
{
    public static void ForceCreateFile(string filePath)
    {
        using(StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("id, prediction, stock");

            var lineCount = Random.Shared.Next(4000, 6000);

            for (int i = 1; i < lineCount; i++)
            {
                var prediction = Random.Shared.Next(10, 1000);
                var stock = Random.Shared.Next(10, 1000);

                writer.WriteLine($"{i}, {prediction}, {stock}");
            }
        }
    }
}
