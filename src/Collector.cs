using System.Text.Json;
using System.Text.Json.Nodes;

class Collector {
    private const string KEYWORD_URL = "https://www.pixiv.net/ajax/search/artworks/"; // base url
    private string _search_url = KEYWORD_URL; // base + tag url

    public string SEARCH_URL {get => _search_url; set => _search_url = KEYWORD_URL + value; } // URL of including tag

    public Collector(string tag) {
        SEARCH_URL = tag;
    }

    public async Task<List<Tuple<string, string>>> Run(HttpClient client, int repeat) {
        Console.WriteLine("===Collecting Start===");

        List<Tuple<string, string>> image_ids = new List<Tuple<string, string>> (); // image list 
        List<Task<string>> requests = new List<Task<string>>(repeat); // request list

        for(int i = 0; i < repeat; i++) {
            requests.Add(client.GetStringAsync(_search_url)); // send to request image list
        }

        foreach (var req in requests) {
            string response = await req; // waiting response

            /* get image id in image list */
            var json = JsonDocument.Parse(response).RootElement;
            var data = json.GetProperty("body").GetProperty("illustManga").GetProperty("data").EnumerateArray();
            foreach (var content in data) {
                string valid = content.ToString();

                if (valid.Contains("id") && valid.Contains("url")) {
                    string id = content.GetProperty("id").GetString()!;
                    string url = content.GetProperty("url").GetString()!;
                    image_ids.Add(Tuple.Create(id, url));
                }
            }

        }

        Console.WriteLine("===Collecting Finished===");
        Console.WriteLine($"===Number of downloadable content : {image_ids.Count}===");
        return image_ids;
    }
}