using System.Text.Json;

class Login_Response {
    public bool error {get; set;}
    public string? message {get; set;}
    public bool reauthenticationRequired {get; set;}
    public JsonElement body {get; set;}
}

class Account {
    public required string login_id { get; set;}
    public required string password { get;set;}
}

// Search Illustations query data struct
class IllustrationsUrlQuery {
    /* Specify search target */
    public enum Type {
        illust_and_ugoira = 0,
        illust = 1,
        ugoira = 2
    }

    // Type enum to string
    private string Type2string(Type type) {
        return (int)type switch
        {
            0 => "illust_and_ugoira",
            1 => "illust",
            2 => "ugoira",
            _ => ""
        };
    }

    /* Premium account authorization is required to use popular */
    public enum Order {
        date_d = 0,
        date = 1,
        popular_d = 2,
        popular = 3,
        popular_male_d = 4,
        popular_male = 5,
        popular_female_d = 6,
        popular_female = 7
    }
    
    // Order enum to string
    private string Order2string(Order order) {
        return (int)order switch
        {
            0 => "date_d",
            1 => "date",
            2 => "popular_d",
            3 => "popular",
            4 => "popular_male_d",
            5 => "popular_male",
            6 => "popular_female_d",
            7 => "popular_female",
            _ => ""
        };
    }

    public enum Mode {
        all = 0, safe = 1, r18 = 2
    }

    // Mode enum to string
    private string Mode2string(Mode mode) {
        return (int)mode switch {
            0 => "all",
            1 => "safe",
            2 => "r18",
            _ => ""
        };
    }

    public (string, string) TypeQuery {get;  init;}
    public (string, string) OrderQuery {get; init;}
    public (string, string) ModeQuery {get; init;}

    public IllustrationsUrlQuery(Type type, Order order, Mode mode) {
        TypeQuery = ("type", Type2string(type));
        OrderQuery = ("order", Order2string(order));
        ModeQuery = ("mode", Mode2string(mode));
    }
}

