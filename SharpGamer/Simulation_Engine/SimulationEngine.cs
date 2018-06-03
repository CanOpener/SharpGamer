using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SharpGamer.SimulationEngine
{
    /* 
     * This is where the graphical simulation engine
     * is created. It can be used to run certain 
     * neural netowork engines on different n player
     * games.
    */
    class SimulationEngine
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Couldnt find this property in the Design tab...
            Form f = new Forms.Form1();
            f.KeyPreview = true;

            Application.Run(f);
        }
    }

    /*
     * This structure is used for transmitting run parameters
     * from the simulation engine to the player doing the running
    */
    struct RunParameters
    {
        public ProgressBar ProgressBarObject { get; set; }
        public RichTextBox TextBox1 { get; set; }
        public RichTextBox TextBox2 { get; set; }
        public double MutationRate { get; set; }
        public double CrossoverRate { get; set; }
        public double MaxStepSize { get; set; }
        public int NumGenerations { get; set; }
        public double ProbabilityC { get; set; }
        public bool UseDiversity { get; set; }

        public RunParameters(ProgressBar pb, RichTextBox tb1, RichTextBox tb2, double mr,
            double cr, double mss, int numGens, double pc, bool dc)
        {
            ProgressBarObject = pb;
            TextBox1 = tb1;
            TextBox2 = tb2;
            MutationRate = mr;
            CrossoverRate = cr;
            MaxStepSize = mss;
            NumGenerations = numGens;
            ProbabilityC = pc;
            UseDiversity = dc;
        }
    }

    /*
     * This structure is used for transmitting run parameters
     * from the simulation engine to the player but this struct
     * contains information for running a game within the GUI
     * display
    */
    struct RunInGuiParameters
    {
        public PictureBox Screen { get; set; }
        public RichTextBox TextBox1 { get; set; }
        public RichTextBox TextBox2 { get; set; }
        public int PixelsW { get; set; }
        public int PixelsH { get; set; }
        public int FPS { get; set; }
        public int MaxTurns { get; set; }
        public NeuralNetworkEngine.NeuralNetwork Network { get; set; }

        public RunInGuiParameters(PictureBox pb, RichTextBox tb1, RichTextBox tb2,
            int pw, int ph, int fps, int max, NeuralNetworkEngine.NeuralNetwork net)
        {
            Screen = pb;
            TextBox1 = tb1;
            TextBox2 = tb2;
            PixelsW = pw;
            PixelsH = ph;
            FPS = fps;
            MaxTurns = max;
            Network = net;
        }
    }
}
