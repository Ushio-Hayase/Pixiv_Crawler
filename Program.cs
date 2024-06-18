using static pixiv_crawler.Crawler;

namespace pixiv_crawler;

class Program
{
    static async Task Main(string[] args)
    {
        Collector collector = new Collector("bluearchive");

        HttpClient client = new HttpClient();

        var data = await collector.Run(client, 500);

        var f = File.CreateText("test/test.json");
        foreach (var i in data) {
            f.WriteLine(i);
        }

        f.Close();
             
    }
}
