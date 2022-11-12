namespace Snake;

//Created by Carlos Costa, October 2022.

sealed class Game
{

    #region Declarations

    //Maps key presses to direction.
    private readonly Dictionary<ConsoleKey, Point> _directionMap = new()
    {
        { ConsoleKey.LeftArrow, new Point(-1, 0) },
        { ConsoleKey.RightArrow, new Point(1, 0) },
        { ConsoleKey.UpArrow, new Point(0, -1) },
        { ConsoleKey.DownArrow, new Point(0, 1) }
    };

    private readonly int _width;
    private readonly int _height;
    private readonly LinkedList<Point> _snake = new();
    private Point _direction;
    private Point _foodPos;
    private int _score;
    private readonly Drawer _drawer = new(new Point(1, 1));

    //Holds reference to input listener thread.
    private readonly Thread _inputHandler;

    //Used to ensure only one key press is registered per game frame.
    private bool _keyFrame = true;

    #endregion

    public Game(int width, int height)
    {
        _width = width;
        _height = height;
        Console.Title = "Snake";
        _inputHandler = GetInputHandler();
        _inputHandler.Start();
        Setup();
        Driver();
    }

    public Game() : this(20, 20) { }

    private void Setup()
    {
        _score = 0;
        _direction = new Point(1, 0);
        Console.Clear();
        DrawBorders();
        _snake.Clear();
        _snake.AddFirst(new Point(_width / 2, _height / 2));
        NewFood();
    }

    //Returns key press listner in new thread.
    private Thread GetInputHandler() =>
        new(() =>
        {
            static void exit()
            {
                Console.CursorVisible = true;
                Console.Clear();
                Environment.Exit(0);
            }

            while (true)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                    exit();
                if (!_keyFrame)
                    continue;
                if (!_directionMap.ContainsKey(key))
                    continue;

                Point newDirection = _directionMap[key];

                /* Guards against opposite directions.
                 * If current direction + new Direction == (0, 0), they must be oppisite.
                 */
                if (!(newDirection + _direction))
                    continue;
                _direction = newDirection;
                _keyFrame = false;
            }
        });

    private void DrawScore()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        var position = new Point((_width + _drawer.BufferOffset.X + 1) * 2, 0);
        Drawer.DrawText(position, $"Score: {_score}");
    }

    private void DrawBorders()
    {
        for (int i = 0; i < _height + 1; i++)
        {
            _drawer.DrawPixel(new Point(_width + 1, i), ConsoleColor.White, false);
            _drawer.DrawPixel(new Point(0, i), ConsoleColor.White, false);
        }
        for (int i = 0; i < _width + 2; i++)
        {
            _drawer.DrawPixel(new Point(i, _height + 1), ConsoleColor.White, false);
            _drawer.DrawPixel(new Point(i, 0), ConsoleColor.White, false);
        }
        Console.BackgroundColor = ConsoleColor.Black;
    }

    private void NextPos()
    {
        Point oldHead = _snake.First!.Value;
        Point newHead = oldHead + _direction;
        if (CheckDeath(newHead))
        {
            Console.SetCursorPosition(_width - (_width / 4), _height / 2);
            Console.Write("You Have Died!!");
            Console.ReadKey();
            Setup();
            return;
        }
        _snake.AddFirst(newHead);
        if (newHead.Equals(_foodPos))
        {
            _score++;
            NewFood();
        }
        else
        {
            Point endPos = _snake.Last!.Value;
            _drawer.DrawPixel(endPos, ConsoleColor.Black);
            _snake.RemoveLast();
        }
        _drawer.DrawPixel(newHead, ConsoleColor.Red);
        _keyFrame = true;
    }

    private bool CheckDeath(Point newHead)
    {
        bool checkBounds() =>
            newHead.X < 0 || 
            newHead.Y < 0 || 
            newHead.X >= _width || 
            newHead.Y >= _height;

        return _snake.Contains(newHead) || checkBounds();
    }

    private void NewFood()
    {
        Random rand = new();

        /* Checks if food has been placed in snake body, and choses new position if so.
         * Will cause game to lock if no pos is available, but I dont really care.
         */
        do
        {
            _foodPos = new Point(rand.Next(_width), rand.Next(_height));
        } 
        while (_snake.Contains(_foodPos));

        _drawer.DrawPixel(_foodPos, ConsoleColor.Green);
    }

    //Main thread driver.
    private void Driver()
    {
        while (true)
        {
            /* Console.CursorVisible is set to false each frame as 
             * changing window size will make cursor visible if not.
             */
            Console.CursorVisible = false;
            NextPos();
            DrawScore();
            Thread.Sleep(130);
        }
    }
}
