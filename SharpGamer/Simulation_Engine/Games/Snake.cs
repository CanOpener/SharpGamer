using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using System.Windows.Forms;

namespace SharpGamer.Simulation_Engine.Games
{
    class Snake
    {
        private enum Cell { Empty, Food, Snake, Wall, Neck };
        private enum Direction { Up, Down, Left, Right };
        private struct Point { public int x, y;};

        private int boardSideLength;
        private List<List<Cell>> cells;
        private List<Point> snake;
        private Direction snakeDirection;
        private ConcurrentQueue<Direction> keyPressQueue;

        private const int maxFrames = 10000;
        private int currentFrame = 1;
        private int fps;
        private int numFood = 0;
        private int score = 0;


        public Snake(int pixelsw, int pixelsh)
        {
            keyPressQueue = new ConcurrentQueue<Direction>();
            boardSideLength = (int)MathNet.Numerics.Euclid.GreatestCommonDivisor(pixelsw, pixelsh);
            cells = new List<List<Cell>>(boardSideLength);
            snake = new List<Point>(1);

            for (int i=0; i<boardSideLength; i++)
            {
                List<Cell> row = new List<Cell>(boardSideLength);
                for (int j=0; j<boardSideLength; j++)
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

        public int runUserGame(int framesPerSecond)
        {
            fps = framesPerSecond;
            long milisPerFrame = 1000 / framesPerSecond;
            while (currentFrame <= maxFrames)
            {
                long frameStartTime = DateTime.UtcNow.Millisecond;

                // Get user input queue
                Direction nextDirection = snakeDirection;
                Cell nextCell = nextCellInDirection(snakeDirection);
                while (!keyPressQueue.IsEmpty)
                {
                    if (!keyPressQueue.TryDequeue(out nextDirection))
                    {
                        MessageBox.Show("Could Not Deque KeyPressQueue in Snake.cs");
                    }
                    else
                    {
                        // If next cell is anything but the neck (including out of bounds)
                        // update snakeDirection. Otherwise ignore.
                        nextCell = nextCellInDirection(nextDirection);
                        if (nextCell != Cell.Neck)
                        {
                            snakeDirection = nextDirection;
                        }
                    }
                }

                // see if events in next move (eat food, hit wall, hit snake etc..)
                if (nextCell == Cell.Wall || nextCell == Cell.Snake)
                {
                    MessageBox.Show("Game Over\nYour score was " + score);
                    return score;
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
                    MessageBox.Show("Somethings wrong...");
                    return score;
                }

                // Draw on canvas

                // Wait until next Frame
                long currentTime = DateTime.UtcNow.Millisecond;
                long frameDuration = currentTime - frameStartTime;
                if (fps != -1 && frameDuration < milisPerFrame)
                {
                    long sleepDuration = milisPerFrame - frameDuration;
                    System.Threading.Thread.Sleep((int)sleepDuration);
                }
                currentFrame++;
            }
            return score;
        }

        public void addKeyPress(KeyEventArgs e)
        {
            switch(e.KeyCode)
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
            // Cell in snake body just before head. Must ignore this
            else if (snake.Count > 1 && currentPoint.x == snake[1].x && currentPoint.y == snake[1].y)
            {
                return Cell.Neck;
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

            if (grow)
            {
                cells[nextPoint.x][nextPoint.y] = Cell.Snake;
                score++;
                generateFood();
            }
            else
            {
                cells[nextPoint.x][nextPoint.y] = Cell.Empty;
            }
        }

        private void generateFood()
        {
            Random rand = new Random();
            Point p;
            Cell c;
            do
            {
                p.x = rand.Next(0, boardSideLength);
                p.y = rand.Next(0, boardSideLength);
                c = cells[p.x][p.y];
            } while (c != Cell.Empty);
            cells[p.x][p.y] = Cell.Food;
            numFood++;
        }

    }
}
