using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.Games.Snake
{
    struct SnakeGameState : GameState
    {
        bool gameOver;
        List<List<Cell>> grid;
        Direction snakeDirection;
        Point snakeHead;
        int score;
        int currentTurn;

        public SnakeGameState(ref List<List<Cell>> grid, Direction sd, Point sh, int s, int ct, bool go)
        {
            this.grid = grid;
            this.snakeDirection = sd;
            this.snakeHead = sh;
            this.score = s;
            this.currentTurn = ct;
            this.gameOver = go;
        }
    }
}
