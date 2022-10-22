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

    #region Operator Overloads

    public static Point operator +(Point p1, Point p2) =>
        new(p1.X + p2.X, p1.Y + p2.Y);

    public static bool operator !(Point p1) =>
        p1.X == 0 && p1.Y == 0;

    #endregion
}
