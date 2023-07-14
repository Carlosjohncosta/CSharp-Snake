namespace Snake;

readonly record struct Point(int X, int Y)
{
    public static Point operator +(Point p1, Point p2) =>
        new(p1.X + p2.X, p1.Y + p2.Y);
}
