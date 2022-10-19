namespace Snake;

readonly struct Point
{
    public readonly int X;
    public readonly int Y;
    public Point(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }
    public static Point operator +(Point p1, Point p2) =>
        new(p1.X + p2.X, p1.Y + p2.Y);
}