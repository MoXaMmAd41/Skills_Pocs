namespace DesignPatterns.Singleton.Problem;

/// <summary>
/// The "first attempt" at a singleton that most people write. It looks right but is not thread-safe.
/// </summary>
public sealed class UnsafeSingleton
{
    private static UnsafeSingleton? _instance;

    private long _last;

    private UnsafeSingleton()
    {
    }

    public static UnsafeSingleton Instance
    {
        get
        {
            // ❌ Two threads can both see null at the same moment and BOTH create an instance.
            //    One of them is lost, along with any numbers it already issued.
            if (_instance is null)
            {
                _instance = new UnsafeSingleton();
            }

            return _instance;
        }
    }

    // ❌ Same race as before: `++` is not atomic.
    public string Next() => $"INV-{++_last:D5}";
}
