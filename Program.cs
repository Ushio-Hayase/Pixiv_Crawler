using static pixiv_crawler.Crawler;

namespace pixiv_crawler;

class Program
{
    static async Task Main(string[] args)
    {
        Collector collector = new Collector("bluearchive");
        Crawler crawler = new Crawler();

        HttpClient client = new HttpClient();

        var data = await collector.Run(client, 3);
        var res = await crawler.GetImageAsync(data);

        int j = 0;

        foreach (var i in res) {
            var f = File.Create($"test/{j}.png");
            f.Write(i);
            f.Close();
            j++;
        }

        
             
    }
}
