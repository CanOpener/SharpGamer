using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.Games
{
    interface NetworkPlayableGame
    {
        void init();
        int getTurn();
        GameState getGameState();
        bool registerMove(int moveEnum);
        bool finishTurn();
    }
}
