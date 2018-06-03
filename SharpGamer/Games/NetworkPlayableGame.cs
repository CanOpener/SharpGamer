using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.Games
{
    interface NetworkPlayableGame
    {
        string Id { get; }
        int Score { get; }
        int TurnNumber { get; }
        bool GameOver { get; }

        void Init();
        bool RegisterMove(int moveEnum);
        bool FinishTurn();
        void Render();
    }
}
