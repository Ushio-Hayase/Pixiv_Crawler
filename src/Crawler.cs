using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace pixiv_crawler;


class Crawler {
    private const string PIXIV_URL = "https://www.pixiv.net/"; // 픽시브 기본 URL
    private const string ARTWORK_URL = "https://www.pixiv.net/artworks/"; // 이미지 기본 url
    private const string BASE_IMAGE_URL = "https://www.pixiv.net/ajax/illust/ARTWORK_ID/pages"; // Image ajax url

    private HttpClient client;

    public Crawler() {
        client = new HttpClient
        {
            BaseAddress = new Uri(PIXIV_URL),
        };
    }

    // set Cookie
    void SetImageId(string id) {
        client.DefaultRequestHeaders.Remove("Referer");
        client.DefaultRequestHeaders.Add("Referer", ARTWORK_URL + id);
    }

    async Task<string> GetOriginalImage(string id) {
        string response = await client.GetStringAsync(BASE_IMAGE_URL.Replace("ARTWORK_ID", id));
        JsonElement json = JsonDocument.Parse(response).RootElement;

        if(json.GetProperty("error").GetBoolean()) 
            throw new HttpRequestException("failed to get original message");

        var body = json.GetProperty("body").EnumerateArray();

        List<string> result = [];

        foreach (var data in body) {
            var url = data.GetProperty("urls").GetProperty("original").GetString();
            result.Add(url!);
        }

        return result[0];
    }

    /* HTML raw 데이터를 가져옵니다 */
    public async Task<List<byte[]>> GetImageAsync(List<Tuple<string, string>> images) {
        Console.WriteLine("===Start DownLoading===");

        List<Task<byte[]>> contents = new(images.Count);
        List<byte[]> results = new(images.Count);

        foreach (var image in images) {
            SetImageId(image.Item1);
            
            var url = await GetOriginalImage(image.Item1);

            contents.Add(client.GetByteArrayAsync(url));
            Thread.Sleep(1);
        }

        foreach (var content in contents) {
            content.Wait();
            results.Add(content.Result);
        }

        Console.WriteLine("===Success DownLoading===");

        return results;
    }
}