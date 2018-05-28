using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace SharpGamer.Games.Snake
{
    public enum Cell { Empty, Food, Snake, Wall };
    public enum Direction { Up, Right, Down, Left };
    public struct Point { public int x, y; public Point(int a, int b) { x = a; y = b; } };

    class Snake : UserPlayableGame, NetworkPlayableGame
    {
        private const int maxTurns = 10000;
        private const int fps = 10;
        private int boardSideLength;
        private int pixelsw;
        private int pixelsh;
        private int currentTurn = 1;
        private int numFood = 0;
        private int score = 0;
        private bool gameOver = false;

        private List<List<Cell>> cells;
        private List<Point> snake;
        private Direction snakeDirection;
        private ConcurrentQueue<Direction> keyPressQueue;
        private PictureBox screen;
        private Graphics drawingArea;
        private Random rand;

        public Snake(int pixelsw, int pixelsh, ref PictureBox screen, int gridSideLength = 20, Random r = null)
        {
            if (r == null) r = new Random();
            this.rand = r;
            this.boardSideLength = gridSideLength;
            this.pixelsw = pixelsw;
            this.pixelsh = pixelsh;
            this.screen = screen;
        }

        // intended for no graphics use.
        public Snake(int gridSideLength = 20, Random r = null)
        {
            if (r == null) r = new Random();
            this.rand = r;
            this.boardSideLength = gridSideLength;
            this.pixelsw = 500;
            this.pixelsh = 500;
            this.screen = null;
        }

        public void init()
        {
            if (screen != null)
            {
                drawingArea = screen.CreateGraphics();
            }
            
            keyPressQueue = new ConcurrentQueue<Direction>();
            cells = new List<List<Cell>>(boardSideLength);
            snake = new List<Point>(1);

            for (int i = 0; i < boardSideLength; i++)
            {
                List<Cell> row = new List<Cell>(boardSideLength);
                for (int j = 0; j < boardSideLength; j++)
                {
                    row.Add(Cell.Empty);
                }
                cells.Add(row);
            }

            Point sneakStartPoint;
            sneakStartPoint.x = boardSideLength / 2;
            sneakStartPoint.y = boardSideLength / 2;
            snake.Add(sneakStartPoint);
            snakeDirection = Direction.Up;
            cells[sneakStartPoint.x][sneakStartPoint.y] = Cell.Snake;
            generateFood();
        }

        public GameState getGameState()
        {
            return new SnakeGameState(
                    ref this.cells,
                    this.snakeDirection,
                    (this.snake.Count > 0 ? this.snake[0] : new Point(0, 0)),
                    this.score,
                    this.currentTurn,
                    this.gameOver
                );
        }

        public bool registerMove(int moveEnum)
        {
            Direction d = (Direction)moveEnum;

            if (d != opposite(snakeDirection))
            {
                snakeDirection = d;
                return true;
            }
            return false;
        }

        public void addKeyPress(KeyEventArgs e)
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
        
        public int getTurn()
        {
            return currentTurn;
        }

        public bool finishTurn()
        {
            Cell nextCell = nextCellInDirection(this.snakeDirection);
            if (nextCell == Cell.Snake || nextCell == Cell.Wall)
            {
                // game over
                gameOver = true;
            }
            else if (nextCell == Cell.Food)
            {
                moveSnakeInDirection(this.snakeDirection, true);
            }
            else
            {
                moveSnakeInDirection(this.snakeDirection, false);
            }

            currentTurn++;
            return gameOver;
        }

        public void startGame()
        {
            long milisPerFrame = 1000 / fps;
            while (currentTurn <= maxTurns)
            {
                long frameStartTime = DateTime.UtcNow.Millisecond;

                // Get user input queue
                if (!keyPressQueue.IsEmpty)
                {
                    Direction nextDirection;
                    if (keyPressQueue.TryDequeue(out nextDirection))
                    {
                        registerMove((int)nextDirection); // sets new snakeDirection if not invalid move
                    }
                    else
                    {
                        MessageBox.Show("Could Not Deque KeyPressQueue in Snake.cs");
                    }
                }
                keyPressQueue = new ConcurrentQueue<Direction>();

                // see if events in next move (eat food, hit wall, hit snake etc..)
                Cell nextCell = nextCellInDirection(this.snakeDirection);
                if (nextCell == Cell.Wall || nextCell == Cell.Snake)
                {
                    MessageBox.Show("Game Over\nYour score was " + score);
                    //return score;
                    return;
                }
                else if (nextCell == Cell.Food)
                {
                    moveSnakeInDirection(snakeDirection, true);
                }
                else if (nextCell == Cell.Empty)
                {
                    moveSnakeInDirection(snakeDirection, false);
                }
                else
                {
                    MessageBox.Show("Somethings wrong..." + nextCell);
                    return;// score;
                }

                // Draw on canvas
                render();

                // Wait until next Frame
                long currentTime = DateTime.UtcNow.Millisecond;
                long frameDuration = currentTime - frameStartTime;
                if (frameDuration < milisPerFrame)
                {
                    long sleepDuration = milisPerFrame - frameDuration;
                    System.Threading.Thread.Sleep((int)sleepDuration);
                }
                currentTurn++;
            }
            return;//score;;
        }

        public void render()
        {
            Pen blackPen = new Pen(Color.Black);
            Brush whiteBrush = new SolidBrush(Color.White);
            Brush greenBrush = new SolidBrush(Color.Green);
            Brush blueBrush = new SolidBrush(Color.Blue);
            Brush blackBrush = new SolidBrush(Color.Black);

            int squareWidthPixels = pixelsw / boardSideLength;
            int squareHeightPixels = pixelsh / boardSideLength;
            Rectangle wipeRect = new Rectangle(new System.Drawing.Point(0, 0),
                        new System.Drawing.Size(pixelsw, pixelsh));
            List<Rectangle> pixels = new List<Rectangle>(boardSideLength*boardSideLength);
            List<Brush> colors = new List<Brush>(boardSideLength * boardSideLength);

            for (int i = 0; i < boardSideLength; i++)
            {
                for (int j = 0; j < boardSideLength; j++)
                {
                    Brush targetBrush = whiteBrush;

                    if (cells[i][j] == Cell.Food) { targetBrush = greenBrush; }
                    else if (cells[i][j] == Cell.Snake) { targetBrush = blueBrush; }

                    colors.Add(targetBrush);
                    pixels.Add(new Rectangle(new System.Drawing.Point(j * squareWidthPixels, i * squareHeightPixels),
                        new System.Drawing.Size(squareWidthPixels, squareHeightPixels)));
                }
            }

            screen.BeginInvoke((MethodInvoker)delegate
            {
                for (int i = 0; i < boardSideLength * boardSideLength; i++)
                {
                    drawingArea.FillRectangle(colors[i], pixels[i]);
                }
            });
        }

        private Point nextPointInDirection(Direction d)
        {
            Point currentPoint = snake[0]; // snake always has a head
            switch (d)
            {
                case Direction.Up:
                    currentPoint.x--;
                    break;
                case Direction.Down:
                    currentPoint.x++;
                    break;
                case Direction.Left:
                    currentPoint.y--;
                    break;
                case Direction.Right:
                    currentPoint.y++;
                    break;
            }
            return currentPoint;
        }

        private Cell nextCellInDirection(Direction d)
        {
            Point currentPoint = nextPointInDirection(d);
            
            // Wall
            if (currentPoint.x < 0 || currentPoint.x >= boardSideLength ||
                currentPoint.y < 0 || currentPoint.y >= boardSideLength)
            {
                return Cell.Wall;
            }
            // The rest can be read from the board
            else
            {
                return cells[currentPoint.x][currentPoint.y];
            }
        }

        private void moveSnakeInDirection(Direction d, bool grow)
        {
            Point nextPoint = nextPointInDirection(d);
            for (int i=0; i<snake.Count; i++)
            {
                Point temp = snake[i];

                snake[i] = nextPoint; // Im assuming this wont be passed as reference as i am using structs
                cells[nextPoint.x][nextPoint.y] = Cell.Snake;

                nextPoint = temp;
            }
            cells[nextPoint.x][nextPoint.y] = Cell.Empty;

            if (grow)
            {
                cells[nextPoint.x][nextPoint.y] = Cell.Snake;
                score++;
                snake.Add(nextPoint);
                generateFood();
            }
        }

        private void generateFood()
        {
            Point randPoint;
            Cell cellAtPoint;
            do
            {
                randPoint.x = this.rand.Next(0, boardSideLength);
                randPoint.y = this.rand.Next(0, boardSideLength);
                cellAtPoint = cells[randPoint.x][randPoint.y];
            } while (cellAtPoint != Cell.Empty);
            cells[randPoint.x][randPoint.y] = Cell.Food;
            numFood++;
        }

        public Direction opposite(Direction d)
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
    }
}
