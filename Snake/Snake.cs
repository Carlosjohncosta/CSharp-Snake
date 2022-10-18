namespace Snake;

// Created by Carlos Costa, October 2022.

sealed class Game
{

    #region Declarations

    private readonly struct Point
    {
        public readonly int x;
        public readonly int y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static Point operator +(Point p1, Point p2) =>
            new Point(p1.x + p2.x, p1.y + p2.y);
    }

    private readonly Dictionary<ConsoleKey, Point> directionMap = new()
    {
        { ConsoleKey.LeftArrow, new Point(-1, 0) },
        { ConsoleKey.RightArrow, new Point(1, 0) },
        { ConsoleKey.UpArrow, new Point(0, -1) },
        { ConsoleKey.DownArrow, new Point(0, 1) }
    };

    private readonly int width;
    private readonly int height;
    private readonly LinkedList<Point> snake = new();
    private Point direction = new Point(0, 1);
    private Point foodPos;
    private int score;

    //Holds reference to input listener thread.
    private readonly Thread inputHandler;

    //Offset of the game window.
    private readonly int[] bufferOffset = new int[] { 1, 1 };

    //Used to ensure only one key press is registered per game frame.
    private bool KeyFrame = true;

    #endregion

    public Game(int width, int height)
    {
        this.width = width;
        this.height = height;
        Console.Title = "Snake";
        Console.CursorVisible = false;
        inputHandler = GetInputHandler();
        inputHandler.Start();
        Reset();
        Driver();
    }

    public Game() : this(20, 20) { }

    private void Reset()
    {
        score = 0;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        DrawBorders();
        snake.Clear();
        snake.AddFirst(new Point(width / 2, height / 2));
        NewFood();
    }

    //Returns key press listner in new thread.
    private Thread GetInputHandler() =>
        new(() =>
        {
            while (true)
            {
                var t = Console.ReadKey(true).Key;

                if (!KeyFrame)
                    continue;
                if (!directionMap.ContainsKey(t))
                    continue;

                Point newDirection = directionMap[t];

                //Gaurds against opitist directions.
                if ((newDirection + direction).Equals(new Point(0, 0)))
                    continue;
                direction = newDirection;
                KeyFrame = false;
            }
        });

    private void DrawPixel(int x, int y, ConsoleColor color, bool buffered)
    {
        int[] offset = buffered ? bufferOffset : new int[] { 0, 0 };
        Console.BackgroundColor = color;
        Console.SetCursorPosition((x + offset[0]) * 2, y + offset[1]);
        Console.Write("  ");
    }

    private void DrawPixel(int x, int y, ConsoleColor color) =>
        DrawPixel(x, y, color, true);

    private void DrawScore()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.SetCursorPosition((width + bufferOffset[0] + 1) * 2, 0);
        Console.Write($"Score: {score}");
    }
    private void DrawBorders()
    {
        for (int i = 0; i < height + 1; i++)
        {
            DrawPixel(width + 1, i, ConsoleColor.White, false);
            DrawPixel(0, i, ConsoleColor.White, false);
        }
        for (int i = 0; i < width + 2; i++)
        {
            DrawPixel(i, height + 1, ConsoleColor.White, false);
            DrawPixel(i, 0, ConsoleColor.White, false);
        }
        Console.BackgroundColor = ConsoleColor.Black;
    }
    private void NextPos()
    {
        Point oldHead = snake.First!.Value;
        Point newHead = oldHead + direction;
        if (CheckDeath(newHead))
        {
            Console.SetCursorPosition(width / 2, height / 2);
            Console.Write("You Have Died!!");
            Console.ReadKey();
            Reset();
            return;
        }
        snake.AddFirst(newHead);
        if (newHead.Equals(foodPos))
        {
            score++;
            NewFood();
        }
        else
        {
            Point endPos = snake.Last!.Value;
            DrawPixel(endPos.x, endPos.y, ConsoleColor.Black);
            snake.RemoveLast();
        }
        DrawPixel(newHead.x, newHead.y, ConsoleColor.Red);
        KeyFrame = true;
    }

    private bool CheckDeath(Point newHead)
    {
        bool checkBounds() =>
            newHead.x < 0 || newHead.y < 0 || newHead.x >= width || newHead.y >= height;
        if (snake.Contains(newHead) || checkBounds())
            return true;
        return false;
    }

    private void NewFood()
    {
        Random rand = new();

        //Checks if food has been placed in snake body, and choses new position if so.
        //Will cause game to lock if no pos is available, but I dont really care.
        do
        {
            foodPos = new(rand.Next(width), rand.Next(height));
        } 
        while (snake.Contains(foodPos));

        DrawPixel(foodPos.x, foodPos.y, ConsoleColor.Green);
    }

    //Main thread driver.
    private void Driver()
    {
        while (true)
        {
            NextPos();
            DrawScore();
            Thread.Sleep(100);
        }
    }
}
