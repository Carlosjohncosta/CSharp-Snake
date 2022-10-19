namespace Snake;

sealed class Drawer
{
    public readonly Point BufferOffset;

    public Drawer(Point bufferOffset)
    {
        this.BufferOffset = bufferOffset;
    }

    public void DrawPixel(int x, int y, ConsoleColor color, bool buffered)
    {
        Point offset = buffered ? BufferOffset : new Point(0, 0);
        Console.BackgroundColor = color;
        Console.SetCursorPosition((x + offset.X) * 2, y + offset.Y);
        Console.Write("  ");
    }

    public void DrawPixel(int x, int y, ConsoleColor color) =>
        DrawPixel(x, y, color, true);

    public static void DrawText(int x, int y, string text)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(text);
    }
}
