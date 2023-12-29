namespace MyNUnitWeb.Data;

public class Assembly
{
    public int Id { get; set; }

    public List<File> Files { get; set; } = new();
}
