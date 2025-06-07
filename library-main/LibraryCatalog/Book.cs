using System;
using Newtonsoft.Json;

public class Book
{
    [JsonProperty("serial")]
    public string SerialNumber { get; set; } = "";

    [JsonProperty("title")]
    public string Title { get; set; } = "";

    [JsonProperty("author")]
    public string Author { get; set; } = "";

    [JsonProperty("languages")]
    public string Languages { get; set; } = "";

    [JsonIgnore] // Don't bind this to the old "languages" field from JSON
    public string Language { get; set; } = "";
    
    [JsonProperty("available")]
    public bool IsAvailable { get; set; }

    public override string ToString()
    {
        return $"[{SerialNumber}] {Title} by {Author} - {(IsAvailable ? "Available" : "Checked Out")}";
    }

}