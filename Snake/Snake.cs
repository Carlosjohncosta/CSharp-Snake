﻿namespace Snake;

// Created by Carlos Costa, October 2022.

sealed class Game
{

    #region Declarations

    private readonly struct Point
    {
        public readonly int X;
        public readonly int Y;
        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public static Point operator +(Point p1, Point p2) =>
            new Point(p1.X + p2.X, p1.Y + p2.Y);
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

    private Drawer drawer = new(new int[] { 1, 1 });

    //Holds reference to input listener thread.
    private readonly Thread inputHandler;

    //Used to ensure only one key press is registered per game frame.
    private bool keyFrame = true;

    #endregion

    public Game(int width, int height)
    {
        this.width = width;
        this.height = height;
        Console.Title = "Snake";
        Console.CursorVisible = false;
        inputHandler = GetInputHandler();
        inputHandler.Start();
        Setup();
        Driver();
    }

    public Game() : this(20, 20) { }

    private void Setup()
    {
        score = 0;
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
                var key = Console.ReadKey(true).Key;

                if (!keyFrame)
                    continue;
                if (!directionMap.ContainsKey(key))
                    continue;

                Point newDirection = directionMap[key];

                //Gaurds against opitist directions.
                if ((newDirection + direction).Equals(new Point(0, 0)))
                    continue;
                direction = newDirection;
                keyFrame = false;
            }
        });

    private void DrawScore()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        drawer.DrawText((width + drawer.BufferOffset[0] + 1) * 2, 0, $"Score: {score}");
    }

    private void DrawBorders()
    {
        for (int i = 0; i < height + 1; i++)
        {
            drawer.DrawPixel(width + 1, i, ConsoleColor.White, false);
            drawer.DrawPixel(0, i, ConsoleColor.White, false);
        }
        for (int i = 0; i < width + 2; i++)
        {
            drawer.DrawPixel(i, height + 1, ConsoleColor.White, false);
            drawer.DrawPixel(i, 0, ConsoleColor.White, false);
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
            Setup();
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
            drawer.DrawPixel(endPos.X, endPos.Y, ConsoleColor.Black);
            snake.RemoveLast();
        }
        drawer.DrawPixel(newHead.X, newHead.Y, ConsoleColor.Red);
        keyFrame = true;
    }

    private bool CheckDeath(Point newHead)
    {
        bool checkBounds() =>
            newHead.X < 0 || newHead.Y < 0 || newHead.X >= width || newHead.Y >= height;
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

        drawer.DrawPixel(foodPos.X, foodPos.Y, ConsoleColor.Green);
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
