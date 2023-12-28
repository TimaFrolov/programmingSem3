/// <summary>
/// Class that has one instance.
/// </summary>
public class Monostate
{
    /// <summary>
    /// Instance of the class.
    /// </summary>
    public static Monostate Instance { get; } = new();

    private Monostate() { }
}
