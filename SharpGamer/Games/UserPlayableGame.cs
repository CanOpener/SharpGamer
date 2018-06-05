using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpGamer.Games
{
    /*
     * This interface defines a calss that is essentially
     * a game that provides functionality for it to be played
     * by a user with keyboard and/or mouse
    */
    interface UserPlayableGame
    {
        // This function instantiates all the recourses of the game
        void Init();

        // This function starts the game with the given run time parameters
        void StartGame(Object parameters);

        // Submits a keyboard keypress to the game.
        void AddKeyPress(KeyEventArgs e);
    }
}
