using System.Text.Json;
using CommandLine;

class Collector {
    private const string KEYWORD_URL = "https://www.pixiv.net/ajax/search/illustrations/"; // base url
    private readonly HttpClient _client;
    private readonly string tag;

    public enum Name {
        id,
        title
    }

    public Name name;
    public string SEARCH_URL {get; set; } // URL of including tag

    public Collector(string tag, Name name) {
        this.tag = tag;
        this.name = name;
        SEARCH_URL = KEYWORD_URL + tag + $"?word={tag}";
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



    public async Task<List<(string, string)>> Run(int page, IllustrationsUrlQuery.Type type,
     IllustrationsUrlQuery.Order order, IllustrationsUrlQuery.Mode mode) {
        string name;
        if(this.name == Name.id) name = "id";
        else if(this.name == Name.title) name = "title";
        else {
            Console.Error.WriteLine("unknown option of file name type");
            Environment.Exit(-1);
        }


        Console.WriteLine("===Collecting Start===");

        List<(string, string)> image_ids = []; // image list 
        List<Task<string>> requests = new(page*60); // request list

        for(int i = 1; i <= page; ++i) {
            IllustrationsUrlQuery query = new IllustrationsUrlQuery(type, order, mode);
            
            /* add query info */
            AddSearchUrlQuery(query.TypeQuery);
            AddSearchUrlQuery(query.OrderQuery);
            AddSearchUrlQuery(query.TypeQuery);
            AddSearchUrlQuery(("s_mode", "s_tag"));
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
                    image_ids.Add((id, url));
                }
            }

        }

        Console.WriteLine("===Collecting Finished===");
        Console.WriteLine($"===Number of downloadable content : {image_ids.Count}===");
        return image_ids;
    }
}