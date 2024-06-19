using static pixiv_crawler.Crawler;

namespace pixiv_crawler;

class Program
{
    static async Task Main(string[] args)
    {
        Collector collector = new("Aru");
        Crawler crawler = new();

        var data = await collector.Run(10, IllustrationsUrlQuery.Type.illust_and_ugoira, IllustrationsUrlQuery.Order.date, IllustrationsUrlQuery.Mode.safe);
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
