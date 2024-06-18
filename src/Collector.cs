using System.Text.Json;
using System.Text.Json.Nodes;

class Collector {
    private const string KEYWORD_URL = "https://www.pixiv.net/ajax/search/artworks/"; // base url
    private string _search_url = KEYWORD_URL;

    public string SEARCH_URL {get => _search_url; set => _search_url = KEYWORD_URL + value; } // URL of including tag

    public Collector(string tag) {
        SEARCH_URL = tag;
    }

    public async Task<List<string>> Run(HttpClient client, int repeat) {
        Console.WriteLine("===Collecting Start===");

        List<string> image_ids = new List<string>();
        List<Task<string>> requests = new List<Task<string>>(repeat);

        for(int i = 0; i < repeat; i++) {
            requests.Add(client.GetStringAsync(_search_url));
        }

        foreach (var req in requests) {
            string response = await req;
            var json = JsonDocument.Parse(response).RootElement;
            var data = json.GetProperty("body").GetProperty("illustManga").GetProperty("data").EnumerateArray();
            foreach (var content in data) {
                if (content.ToString().Contains("id")) {
                    JsonElement id = content.GetProperty("id");
                    image_ids.Add(id.GetString()!);
                }
            }
        }

        Console.WriteLine("===Collecting Finished===");
        Console.WriteLine($"===Number of downloadable content : {image_ids.Count}===");
        return image_ids;
    }
}