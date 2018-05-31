using SharpGamer.Games;
using SharpGamer.Games.Snake;
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

namespace SharpGamer.Forms
{
    public partial class Form1 : Form
    {
        private Thread workerThread = null;
        private Snake game;
        private SharpSnakePlayer player;

        public Form1()
        {
            InitializeComponent();
            player = new SharpSnakePlayer(10000);
            player.init();
            game = new Snake(500, 500, ref pictureBox1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void start_Click(object sender, EventArgs e)
        {
            ParameterizedThreadStart start = new ParameterizedThreadStart(player.runNGenerations);
            workerThread = new Thread(start);
            workerThread.Start(new Players.SharpSnakePlayer.runNextGenerationParams(progressBar1, richTextBox1,
                (double)mutationRatePicker.Value, (double)crossOverRatePicker.Value, (double)maxStepSizePicker.Value, (int)numGenerationsPicker.Value, (int)pcPicker.Value));
        }

        private void go_n_click(object sender, EventArgs e)
        {

        }

        private void pause_click(object sender, EventArgs e)
        {
            ParameterizedThreadStart start = new ParameterizedThreadStart(player.runBestOnScreen);
            workerThread = new Thread(start);
            workerThread.Start(new Players.SharpSnakePlayer.runBestOnScreenParams(500, 500, 10, (int)networkPicker.Value, pictureBox1));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            game.addKeyPress(e);
        }
    }
}
