public class Monostate
{
    public static Monostate Instance { get; } = new();

    private Monostate() { }
}
