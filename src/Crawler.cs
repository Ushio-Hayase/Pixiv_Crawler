using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace pixiv_crawler;


class Crawler {
    private const string PIXIV_URL = "https://www.pixiv.net/"; // 픽시브 기본 URL

    private HttpClient client;

    public Crawler(string PHPSESSID) {
        client = new HttpClient
        {
            BaseAddress = new Uri(PIXIV_URL),
        };

        SetLoginCookie(PHPSESSID);
    }

    // set Cookie
    void SetLoginCookie(string PHPSESSID) { 
        client.DefaultRequestHeaders.Add("Accept",
        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");

        client.DefaultRequestHeaders.Add("login_ever", "yes");
        client.DefaultRequestHeaders.Add("PHPSESSID", PHPSESSID);
    }

    /* HTML raw 데이터를 가져옵니다 */
    public async Task<string> GetHTMLAsync() {
        using HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

        
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine("===Success to get pixiv content===");


        return content;
    }
}