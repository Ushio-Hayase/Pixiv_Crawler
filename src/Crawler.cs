using System.Net.Http.Json;
using System.Text.Json;

namespace pixiv_crawler;


class Crawler {
    private const string PIXIV_URL = "https://www.pixiv.net/"; // 픽시브 기본 URL
    private const string Login_URL = "https://accounts.pixiv.net/ajax/login?lang=ko"; // Login URL

    private HttpClient client;

    public Crawler() {
        client = new HttpClient
        {
            BaseAddress = new Uri(PIXIV_URL)
        };
    }

    // TODO 
    public async Task<Login_Response?> GetLoginCookie(Account loginInfo) { 
        var data = await client.PostAsJsonAsync(Login_URL, loginInfo);

        var json = await data.Content.ReadFromJsonAsync<Login_Response>();

        return json;
    }

    public async Task<string> GetHTMLAsync() {
        using HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

        
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine("===Success to get pixiv content===");

        return content;
    }
}