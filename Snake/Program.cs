namespace Snake;

sealed class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
            _ = new Game();
        else if (args.Length > 2 || args.Length == 1)
            Error();
        else if (!(Int32.TryParse(args[0], out Int32 width) && Int32.TryParse(args[1], out Int32 height)))
            Error();
        else 
            _ = new Game(width, height);
    }

    private static void Error()
    {
        throw new Exception("Args should be 2 numbers (width and height of game), " +
                    "or no args should be passed.");
    }
} 