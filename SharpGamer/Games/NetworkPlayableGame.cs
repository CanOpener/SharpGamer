using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGamer.SimulationEngine;

namespace SharpGamer.Games
{
    /*
     * This interface defines a calss that is essentially
     * a game that provides functionality for it to be played
     * by a neural network
    */
    interface NetworkPlayableGame
    {
        // Unique ID.
        string Id { get; }

        // The current score of the game.
        int Score { get; }

        // The current Turn number.
        int TurnNumber { get; }

        // True if game is over.
        bool GameOver { get; }
        
        // This function instantiates all the recourses of the game
        void Init();


        // Provides the networks "move" through an enum
        bool RegisterMove(int moveEnum);

        // After the move is registered the turn can be finsihed
        // If this function returns true the game is over
        bool FinishTurn();

        // Renders the game to the screen object provided through
        // the constructor
        void Render(RunInGuiParameters ops);
    }
}
