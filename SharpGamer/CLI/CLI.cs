using System;
using System.Collections.Generic;
using System.Text;
using SharpGamer.SimulationEngine;
using SharpGamer.NeuralNetworkEngine;
namespace SharpGamer.CLI
{
    /*
     * This is generally for debugging the different
     * the different parts of the project. Uses consol
     * for quick and easy changes.
    */
    class CLI
    {
        static void Main(string[] args)
        {
            float[] inputs = { 0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f };

            var sig = new NeuralNetworkEngine.ActivationFunctions.SigmoidActivation();
            var sof = new NeuralNetworkEngine.ActivationFunctions.SoftmaxActivation();
            var modSof = new NeuralNetworkEngine.ActivationFunctions.ReluActivation();

            drawArray(sig.ActivateAll(inputs));
            drawArray(sof.ActivateAll(inputs));
            drawArray(modSof.ActivateAll(inputs));

        }

        public static void drawArray(float[] arr)
        {
            float sum = 0;
            foreach (float f in arr)
            {
                Console.Write($"{f}, ");
                sum += f;
            }
            Console.WriteLine($"\n... {sum}\n");
        }
    }
}
