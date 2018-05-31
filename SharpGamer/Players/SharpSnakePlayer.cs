using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGamer.Neural_Network_Engine;
using SharpGamer.Games;
using SharpGamer.Games.Snake;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace SharpGamer.Players
{

    class SharpSnakePlayer : SharpPlayer
    {
        private int populationMax;
        private int generation = 1;
        private List<SharpNeuralNetwork> population;
        private Random rand;

        public SharpSnakePlayer(int pop, Random r = null)
        {
            if (r == null) rand = new Random();
            else rand = r;
            this.populationMax = pop;
        }

        public void init()
        {
            // populate
            population = new List<SharpNeuralNetwork>(populationMax);
            while(population.Count < populationMax)
            {
                population.Add(createNetworkV1());
            }
        }

        public struct runNextGenerationParams
        {
            public ProgressBar progressBar;
            public RichTextBox textBox;
            public double mutationRate;
            public double crossoverRate;
            public double maxStepSize;
            public int numGenerations;
            public double pc;
            public runNextGenerationParams(ProgressBar pb, RichTextBox tb, double mr, double cr, double mss, int numGens, double pc)
            {
                this.progressBar = pb;
                this.textBox = tb;
                this.mutationRate = mr;
                this.crossoverRate = cr;
                this.maxStepSize = mss;
                this.numGenerations = numGens;
                this.pc = pc;
            }
        }

        public void runNGenerations(Object obj)
        {
            runNextGenerationParams p = (runNextGenerationParams)obj;

            for (int i=0; i<p.numGenerations; i++)
            {
                runNextGeneration(obj);
            }
        }

        public void runNextGeneration(Object obj)
        {
            runNextGenerationParams p = (runNextGenerationParams)obj;
            long generationStartTime = DateTime.UtcNow.Second;

            // mutate population based on previous results
            if (generation != 1)
            {
                mutatePopulation(p);
            }

            // instantiate a game for each entity
            List<Snake> games = new List<Snake>(population.Count());
            for (int i=0; i<population.Count(); i++)
            {
                Snake newGame = new Snake(25);
                newGame.init();
                games.Add(newGame);
            }
            
            int turnNumber = 1;
            int numFinished = 0;

            // Fix this shit TODO
            while (numFinished < population.Count)
            {
                int previousNumFinished = numFinished;

                for (int i = 0; i < population.Count(); i++)
                {
                    SharpNeuralNetwork network = population[i];
                    Snake gameInstance = games[i];

                    // interprete game board to network inputs
                    SnakeGameState state = (SnakeGameState)gameInstance.getGameState();
                    if (state.gameOver)
                    {
                        continue;
                    }
                    Matrix<float> inputs = gameStateToInputs(state);

                    // get move from network
                    Matrix<float> output = network.feedForwardInput(inputs);

                    // interprete network output to game move
                    gameInstance.registerMove(networkOutputToMove(output));

                    // go to next turn in game
                    bool gameOver = gameInstance.finishTurn();

                    // if game over store index for removal
                    if (gameOver)
                    {
                        numFinished++;
                        //network.score = gameInstance.score;
                        network.score = gameInstance.score;
                    }
                }

                String strToWrite = $"{population.Count-numFinished} left.\n";
                strToWrite += $"Turn: {turnNumber}\n";

                p.textBox.BeginInvoke((MethodInvoker)delegate
                {
                    p.textBox.Text = strToWrite;
                });
                turnNumber++;
            }

            // sort list so [0] is fittest
            population = population.OrderBy(i => i.getScore()).ToList();
            population.Reverse();

            String str = $"Done.\n";
            str += $"Generation: {generation}\n";
            str += $"Turn: {turnNumber}\n";
            str += $"Best NN score: {population[0].score}\n";

            p.textBox.BeginInvoke((MethodInvoker)delegate
            {
                p.textBox.Text = str;
            });

            generation++;
        }

        private void mutatePopulation(runNextGenerationParams p)
        {
            if (generation == 1) return;
            List<DNA> pop = new List<DNA>(population.Count);
            foreach (SharpNeuralNetwork n in population)
            {
                pop.Add(n);
            }

            List<DNA> newPopulation = GeneticLearning.generateNewPopulationFromPcSelection(pop,
                p.crossoverRate, p.mutationRate, p.maxStepSize, p.pc, rand);

            for (int i = 0; i < newPopulation.Count; i++)
            {
                population[i] = (SharpNeuralNetwork)newPopulation[i];
            }
        }

        public struct runBestOnScreenParams
        {
            public int pixelsw, pixelsh, fps, index;
            public PictureBox screen;
            public runBestOnScreenParams(int w, int h, int f, int i, PictureBox p)
            {
                this.pixelsw = w;
                this.pixelsh = h;
                this.fps = f;
                this.index = i;
                this.screen = p;
            }
        }

        public void runBestOnScreen(Object obj) 
        {
            runBestOnScreenParams p = (runBestOnScreenParams)obj;

            if (p.index > population.Count()) p.index = 0;
            SharpNeuralNetwork candidate = population[p.index];
            Snake newGame = new Snake(p.pixelsw, p.pixelsh,ref p.screen, 25);
            newGame.init();

            long millisPerFrame = 1000 / p.fps;

            bool gameOver = false;
            while (!gameOver)
            {
                long frameStartTime = DateTime.UtcNow.Millisecond;

                // interprete game board to network inputs
                SnakeGameState state = (SnakeGameState)newGame.getGameState();

                Matrix<float> inputs = gameStateToInputs(state);

                // get move from network
                Matrix<float> output = candidate.feedForwardInput(inputs);
                for (int i=0; i<4; i++)
                {
                    Console.Write($"{output.At(i, 0)},");
                    
                }
                Console.WriteLine("");
                

                // interprete network output to game move
                newGame.registerMove(networkOutputToMove(output));

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

        public int networkOutputToMove(Matrix<float> output)
        {
            Vector<float> ou = output.Column(0);
            return ou.MaximumIndex();
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
                deltasForDirection(i, out xDelta, out yDelta);

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

        public SharpNeuralNetwork createNetworkV1()
        {
            SharpNeuralNetwork nn = new SharpNeuralNetwork(24);
            nn.addLayer(12);
            nn.addLayer(4);
            return nn;
        }
    }
}
