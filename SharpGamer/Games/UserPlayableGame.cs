using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpGamer.Games
{
    interface UserPlayableGame
    {
        void init();
        void startGame();
        void addKeyPress(KeyEventArgs e);
    }
}
