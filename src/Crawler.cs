using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace pixiv_crawler;


class Crawler {
    private const string PIXIV_URL = "https://www.pixiv.net/"; // 픽시브 기본 URL
    private const string ARTWORK_URL = "https://www.pixiv.net/artworks/"; // 이미지 기본 url

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

    /* HTML raw 데이터를 가져옵니다 */
    public async Task<List<byte[]>> GetImageAsync(List<Tuple<string, string>> images) {
        Console.WriteLine("===Start DownLoading===");

        List<byte[]> contents = new List<byte[]>(images.Count);

        foreach (var image in images) {
            SetImageId(image.Item1);
            contents.Add(await client.GetByteArrayAsync(image.Item2));
            Thread.Sleep(10);
        }

    

        Console.WriteLine("===Success DownLoading===");


        return contents;
    }
}