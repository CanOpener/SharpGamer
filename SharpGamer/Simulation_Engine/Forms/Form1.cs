using SharpGamer.Games;
using SharpGamer.Players;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics;

namespace SharpGamer.SimulationEngine.Forms
{
    public partial class Form1 : Form
    {
        private Thread workerThread = null;
        private SnakeGame game;
        private SnakePlayer player;
        private Random rand;

        public Form1()
        {
            InitializeComponent();
            rand = new Random();
            player = new SnakePlayer(1000, rand);
            player.Init();
            game = new SnakeGame(rand, 25);
            game.Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void start_Click(object sender, EventArgs e)
        {
            var runParams = new RunParameters(
                progressBar1,
                richTextBox1,
                richTextBox2,
                (double)mutationRatePicker.Value,
                (double)crossOverRatePicker.Value,
                (double)maxStepSizePicker.Value,
                (int)numGenerationsPicker.Value,
                (double)pcPicker.Value,
                diversityCB.Checked);

            ParameterizedThreadStart start = new ParameterizedThreadStart(player.RunNGenerations);
            workerThread = new Thread(start);
            workerThread.Start(runParams);
            richTextBox2.Text = $"Mutation Rate: {(double)mutationRatePicker.Value}\n" +
                $"Crossover Rate: {(double)crossOverRatePicker.Value}\n" +
                $"Max Step Size: {(double)maxStepSizePicker.Value}\n" +
                $"Num Generations: {(int)numGenerationsPicker.Value}\n" +
                $"Pc: {(double)pcPicker.Value}";
        }

        private void go_n_click(object sender, EventArgs e)
        {

        }

        private void pause_click(object sender, EventArgs e)
        {
            var runGuiParams = new RunInGuiParameters(
                pictureBox1,
                richTextBox1,
                richTextBox2,
                500,
                500,
                20,
                1000, // fix this param
                player.Population[(int)networkPicker.Value]);
            ParameterizedThreadStart start = new ParameterizedThreadStart(player.RunNetworkForGui);
            workerThread = new Thread(start);
            workerThread.Start(runGuiParams);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            game.AddKeyPress(e);
        }
    }
}
