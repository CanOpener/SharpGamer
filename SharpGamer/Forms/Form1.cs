using SharpGamer.Simulation_Engine.Games;
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

namespace SharpGamer.Forms
{
    public partial class Form1 : Form
    {
        private Thread workerThread = null;
        private Snake game;

        public Form1()
        {
            InitializeComponent();
            game = new Snake(500, 500, ref pictureBox1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void start_Click(object sender, EventArgs e)
        {
            game = new Snake(500, 500, ref pictureBox1);
            workerThread = new System.Threading.Thread(new System.Threading.ThreadStart(game.runUserGame));
            workerThread.Start();
        }

        private void go_n_click(object sender, EventArgs e)
        {

        }

        private void pause_click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            game.addKeyPress(e);
        }
    }
}
