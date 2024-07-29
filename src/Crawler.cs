using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace pixiv_crawler;


class Crawler {
    private const string PIXIV_URL = "http://www.pixiv.net/"; // 픽시브 기본 URL
    private const string ARTWORK_URL = "http://www.pixiv.net/artworks/"; // 이미지 기본 url
    private const string BASE_IMAGE_URL = "http://www.pixiv.net/ajax/illust/ARTWORK_ID/pages"; // Image ajax url
    private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.0.0 Safari/537.36";

    private HttpClient client;

    public Crawler() {
        client = new HttpClient
        {
            BaseAddress = new Uri(PIXIV_URL),
            Timeout = TimeSpan.FromMinutes(2)
        };
        client.DefaultRequestHeaders.Add("user-agent", USER_AGENT);
    }

    void SetImageId(string id) {
        client.DefaultRequestHeaders.Remove("Referer");
        client.DefaultRequestHeaders.Add("Referer", ARTWORK_URL + id);
    }

    private (string, Task<byte[]>) DownloadImage((string, string) id_url) { // (name, url) id_url
        SetImageId(id_url.Item1);
            
        var url = GetOriginalImageUrl(id_url.Item1).Result;

        string extension = Path.GetExtension(url);

        return (id_url.Item1 + extension, client.GetByteArrayAsync(url));
    }

    /* get original image url */
    async Task<string> GetOriginalImageUrl(string id) {
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

    
    public async Task<List<(string, byte[])>> GetImageAsync(List<(string, string)> images) { // List<Name, Url>
        var StartTime = DateTime.Now;
        Console.WriteLine($"===Start DownLoading===");

        List<(string, Task<byte[]>)> thread = [];
        List<(string, byte[])> contents  = new();

        foreach (var image in images) {
            thread.Add(DownloadImage(image));
        }

        int i = 0;

        while(thread.Count != 0) {
            try {
                if(thread[i].Item2.IsCompletedSuccessfully) {
                    contents.Add((thread[i].Item1, await thread[i].Item2));
                    thread[i].Item2.Dispose();
                    thread.Remove(thread[i]);
                }
            } catch (Exception err) {
                Console.WriteLine($"Exception : {err.Message}");
                throw new Exception(err.ToString());
            }
            i = thread.Count - 1 > i ? i+1 : 0;
        }

        Console.WriteLine("===Success DownLoading===");
        Console.WriteLine($"===Downloading Time : {DateTime.Now-StartTime}===");

        return contents;
    }
}