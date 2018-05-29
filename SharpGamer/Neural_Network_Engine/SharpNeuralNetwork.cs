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
    public class SharpNeuralNetwork
    {
        public int numHiddenLayers = 0;
        public int inputLayerSize;
        public List<Matrix<float>> allWeights;
        public List<Matrix<float>> allBiases;
        public Random rand;

        public SharpNeuralNetwork(int inputSize)
        {
            inputLayerSize = inputSize;
            allWeights = new List<Matrix<float>>();
            allBiases = new List<Matrix<float>>();
            rand = new Random();
        }

        public SharpNeuralNetwork(SharpNeuralNetwork nn)
        {
            inputLayerSize = nn.inputLayerSize;

            allWeights = new List<Matrix<float>>(nn.allWeights.Count);
            for (int i=0; i<nn.allWeights.Count; i++)
            {
                Matrix<float> r = nn.allWeights[i];
                Matrix<float> weightCopy = CreateMatrix.Dense<float>(r.RowCount, r.ColumnCount, r.ToColumnMajorArray());
                allWeights.Add(weightCopy);
            }

            allBiases = new List<Matrix<float>>(nn.allBiases.Count);
            for (int i = 0; i < nn.allBiases.Count; i++)
            {
                Matrix<float> r = nn.allBiases[i];
                Matrix<float> biasCopy = CreateMatrix.Dense<float>(r.RowCount, r.ColumnCount, r.ToColumnMajorArray());
                allBiases.Add(biasCopy);
            }

            rand = new Random();
        }

        public void addLayer(int numNeurons)
        {
            int previousLayerSize = inputLayerSize;
            if (numHiddenLayers > 0)
            {
                previousLayerSize = allBiases[allBiases.Count - 1].RowCount;
            }
            
            // HERE IS THE PROBLEM
            Matrix<float> weights = Matrix<float>.Build.Dense(numNeurons, previousLayerSize, generateNewWeightSet(numNeurons*previousLayerSize));
            Matrix<float> biases = Matrix<float>.Build.Dense(numNeurons, 1, generateNewWeightSet(numNeurons));

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
        public float[] generateNewWeightSet(int size)
        {
            List<float> l = new List<float>(size);
            for (int i=0; i<size; i++)
            {
                l.Add(generateNewWeight());
            }
            return l.ToArray();
        }
        public float generateNewWeight()
        {
            return (float)((rand.NextDouble()*20)-10);
        }

        public float generateNewBias()
        {
            return (float)(rand.NextDouble());
        }

    }
}
