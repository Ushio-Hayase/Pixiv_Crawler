using static pixiv_crawler.Crawler;

namespace pixiv_crawler;

class Program
{
    static async Task Main(string[] args)
    {
        Crawler crawler = new Crawler();

        var account = new Account { login_id="1", password="1"};

        var data = await crawler.GetLoginCookie(account);

        Console.WriteLine(data.message);
             
    }
}
