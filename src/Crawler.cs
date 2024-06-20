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
            Timeout = TimeSpan.FromMinutes(10)
        };
    }

    // set Cookie
    void SetImageId(string id) {
        client.DefaultRequestHeaders.Remove("Referer");
        client.DefaultRequestHeaders.Add("Referer", ARTWORK_URL + id);
    }

    private async Task<(string, Task<byte[]>)> DownloadImage((string, string) id_url) {
        SetImageId(id_url.Item1);
            
        var url = await GetOriginalImage(id_url.Item1);

        return (id_url.Item1, client.GetByteArrayAsync(url));
    }

    /* get original image url */
    async Task<string> GetOriginalImage(string id) {
        var response = client.GetStringAsync(BASE_IMAGE_URL.Replace("ARTWORK_ID", id));
        JsonElement json = JsonDocument.Parse(await response).RootElement;

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
    public async Task<List<(string, byte[])>> GetImageAsync(List<(string, string)> images) {
        var StartTime = DateTime.Now;
        Console.WriteLine($"===Start DownLoading===");

        List<Task<(string, Task<byte[]>)>> thread = [];
        List<(string, byte[])> contents = new(images.Count);

        foreach (var image in images) {
            thread.Add(DownloadImage(image));
        }

        int i = 0;

        while(thread.Count != 0) {
            try {
                if(thread[i].IsCompleted) {
                    contents.Add((thread[i].Result.Item1, thread[i].Result.Item2.Result));
                    thread.Remove(thread[i]);
                }
            } catch (Exception err) {
                Console.WriteLine($"Exception : {err.Message}");
                continue;
            }
            i = thread.Count >= i ? 0 : i+1;
        }

        Console.WriteLine("===Success DownLoading===");
        Console.WriteLine($"===Downloading Time : {DateTime.Now-StartTime}===");

        return contents;
    }
}