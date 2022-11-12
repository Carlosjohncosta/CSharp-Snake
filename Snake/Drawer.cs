namespace Snake;

sealed class Drawer
{
    public Point BufferOffset { get; }

    public Drawer(Point BufferOffset) =>
        this.BufferOffset = BufferOffset;

    public void DrawPixel(Point point, ConsoleColor color, bool buffered)
    {
        Point offset = buffered ? BufferOffset : new(0, 0);
        Console.BackgroundColor = color;
        Console.SetCursorPosition((point.X + offset.X) * 2, point.Y + offset.Y);
        Console.Write("  ");
    }

    public void DrawPixel(Point point, ConsoleColor color) =>
        DrawPixel(point, color, true);

    public static void DrawText(Point point, string text)
    {
        Console.SetCursorPosition(point.X, point.Y);
        Console.Write(text);
    }
}
