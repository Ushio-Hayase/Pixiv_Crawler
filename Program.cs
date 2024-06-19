using System.Text;
using System.Text.Json;
using static pixiv_crawler.Crawler;

namespace pixiv_crawler;

class Program
{
    static async Task Main(string[] args)
    {
        List<string> StudentList = [];
        using (var f = new StreamReader("studentName.json")){
            foreach (var name in JsonDocument.Parse(f.ReadToEnd()).RootElement.EnumerateArray())
                StudentList.Add(name.GetString()!);
        }
        foreach (var name in StudentList) {
            Collector collector = new("ブルーアーカイブ AND "+name);
            Crawler crawler = new();

            var data = await collector.Run(25, IllustrationsUrlQuery.Type.illust, IllustrationsUrlQuery.Order.date_d, IllustrationsUrlQuery.Mode.safe);
            var res = await crawler.GetImageAsync(data);

            int j = 0;

            Directory.CreateDirectory($"test/{name}");

            foreach (var i in res) {
                var f = File.Create($"test/{name}/{j}.png");
                f.Write(i);
                f.Close();
                j++;
            }
        }

        
             
    }
}
