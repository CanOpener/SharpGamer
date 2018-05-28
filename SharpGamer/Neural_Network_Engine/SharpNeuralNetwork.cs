using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace SharpGamer.Neural_Network_Engine
{
    /*
     * This class must cover the following functionality:
     * 
     * 1. Customisable MLP structure neural network.
     * 2. Ability to feed forward input.
     * 3. Weights and Biases
    */
    class SharpNeuralNetwork
    {
        private int numHiddenLayers = 0;
        private int inputLayerSize;
        private List<Matrix<float>> allWeights;
        private List<Matrix<float>> allBiases;
        private Random rand;

        public SharpNeuralNetwork(int inputSize)
        {
            inputLayerSize = inputSize;
            allWeights = new List<Matrix<float>>();
            allBiases = new List<Matrix<float>>();
            rand = new Random();
        }

        public void addLayer(int numNeurons)
        {
            int previousLayerSize = inputLayerSize;
            if (numHiddenLayers > 0)
            {
                previousLayerSize = allBiases[allBiases.Count - 1].RowCount;
            }
            
            Matrix<float> weights = Matrix<float>.Build.Dense(numNeurons, previousLayerSize, generateNewWeight);
            Matrix<float> biases = Matrix<float>.Build.Dense(numNeurons, 1, generateNewBias);

            allWeights.Add(weights);
            allBiases.Add(biases);
            numHiddenLayers++;
        }

        public Matrix<float> feedForwardInput(Matrix<float> input)
        {
            if (input.RowCount*input.ColumnCount != inputLayerSize)
            {
                System.Console.WriteLine("Can't run this input, invalid dimensions");
                MessageBox.Show("Can't run this input, invalid dimensions");
                return null;
            }

            Matrix<float> previousActivation = input;
            for (int i=0; i<numHiddenLayers; i++)
            {
                Matrix<float> activation = allWeights[i].Multiply(previousActivation);
                activation = activation.Add(allBiases[i]);

                Vector<float> toActivate = activation.Column(0);
                for (int j=0; j<toActivate.Count; j++)
                {
                    toActivate[j] = (float)MathNet.Numerics.SpecialFunctions.Logistic((double)toActivate[j]); // sigmoid
                }
                activation.SetColumn(0, toActivate);
                previousActivation = activation;
            }

            return previousActivation;
        }

        /*
         * Used for generating new weight and bias values between 0 and 1
        */
        private float generateNewWeight(int x, int y)
        {
            return (float)((rand.NextDouble()*8)-4);
        }

        private float generateNewBias(int x, int y)
        {
            return (float)(rand.NextDouble());
        }

    }
}
