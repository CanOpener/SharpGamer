using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

using MathNet.Numerics;
using SharpGamer.SimulationEngine;

namespace SharpGamer.Games
{
    public enum Cell { Empty, Food, Snake, Wall };
    public enum Direction { Up, Right, Down, Left };

    public struct GridPoint {
        public int X { get; set; }
        public int Y { get; set; }

        public GridPoint(int a, int b) {
            X = a;
            Y = b;
        }
    };

    class SnakeGame : UserPlayableGame, NetworkPlayableGame
    {
        // private varibales
        private string id;
        private bool gameOver = false;
        private Direction snakeDirection;
        private int maxTurnsSinceLastGrow = 300;
        private int gridSideLength = 20;
        private int turnNumber = 1;
        private int score = 0;
        private int turnsSinceLastGrow = 0;

        private ConcurrentQueue<Direction> keyPressQueue;
        private List<List<Cell>> grid;
        private List<GridPoint> snakeBody;
        
        // public properties
        public Random Rand { get; set; }
        public string Id { get => id; }
        public bool GameOver { get => gameOver; }
        public Direction SnakeDirection { get => snakeDirection; }
        public GridPoint SnakeHead { get => snakeBody[0]; }
        public int MaxTurnsSinceLastGrow { get => maxTurnsSinceLastGrow; }
        public int GridSideLength { get => gridSideLength; }
        public int TurnNumber { get => turnNumber; }
        public int Score { get => score; }
        
        // Returning a copy of the grid here for safety.
        public List<List<Cell>> Grid {
            get {
                var g = new List<List<Cell>>(grid.Count);
                foreach (var col in grid)
                {
                    var c = new List<Cell>(col);
                    g.Add(c);
                }
                return g;
            }
        }

        /*
         * Default constructor
        */
        public SnakeGame(Random r, int sideLength = 20)
        {
            Rand = r;
            gridSideLength = sideLength;
        }

        /*
         * Instantiates the recourses needed for the game
        */
        public void Init()
        {
            keyPressQueue = new ConcurrentQueue<Direction>();
            grid = new List<List<Cell>>(gridSideLength);
            snakeBody = new List<GridPoint>(1);

            for (var i = 0; i < gridSideLength; i++)
            {
                List<Cell> row = new List<Cell>(gridSideLength);
                for (var j = 0; j < gridSideLength; j++)
                {
                    row.Add(Cell.Empty);
                }
                grid.Add(row);
            }

            var sneakStartPoint = new GridPoint(gridSideLength / 2, gridSideLength / 2);
            snakeBody.Add(sneakStartPoint);
            snakeDirection = Direction.Up;
            grid[sneakStartPoint.X][sneakStartPoint.Y] = Cell.Snake;
            GenerateFood();
        }

        /*
         * moveEnum can be cast to a Direction which is
         * to become the snakeDirection
        */
        public bool RegisterMove(int moveEnum)
        {
            Direction move = (Direction)moveEnum;

            if (move != Opposite(snakeDirection))
            {
                snakeDirection = move;
                return true;
            }
            return false;
        }

        /*
         * Takes the key press information and stores it in
         * a queue to be processed in the next move. This is 
         * used for user playable games
        */
        public void AddKeyPress(KeyEventArgs e)
        {
            if (keyPressQueue == null) return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    keyPressQueue.Enqueue(Direction.Up);
                    break;
                case Keys.Down:
                    keyPressQueue.Enqueue(Direction.Down);
                    break;
                case Keys.Left:
                    keyPressQueue.Enqueue(Direction.Left);
                    break;
                case Keys.Right:
                    keyPressQueue.Enqueue(Direction.Right);
                    break;
                default:
                    return;
            }
        }

        /*
         * Moves the snake in its direction. If this move
         * causes a game over by hitting a wall or a body part
         * it will return true
        */
        public bool FinishTurn()
        {
            Cell nextCell = NextCellInDirection(snakeBody[0], snakeDirection, grid);
            if (nextCell == Cell.Snake || nextCell == Cell.Wall || turnsSinceLastGrow + 1 > maxTurnsSinceLastGrow)
            {
                // game over

                gameOver = true;
            }
            else if (nextCell == Cell.Food)
            {
                MoveSnakeInDirection(this.snakeDirection, true);
            }
            else
            {
                MoveSnakeInDirection(this.snakeDirection, false);
            }

            turnNumber++;
            turnsSinceLastGrow++;
            return gameOver;
        }

        /*
         * Starts a user playable game. The Object
         * paramater can be cast to a SimulationEngine.
         * RunInGuiParameters object which contains references
         * to GUI objects and fps etc...
        */
        public void StartGame(Object parameters)
        {
            RunInGuiParameters ops = (RunInGuiParameters)parameters;
            long milisPerFrame = 1000 / ops.FPS;

            while (turnNumber <= maxTurnsSinceLastGrow)
            {
                long frameStartTime = DateTime.UtcNow.Millisecond;
                WriteGameStateToTextBox(ops.TextBox2);

                // Get user input queue
                if (!keyPressQueue.IsEmpty)
                {
                    Direction nextDirection;
                    if (keyPressQueue.TryDequeue(out nextDirection))
                    {
                        // Sets new snakeDirection if not invalid move.
                        RegisterMove((int)nextDirection); 
                    }
                }
                keyPressQueue = new ConcurrentQueue<Direction>();

                // Checks if events in next move (eat food, hit wall, hit snake etc..)
                FinishTurn();

                // Draw on canvas
                Render(ops);

                // Wait until next Frame
                long currentTime = DateTime.UtcNow.Millisecond;
                long frameDuration = currentTime - frameStartTime;
                if (frameDuration < milisPerFrame)
                {
                    long sleepDuration = milisPerFrame - frameDuration;
                    System.Threading.Thread.Sleep((int)sleepDuration);
                }
            }

            WriteGameStateToTextBox(ops.TextBox2);
        }

