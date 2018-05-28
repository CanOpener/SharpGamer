using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.Games.Snake
{
    struct SnakeGameState : GameState
    {
        public bool gameOver;
        public List<List<Cell>> grid;
        public Direction snakeDirection;
        public Point snakeHead;
        public int score;
        public int currentTurn;
        public int gridSideLength;

        public SnakeGameState(ref List<List<Cell>> grid, Direction sd, Point sh, int s, int ct, bool go)
        {
            this.grid = grid;
            this.snakeDirection = sd;
            this.snakeHead = sh;
            this.score = s;
            this.currentTurn = ct;
            this.gameOver = go;
            this.gridSideLength = grid.Count();
        }
    }
}
