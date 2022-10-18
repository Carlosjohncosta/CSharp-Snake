namespace Snake;

internal class Drawer
{
    public readonly int[] BufferOffset = new int[] { 1, 1 };

    public Drawer(int[] bufferOffset)
    {
        this.BufferOffset = bufferOffset;
    }

    public void DrawPixel(int x, int y, ConsoleColor color, bool buffered)
    {
        int[] offset = buffered ? BufferOffset : new int[] { 0, 0 };
        Console.BackgroundColor = color;
        Console.SetCursorPosition((x + offset[0]) * 2, y + offset[1]);
        Console.Write("  ");
    }

    public void DrawPixel(int x, int y, ConsoleColor color) =>
        DrawPixel(x, y, color, true);

    public void DrawText(int x, int y, string text)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(text);
    }
}
