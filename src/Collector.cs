using System.Text.Json;
using System.Text.Json.Nodes;

class Collector {
    private const string KEYWORD_URL = "https://www.pixiv.net/ajax/search/illustrations/"; // base url
    private HttpClient _client;
    private string tag;

    public string SEARCH_URL {get; set; } // URL of including tag

    public Collector(string tag) {
        this.tag = tag;
        SEARCH_URL = KEYWORD_URL + tag;
        _client = new HttpClient {
            BaseAddress = new Uri(SEARCH_URL)
        };
    }

    private void InitSearchUrl() {
        SEARCH_URL = KEYWORD_URL + tag + $"?word={tag}";
    }

    void AddSearchUrlQuery((string, string) item) {
        SEARCH_URL = SEARCH_URL.Contains('?') ? SEARCH_URL + $"&{item.Item1}={item.Item2}" : SEARCH_URL + $"?{item.Item1}={item.Item2}";
    } 



    public async Task<List<Tuple<string, string>>> Run(int page, IllustrationsUrlQuery.Type type,
     IllustrationsUrlQuery.Order order, IllustrationsUrlQuery.Mode mode) {
        Console.WriteLine("===Collecting Start===");

        List<Tuple<string, string>> image_ids = new List<Tuple<string, string>> (); // image list 
        List<Task<string>> requests = new List<Task<string>>(page*70); // request list

        for(int i = 1; i <= page; ++i) {
            IllustrationsUrlQuery query = new IllustrationsUrlQuery(type, order, mode);
            
            /* add query info */
            AddSearchUrlQuery(query.TypeQuery);
            AddSearchUrlQuery(query.OrderQuery);
            AddSearchUrlQuery(query.TypeQuery);
            AddSearchUrlQuery(("p", i.ToString()));

            requests.Add(_client.GetStringAsync(SEARCH_URL)); // send to request image list

            InitSearchUrl(); // initialing Url
        }

        foreach (var req in requests) {
            string response = await req; // waiting response

            /* get image id in image list */
            var json = JsonDocument.Parse(response).RootElement;

            if(json.GetProperty("error").GetBoolean()) continue; // if raise error, continure

            var data = json.GetProperty("body").GetProperty("illust").GetProperty("data").EnumerateArray(); // json parse
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