namespace MyNUnitWeb.Data;

using System.Text.Json.Serialization;

public class File
{
    public int Id { get; set; }

    public byte[] ContentSha256 { get; set; }

    public string Name { get; set; }

    [JsonIgnore]
    public byte[] Content { get; set; }
}
