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
