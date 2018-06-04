using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGamer.NeuralNetworkEngine;
using SharpGamer.NeuralNetworkEngine.ActivationFunctions;
using SharpGamer.SimulationEngine;
using SharpGamer.Games;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace SharpGamer.Players
{
    class SnakePlayer : Player
    {
        private int generationNumber = 1;
        private List<NeuralNetwork> population;

        public override Random Rand { get; set; }
        public override int PopulationMax { get; set; }
        public override int GenerationNumber { get => generationNumber; }
        public override List<NeuralNetwork> Population {
            get {
                //List<NeuralNetwork> pop = new List<NeuralNetwork>(population.Count);
                //foreach (var n in population)
                //{
                //    pop.Add(new NeuralNetwork(n));
                //}
                //return pop;

                // The above code introduced problems with my
                // logic. Just going to use the reference pass by now
                return population;
            }
        }
        
        /*
         * Default constructor
        */
        public SnakePlayer(int populationMax, Random r)
        {
            Rand = r;
            PopulationMax = populationMax;
        }

        /*
         * Creates and Instantiates a new population based on the 
         * PopulationMax property
        */
        public override void Init()
        {
            List<NeuralNetwork> newPop = new List<NeuralNetwork>(PopulationMax);
            while(newPop.Count() < PopulationMax)
            {
                newPop.Add(CreateNetwork());
            }
            population = newPop;
        }

        /*
         * Runs a specified number of generations. The reason it takes
         * an Object parameter is so that it can be started in another
         * thread... C#
        */
        public override void RunNGenerations(Object parameters)
        {
            RunParameters ops = (RunParameters)parameters;

            for (int i = 0; i < ops.NumGenerations; i++)
            {
                TestCurrentPopulation(parameters);
            }
        }

        /* 
         * Runs the current population of networks on newly
         * instantiated game instaces and gets fitness ratings
         * and other information to be used for selection into
         * the next generation. This function also mutates the
         * previous generation.
        */
        public override void TestCurrentPopulation(Object parameters)
        {
            RunParameters ops = (RunParameters)parameters;

            // Mutate previous population into this generation.
            // This is done at the start so that you can view the
            // population statistics before deciding what parameters
            // to use to mutate it.
            NextGeneration(parameters);

            // Instantiate a Snake game for each member of the population
            List<SnakeGame> games = new List<SnakeGame>(population.Count());
            foreach (var i in population)
            {
                SnakeGame newGame = new SnakeGame(Rand, 25);
                newGame.Init();
                games.Add(newGame);
            }

            // Run all games.
            var turnNumber = 1;
            var numFinished = 0;
            while (numFinished < population.Count)
            {
                for (var i = 0; i < population.Count(); i++)
                {
                    NeuralNetwork network = population[i];
                    SnakeGame gameInstance = games[i];
                    
                    // Skip if game is already over
                    if (gameInstance.GameOver)
                    {
                        continue;
                    }

                    // Interprete game state to inputs for the neural network
                    Matrix<float> inputs = GameStateToNetworkInput(gameInstance);

                    // Run inputs through network and retrieve output
                    Matrix<float> output = network.ProcessInput(inputs);

                    // Interprete network output to game move.
                    // gameInstance.registerMove(networkOutputToMove(output));
                    // gameInstance.registerMove(networkOutputFacingToMove(output, state.snakeDirection));
                    gameInstance.RegisterMove(NetworkOutputToMove(output, gameInstance));

                    // go to next turn in game
                    bool gameOver = gameInstance.FinishTurn();
                    if (gameOver)
                    {
                        numFinished++;
                        network.Score = CalculateFitness(gameInstance);
                    }
                }

                String strToWrite = $"{population.Count - numFinished} Left.\n" +
                    $"Generation : {generationNumber}\n" +
                    $"Turn: {turnNumber}\n";

                ops.TextBox1.BeginInvoke((MethodInvoker)delegate
                {
                    ops.TextBox1.Text = strToWrite;
                });
                turnNumber++;
            }

            // sort list so [0] is fittest
            population = population.OrderBy(i => i.Score).ToList();
            population.Reverse();

            String str = $"Done.\n";
            str += $"Generation: {generationNumber}\n";
            str += $"Turn: {turnNumber}\n";
            str += $"Best NN score: {population[0].Score}\n";

            ops.TextBox2.BeginInvoke((MethodInvoker)delegate
            {
                ops.TextBox2.Text = str;
            });
            generationNumber++;
        }

        // The classic fitness function for the specific game
        // realistically this is going to be a really dynamic
        // function..
        public override int CalculateFitness(NetworkPlayableGame game)
        {
            return game.Score;
        }

        /*
         * Mutates the current population based on the mutation paramaters
         * sent down by the Simulation Engine. The paramaters object must be
         * castable to a SimulationEngine.RunParameters object
        */
        public override void NextGeneration(Object parameters)
        {
            RunParameters ops = (RunParameters)parameters;
            if (generationNumber == 1)
            {
                return;
            }

            List<DNA> currentPopAsDNAList = new List<DNA>(population.Count);
            List<DNA> newPopulationAsDNAList = new List<DNA>(PopulationMax);
            List<NeuralNetwork> newPopulation = new List<NeuralNetwork>(PopulationMax);
            foreach (var network in population)
            {
                currentPopAsDNAList.Add(network);
            }

            if (ops.UseDiversity)
            {
                newPopulationAsDNAList = GeneticLearning.GeneratePopulationFromFitnessAndDiversityPC(
                    currentPopAsDNAList,
                    ops.CrossoverRate,
                    ops.MutationRate,
                    ops.MaxStepSize,
                    ops.ProbabilityC,
                    Rand);
            }
            else
            {
                newPopulationAsDNAList = GeneticLearning.GeneratePopulationFromFitnessPC(
                    currentPopAsDNAList,
                    ops.CrossoverRate,
                    ops.MutationRate,
                    ops.MaxStepSize,
                    ops.ProbabilityC,
                    Rand);
            }
            
            for (int i = 0; i < newPopulationAsDNAList.Count; i++)
            {
                newPopulation.Add((NeuralNetwork)newPopulationAsDNAList[i]);
            }

            population = newPopulation;
        }

        /*
         * Runs the given network on a new instance of the game
         * and renders the gameplay as it goes. The paramaters object must be
         * castable to a SimulationEngine.RunParameters object
        */
        public override void RunNetworkForGui(Object parameters)
        {
            RunInGuiParameters ops = (RunInGuiParameters)parameters;
            NeuralNetwork candidate = ops.Network;
            SnakeGame gameInstance = new SnakeGame(Rand, 25);
            gameInstance.Init();

            long millisPerFrame = 1000 / ops.FPS;
            bool gameOver = false;
            while (!gameOver)
            {
                long frameStartTime = DateTime.UtcNow.Millisecond;
                WriteGameStateToTextBox(ops.TextBox2, gameInstance);

                // Interprete game state to inputs for the neural network
                Matrix<float> inputs = GameStateToNetworkInput(gameInstance);

                // Run inputs through network and retrieve output
                Matrix<float> output = candidate.ProcessInput(inputs);


                // interprete network output to game move
                gameInstance.RegisterMove(NetworkOutputToMove(output, gameInstance));

                // render
                gameInstance.Render(ops);

                // go to next turn in game
                gameOver = gameInstance.FinishTurn();

                // Wait until next Frame
                long currentTime = DateTime.UtcNow.Millisecond;
                long frameDuration = currentTime - frameStartTime;
                if (frameDuration < millisPerFrame)
                {
                    long sleepDuration = millisPerFrame - frameDuration;
                    System.Threading.Thread.Sleep((int)sleepDuration);
                }
            }

            WriteGameStateToTextBox(ops.TextBox2, gameInstance);
        }

        /*
         * Uses the activation of the output layer of a network to 
         * select a move for the game.
        */
        public override int NetworkOutputToMove(Matrix<float> output, NetworkPlayableGame g)
        {
            Direction facingDirection = ((SnakeGame)g).SnakeDirection;
            Vector<float> ou = output.Column(0);

            if (ou.Count() != 3)
            {
                throw new ArgumentException("Output dimension does not match method for extracting move from output");
            }

            switch (ou.MaximumIndex())
            {
                case 0: // choosing to go straight
                    return (int)facingDirection;
                case 1: // choosing to swing a right
                    return ((int)facingDirection + 1) % 4;
                case 2: // making a hard left
                    return ((int)facingDirection + 3) % 4;
            }

            return (int)facingDirection;
        }

        public override Matrix<float> GameStateToNetworkInput(NetworkPlayableGame g)
        {
            SnakeGame gameInstance = (SnakeGame)g;
            List<float> inputs = new List<float>(24);
            var grid = gameInstance.Grid;

            // loop through all 8 directions starting from head
            for (var i = 0; i < 8; i++)
            {
                var head = gameInstance.SnakeHead;
                int xDelta = 0;
                int yDelta = 0;

                // Trying out new perspective input method
                DeltasForDirectionFacing(i, gameInstance.SnakeDirection, out xDelta, out yDelta);

                int numValuesFound = 0;
                int numSpaces = 0;
                bool[] valuesFound = { false, false, false };
                float[] values = { 0f, 0f, 0f };
                while (numValuesFound < values.Count()) // one distance value for each type of cell in this direction
                {
                    head.X += xDelta;
                    head.Y += yDelta;
                    Cell cellHere;

                    // Wall
                    if (head.X < 0 || head.X >= gameInstance.GridSideLength ||
                        head.Y < 0 || head.Y >= gameInstance.GridSideLength)
                    {
                        cellHere = Cell.Wall;
                    }
                    else
                    {
                        cellHere = grid[head.X][head.Y];
                        if (cellHere == Cell.Empty)
                        {
                            numSpaces++;
                            continue;
                        }
                    }

                    int indexHere = (int)cellHere - 1;

                    if (!valuesFound[indexHere]) // This type of cell has not been found in this direction yet
                    {
                        values[indexHere] = 1f - ((float)numSpaces / (float)(gameInstance.GridSideLength - 1));
                        valuesFound[indexHere] = true;
                        if (cellHere == Cell.Wall)
                        {
                            // after wall is found nothing else can be found in that direction
                            break;
                        }
                        numValuesFound++;
                    }

                    numSpaces++;
                }
                inputs.AddRange(values);
            }
            return CreateMatrix.Dense<float>(inputs.Count(), 1, inputs.ToArray());
        }

        /*
         * the int "direction" is between 0 and 7 and it indeicates which direction
         * you need the deltas for. 0 is straing up, 1 is up-right 2 is right etc..
         * the x and y Deltas are values that should be added to the grid indexes
         * in order for the next grid cell to be the next cell in the given direction.
         * Sorry, best explenation i could come up with.
        */
        private void DeltasForDirection(int direction, out int xDelta, out int yDelta)
        {
            switch (direction)
            {
                case 0: // Up
                    xDelta = -1;
                    yDelta = 0;
                    break;
                case 1: // Up-Right
                    xDelta = -1;
                    yDelta = 1;
                    break;
                case 2: // Right
                    xDelta = 0;
                    yDelta = 1;
                    break;
                case 3: // Down-Right
                    xDelta = 1;
                    yDelta = 1;
                    break;
                case 4: // Down
                    xDelta = 1;
                    yDelta = 0;
                    break;
                case 5: // Down-Left
                    xDelta = 1;
                    yDelta = -1;
                    break;
                case 6: // Left
                    xDelta = 0;
                    yDelta = -1;
                    break;
                case 7: // Up-Left
                    xDelta = -1;
                    yDelta = -1;
                    break;
                default:
                    xDelta = 0;
                    yDelta = 0;
                    break;
            }
        }

        /*
         * This function adds another layer to the deltas function by
         * giving you the deltas for traversing the grid but but with reference
         * to the direction the snake is facing
        */
        private void DeltasForDirectionFacing(int direction, Direction facing, out int xDelta, out int yDelta)
        {
            switch(facing)
            {
                case Direction.Up:
                    DeltasForDirection(direction, out xDelta, out yDelta);
                    break;
                case Direction.Right:
                    DeltasForDirection(((direction + 2) % 8), out xDelta, out yDelta);
                    break;
                case Direction.Down:
                    DeltasForDirection(((direction + 4) % 8), out xDelta, out yDelta);
                    break;
                case Direction.Left:
                    DeltasForDirection(((direction + 6) % 8), out xDelta, out yDelta);
                    break;
                default:
                    xDelta = 0;
                    yDelta = 0;
                    return;
            }
        }

        /*
         * Creates a new random network for the snake gam
        */
        public override NeuralNetwork CreateNetwork()
        {
            var network = new NeuralNetwork(24, Rand);
            network.AddLayer(9, ActivationType.Relu);
            network.AddLayer(3, ActivationType.Softmax);
            return network;
        }

        /*
         * Writes current game state to the given text box
        */
        private void WriteGameStateToTextBox(RichTextBox tb, SnakeGame gameInstance)
        {
            string str = $"Game Over: {gameInstance.GameOver}\nTurn Number: {gameInstance.TurnNumber}\nScore: {gameInstance.Score}";
            tb.BeginInvoke((MethodInvoker)delegate
            {
                tb.Text = str;
            });
        }
    }
}
