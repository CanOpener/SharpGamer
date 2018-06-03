using SharpGamer.Games;
using SharpGamer.Games.SnakeGame;
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

        public Form1()
        {
            InitializeComponent();
            player = new SnakePlayer(100000);
            player.init();
            game = new SnakeGame(500, 500, ref pictureBox1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void start_Click(object sender, EventArgs e)
        {
            ParameterizedThreadStart start = new ParameterizedThreadStart(player.runNGenerations);
            workerThread = new Thread(start);
            workerThread.Start(new Players.SnakePlayer.runNextGenerationParams(progressBar1, richTextBox1, (double)mutationRatePicker.Value,
                (double)crossOverRatePicker.Value, (double)maxStepSizePicker.Value, (int)numGenerationsPicker.Value, (double)pcPicker.Value, diversityCB.Checked));
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
            ParameterizedThreadStart start = new ParameterizedThreadStart(player.runBestOnScreen);
            workerThread = new Thread(start);
            workerThread.Start(new Players.SnakePlayer.runBestOnScreenParams(500, 500, 10, (int)networkPicker.Value, pictureBox1));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            game.addKeyPress(e);
        }
    }
}
