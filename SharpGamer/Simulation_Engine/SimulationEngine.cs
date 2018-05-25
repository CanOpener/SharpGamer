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
}