        /*
         * Renders the game to the given PicxtureBox
        */
        public void Render(RunInGuiParameters ops)
        {
            var drawingArea = ops.Screen.CreateGraphics();

            Brush backgroundBrush = new SolidBrush(Color.LightSteelBlue);
            Brush foodBrush = new SolidBrush(Color.Red);
            Brush snakeBrush = new SolidBrush(Color.Black);

            int squareWidthPixels = ops.PixelsW / gridSideLength;
            int squareHeightPixels = ops.PixelsH / gridSideLength;

            var wipeRect = new Rectangle(new System.Drawing.Point(0, 0),
                        new System.Drawing.Size(ops.PixelsW, ops.PixelsH));
            var pixels = new List<Rectangle>(gridSideLength * gridSideLength);
            var colors = new List<Brush>(gridSideLength * gridSideLength);

            for (var i = 0; i < gridSideLength; i++)
            {
                for (var j = 0; j < gridSideLength; j++)
                {
                    Brush targetBrush = backgroundBrush;

                    if (grid[i][j] == Cell.Food) { targetBrush = foodBrush; }
                    else if (grid[i][j] == Cell.Snake) { targetBrush = snakeBrush; }

                    colors.Add(targetBrush);
                    pixels.Add(new Rectangle(new System.Drawing.Point(j * squareWidthPixels, i * squareHeightPixels),
                        new System.Drawing.Size(squareWidthPixels, squareHeightPixels)));
                }
            }

            ops.Screen.BeginInvoke((MethodInvoker)delegate
            {
                for (var i = 0; i < pixels.Count(); i++)
                {
                    drawingArea.FillRectangle(colors[i], pixels[i]);
                }
            });
        }

        /*
         * Returns the GridPoint of the next cell in
         * the given direction when standing on the given
         * location
        */
        public static GridPoint NextPointInDirection(GridPoint location, Direction dir)
        {
            var currentPoint = location;
            switch (dir)
            {
                case Direction.Up:
                    currentPoint.X--;
                    break;
                case Direction.Down:
                    currentPoint.X++;
                    break;
                case Direction.Left:
                    currentPoint.Y--;
                    break;
                case Direction.Right:
                    currentPoint.Y++;
                    break;
            }
            return currentPoint;
        }

        /*
         * Returns the next Cell in the given direction
         * when standing on the given location
        */
        public static Cell NextCellInDirection(GridPoint location, Direction dir, List<List<Cell>> grid)
        {
            var currentPoint = NextPointInDirection(location, dir);
            
            // Wall
            if (currentPoint.X < 0 || currentPoint.X >= grid.Count() ||
                currentPoint.Y < 0 || currentPoint.Y >= grid.Count())
            {
                return Cell.Wall;
            }
            // The rest can be read from the board
            else
            {
                return grid[currentPoint.X][currentPoint.Y];
            }
        }

        /*
         * Returns the direction opposite to the given direction
        */
        public static Direction Opposite(Direction d)
        {
            switch (d)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    return Direction.Down;
            }
        }

        /*
         * Moves the snake in the given direction. if
         * grow is true it will also grow the snake by 1
         * block and increment the score counter.
        */
        private void MoveSnakeInDirection(Direction d, bool grow)
        {
            var nextPoint = NextPointInDirection(snakeBody[0], d);
            for (int i=0; i< snakeBody.Count; i++)
            {
                var temp = snakeBody[i];

                // Im assuming this wont be passed as reference as i am using structs
                snakeBody[i] = nextPoint; 
                grid[nextPoint.X][nextPoint.Y] = Cell.Snake;

                nextPoint = temp;
            }
            grid[nextPoint.X][nextPoint.Y] = Cell.Empty;

            if (grow)
            {
                grid[nextPoint.X][nextPoint.Y] = Cell.Snake;
                turnsSinceLastGrow = 0;
                score++;
                snakeBody.Add(nextPoint);
                GenerateFood();
            }
        }

        /*
         * Generates a new random food object
         * on the grid
        */
        private void GenerateFood()
        {
            GridPoint randPoint = new GridPoint();
            Cell cellAtPoint;
            do
            {
                randPoint.X = Rand.Next(0, gridSideLength);
                randPoint.Y = Rand.Next(0, gridSideLength);
                cellAtPoint = grid[randPoint.X][randPoint.Y];
            } while (cellAtPoint != Cell.Empty);
            grid[randPoint.X][randPoint.Y] = Cell.Food;
        }

        /*
         * Writes current game state to the given text box
        */
        private void WriteGameStateToTextBox(RichTextBox tb)
        {
            string str = $"Game Over: {gameOver}\nTurn Number: {turnNumber}\nScore: {score}";
            tb.BeginInvoke((MethodInvoker)delegate
            {
                tb.Text = str;
            });
        }
    }
}
