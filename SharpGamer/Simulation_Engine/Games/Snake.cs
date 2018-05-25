using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace SharpGamer.Simulation_Engine.Games
{
    class Snake
    {
        private enum Cell { Empty, Food, Snake };
        private enum Direction { Up, Down, Left, Right };
        private struct Point { public int x, y;};

        private int boardSideLength;
        private List<List<Cell>> cells;
        private List<Point> snake;
        private Direction snakeDirection;

        private const int maxFrames = 10000;
        private int fps;
        private int score = 0;


        public Snake(int pixelsw, int pixelsh)
        {
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
        }

        public int runUserGame(int framesPerSecond)
        {
            fps = framesPerSecond;
            int currentFrame = 1;
            while (currentFrame <= maxFrames)
            {
                // Get user input queue
                // process last input command + change direction if needed
                // see if events in next move (eat food, hit wall, hit snake etc..)
                // move snake
            }
            return score;
        }

    }
}
