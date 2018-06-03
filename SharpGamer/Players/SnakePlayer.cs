using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGamer.NeuralNetworkEngine;
using SharpGamer.SimulationEngine;
using SharpGamer.Games;
using SharpGamer.Games.SnakeGame;
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
        public void init()
        {
            List<NeuralNetwork> newPop = new List<NeuralNetwork>(PopulationMax);
            while(newPop.Count() < PopulationMax)
            {
                newPop.Add(CreateNetwork());
            }
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
            foreach (var _ in population)
            {
                SnakeGame newGame = new SnakeGame(25);
                newGame.init();
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
                    Matrix<float> output = network.FeedForwardInput(inputs);

                    // Interprete network output to game move.
                    // gameInstance.registerMove(networkOutputToMove(output));
                    // gameInstance.registerMove(networkOutputFacingToMove(output, state.snakeDirection));
                    gameInstance.registerMove(NetworkOutputToMove(output));

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
                newPopulationAsDNAList = GeneticLearning.generateNewPopulationFromDiversityAndFitness(
                    currentPopAsDNAList,
                    ops.CrossoverRate,
                    ops.MutationRate,
                    ops.MaxStepSize,
                    ops.ProbabilityC,
                    PopulationMax,
                    Rand);
            }
            else
            {
                newPopulationAsDNAList = GeneticLearning.generateNewPopulationFromPcSelection(
                    currentPopAsDNAList,
                    ops.CrossoverRate,
                    ops.MutationRate,
                    ops.MaxStepSize,
                    ops.ProbabilityC,
                    PopulationMax,
                    Rand);
            }
            
            for (int i = 0; i < newPopulationAsDNAList.Count; i++)
            {
                newPopulation.Add((NeuralNetwork)newPopulationAsDNAList[i]);
            }

            population = newPopulation;
            generationNumber++;
        }

        /*
         * Runs the given network on a new instance of the game
         * and renders the gameplay as it goes. The paramaters object must be
         * castable to a SimulationEngine.RunParameters object
        */
        public abstract void RunNetworkForGui(Object parameters)
        {
            RunInGuiParameters ops = (RunInGuiParameters)parameters;
            NeuralNetwork candidate = ops.Network;
            SnakeGame newGame = new SnakeGame(ops.PixelsW, ops.PixelsH, ref p.screen, 25);
            newGame.init();

            long millisPerFrame = 1000 / p.fps; // --------------------------------------------------------------------TODO---------------------------------------------------------------------------------------------

            bool gameOver = false;
            while (!gameOver)
            {
                long frameStartTime = DateTime.UtcNow.Millisecond;

                // interprete game board to network inputs
                SnakeGameState state = (SnakeGameState)newGame.getGameState();

                Matrix<float> inputs = gameStateToInputs(state);

                // get move from network
                Matrix<float> output = candidate.feedForwardInput(inputs);
                for (int i = 0; i < 3; i++)
                {
                    Console.Write($"{output.At(i, 0)},");

                }
                Console.WriteLine("");


                // interprete network output to game move
                //newGame.registerMove(networkOutputToMove(output));
                newGame.registerMove(networkOutputFacingToMove(output, state.snakeDirection));

                // render
                newGame.render();

                // go to next turn in game
                gameOver = newGame.finishTurn();

                // Wait until next Frame
                long currentTime = DateTime.UtcNow.Millisecond;
                long frameDuration = currentTime - frameStartTime;
                if (frameDuration < millisPerFrame)
                {
                    long sleepDuration = millisPerFrame - frameDuration;
                    System.Threading.Thread.Sleep((int)sleepDuration);
                }
            }

            MessageBox.Show($"Score : {(((SnakeGameState)(newGame.getGameState())).score)}");
        }

        public void runBestOnScreen(Object obj) 
        {


        }

        public int networkOutputToMove(Matrix<float> output)
        {
            Vector<float> ou = output.Column(0);
            return ou.MaximumIndex();
        }

        public int networkOutputFacingToMove(Matrix<float> output, Direction facingDirection)
        {
            Vector<float> ou = output.Column(0);

            switch(ou.MaximumIndex())
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

        public Matrix<float> gameStateToInputs(GameState gs)
        {
            SnakeGameState g = (SnakeGameState)gs;
            List<float> inputs = new List<float>(32);
            
            // loop through all 8 directions starting from head
            for (int i=0; i<8; i++)
            {
                Point head = g.snakeHead;
                int xDelta = 0;
                int yDelta = 0;

                // Trying out new perspective input method
                //deltasForDirection(i, out xDelta, out yDelta);
                deltasForDirectionFacing(i, g.snakeDirection, out xDelta, out yDelta);

                int numValuesFound = 0;
                int numSpaces = 0;
                bool[] valuesFound = { false, false, false };
                float[] values = { 0f, 0f, 0f };
                while (numValuesFound < values.Count()) // one distance value for each type of cell in this direction
                {
                    head.x += xDelta;
                    head.y += yDelta;
                    Cell cellHere;
                    
                    // Wall
                    if (head.x < 0 || head.x >= g.gridSideLength ||
                        head.y < 0 || head.y >= g.gridSideLength)
                    {
                        cellHere = Cell.Wall;
                    }
                    else
                    {
                        cellHere = g.grid[head.x][head.y];
                        if (cellHere == Cell.Empty)
                        {
                            continue;
                        }
                    }

                    int indexHere = (int)cellHere - 1;

                    if (valuesFound[indexHere] == false) // This type of cell has not been found in this direction yet
                    {
                        values[indexHere] = 1f - ((float)numSpaces / (float)(g.gridSideLength - 1));
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

        private void deltasForDirection(int direction, out int xDelta, out int yDelta)
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

        private void deltasForDirectionFacing(int direction, Direction facing, out int xDelta, out int yDelta)
        {
            switch(facing)
            {
                case Direction.Up:
                    deltasForDirection(direction, out xDelta, out yDelta);
                    break;
                case Direction.Right:
                    deltasForDirection(((direction + 2) % 8), out xDelta, out yDelta);
                    break;
                case Direction.Down:
                    deltasForDirection(((direction + 4) % 8), out xDelta, out yDelta);
                    break;
                case Direction.Left:
                    deltasForDirection(((direction + 6) % 8), out xDelta, out yDelta);
                    break;
                default:
                    xDelta = 0;
                    yDelta = 0;
                    return;
            }
        }

        public SharpNeuralNetwork createNetworkV1()
        {
            SharpNeuralNetwork nn = new SharpNeuralNetwork(24);
            nn.addLayer(12);
            nn.addLayer(3);
            return nn;
        }
    }
}
