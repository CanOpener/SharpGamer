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
    internal class Simulation
    {
        public NetworkPlayableGame g;
        public SharpNeuralNetwork n;
        public int s; // score

        public Simulation(NetworkPlayableGame g, SharpNeuralNetwork n, int s)
        {
            this.g = g;
            this.n = n;
            this.s = s;
        }
    }


    internal class SimulationComparer : IComparer<Simulation>
    {
        public int Compare(Simulation x, Simulation y)
        {
            if (x.s > y.s)
            {
                return -1;
            }
            if (x.s < y.s)
            {
                return 1;
            }
            return 0;
        }
    }

    class SharpSnakePlayer : SharpPlayer
    {
        private int populationMax;
        private int generation = 1;
        private List<SharpNeuralNetwork> population;
        
        public SharpSnakePlayer(int pop)
        {
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
            public runNextGenerationParams(ProgressBar pb, RichTextBox tb)
            {
                this.progressBar = pb;
                this.textBox = tb;
            }
        }

        public void runNextGeneration(Object obj)
        {
            runNextGenerationParams p = (runNextGenerationParams)obj;

            List<Simulation> sims = new List<Simulation>(populationMax);

            // instantiate a game for each entity
            foreach (SharpNeuralNetwork network in population)
            {
                NetworkPlayableGame snakeGameInstance = new Snake(25, new Random());
                snakeGameInstance.init();
                sims.Add(new Simulation(snakeGameInstance, network, 0));
            }
            
            int turnNumber = 1;
            int numFinished = 0;

            // Fix this shit TODO
            while (numFinished < population.Count)
            {
                int previousNumFinished = numFinished;

                for (int i = 0; i < sims.Count; i++)
                {
                    Simulation sim = sims[i];

                    // interprete game board to network inputs
                    SnakeGameState state = (SnakeGameState)sim.g.getGameState();
                    if (state.gameOver)
                    {
                        continue;
                    }
                    Matrix<float> inputs = gameStateToInputs(state);

                    // get move from network
                    Matrix<float> output = sim.n.feedForwardInput(inputs);

                    // interprete network output to game move
                    sim.g.registerMove(networkOutputToMove(output));

                    // go to next turn in game
                    bool gameOver = sim.g.finishTurn();

                    // if game over store index for removal
                    if (gameOver)
                    {
                        numFinished++;
                        sim.s = ((SnakeGameState)(sim.g.getGameState())).score;
                    }
                }

                if (numFinished != previousNumFinished)
                {
                    // update progress bar
                }

                String strToWrite = $"{numFinished} Done, {population.Count-numFinished} left.\n";
                strToWrite += $"Turn: {turnNumber}\n";

                p.textBox.BeginInvoke((MethodInvoker)delegate
                {
                    p.textBox.Text = strToWrite;
                });
                turnNumber++;
            }

            String str = $"Done.\n";
            str += $"Turn: {turnNumber}\n";

            p.textBox.BeginInvoke((MethodInvoker)delegate
            {
                p.textBox.Text = str;
            });

            // sort in order of score
            sims.Sort(new SimulationComparer());
            List<SharpNeuralNetwork> newPop = new List<SharpNeuralNetwork>(sims.Count);
            Console.WriteLine($"s:{sims[0].s} , e:{sims[sims.Count - 1].s}\n");
            
            for (int i=0; i<sims.Count; i++)
            {
                newPop.Add(sims[i].n);
            }

            population = newPop;
        }

        public struct runBestOnScreenParams
        {
            public int pixelsw, pixelsh, fps;
            public PictureBox screen;
            public runBestOnScreenParams(int w, int h, int f, PictureBox p)
            {
                this.pixelsw = w;
                this.pixelsh = h;
                this.fps = f;
                this.screen = p;
            }
        }

        public void runBestOnScreen(Object obj) 
        {
            runBestOnScreenParams p = (runBestOnScreenParams)obj;

            SharpNeuralNetwork candidate = population[0];
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
                bool[] valuesFound = { false, false, false, false };
                float[] values = { 1f, 1f, 1f, 1f };
                while (numValuesFound < 4) // one distance value for each type of cell in this direction
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
                    }

                    if (valuesFound[(int)cellHere] == false) // This type of cell has not been found in this direction yet
                    {
                        values[(int)cellHere] = 1f - ((float)numSpaces / (float)(g.gridSideLength - 1));
                        valuesFound[(int)cellHere] = true;
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
            return CreateMatrix.Dense<float>(32, 1, inputs.ToArray());
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
            SharpNeuralNetwork nn = new SharpNeuralNetwork(32);
            nn.addLayer(16);
            nn.addLayer(4);
            return nn;
        }
    }
}
