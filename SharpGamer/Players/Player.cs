using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using SharpGamer.NeuralNetworkEngine;
using SharpGamer.Games;

namespace SharpGamer.Players
{
    abstract class Player
    {
        // Current generation number
        public abstract int GenerationNumber { get; }

        // The maximum population for a generation
        public abstract int PopulationMax { get; set; }

        // The current generation population
        public abstract List<NeuralNetwork> Population { get; }

        // The random number generator used for all random numbers
        public abstract Random Rand { get; set; }

        // Instantiates a new population;
        public abstract void Init();

        // Runs a specified number of generations. The reason it takes
        // an Object parameter is so that it can be started in another
        // thread... C#
        public abstract void RunNGenerations(Object parameters);

        // Runs the current population of networks on newly
        // instantiated game instaces and gets fitness ratings
        // and other information to be used for selection into
        // the next generation. This function also mutates the 
        // previous generation.
        public abstract void TestCurrentPopulation(Object parameters);

        // Mutates the current population based on their
        // fitness ratings, the mutation parameters etc...
        public abstract void NextGeneration(Object parameters);

        // Runs the given network on a new instance of the game
        // and renders the gameplay as it goes.
        public abstract void RunNetworkForGui(Object parameters);

        // Uses the output from the network to create a "move"
        // for the game. This "move" is submitted to the game and the
        // turn finishes.
        public abstract int NetworkOutputToMove(Matrix<float> output);

        // Uses parameters from the game object to create a set of
        // inputs for the neural network.
        public abstract Matrix<float> GameStateToNetworkInput(NetworkPlayableGame game);

        // The classic fitness function for the specific game
        // realistically this is going to be a really dynamic
        // function..
        public abstract int CalculateFitness(NetworkPlayableGame game);

        // Creates a neural network with the correct paramaters
        // for the game which the player is trying to play.
        public abstract NeuralNetwork CreateNetwork();
    }
}
