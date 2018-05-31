﻿using System;
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
    public class SharpNeuralNetwork : DNA
    {
        public int numHiddenLayers = 0;
        public int inputLayerSize;
        public List<Matrix<float>> allWeights;
        public List<Matrix<float>> allBiases;
        public Random rand;

        public int score { get; set; } = 0;
        public int getScore() { return score; }

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
                Matrix<float> weightCopy = CreateMatrix.Dense<float>(r.RowCount, r.ColumnCount);
                r.CopyTo(weightCopy);
                allWeights.Add(weightCopy);
            }
            
            allBiases = new List<Matrix<float>>(nn.allBiases.Count);
            for (int i = 0; i < nn.allBiases.Count; i++)
            {
                Matrix<float> r = nn.allBiases[i];
                Matrix<float> biasCopy = CreateMatrix.Dense<float>(r.RowCount, r.ColumnCount);
                r.CopyTo(biasCopy);
                allBiases.Add(biasCopy);
            }

            numHiddenLayers = nn.numHiddenLayers;

            rand = new Random();
        }

        public DNA clone()
        {
            return new SharpNeuralNetwork(this);
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
            Matrix<float> biases = Matrix<float>.Build.Dense(numNeurons, 1, generateNewBiasSet(numNeurons));

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

        public int getGenomeSize()
        {
            int size = 0;
            for (int i = 0; i < allWeights.Count(); i++)
            {
                int matrixCount = allWeights[i].RowCount * allWeights[i].ColumnCount;
                int biasMatrixCount = allBiases[i].RowCount * allBiases[i].ColumnCount;
                size += matrixCount + biasMatrixCount;
            }
            return size;
        }

        public DNA applyCrossOver(DNA d)
        {
            // TODO do this properly
            SharpNeuralNetwork n = new SharpNeuralNetwork(this);


            return d;
        }

        public void mutateAt(int index, double step, bool positive)
        {
            int currentIndex = 0;
            for (int i=0; i<allWeights.Count(); i++)
            {
                int matrixCount = allWeights[i].RowCount * allWeights[i].ColumnCount;
                if (matrixCount + currentIndex > index)
                {
                    mutateValue(true, i, index - currentIndex, step, positive);
                    return;
                }
                currentIndex += matrixCount;

                int biasMatrixCount = allBiases[i].RowCount * allBiases[i].ColumnCount;
                if (biasMatrixCount + currentIndex > index)
                {
                    // mutation spot found do mutation
                    mutateValue(false, i, index - currentIndex, step, positive);
                    return;
                }
                currentIndex += biasMatrixCount;
            }
            Console.WriteLine($"Mutation Failed. Could not find index {index}");
        }

        private void mutateValue(bool weight, int startIndex, int matrixIndex, double step, bool positive)
        {
            Matrix<float> matrixToEdit = allWeights[startIndex]; ;
            if (!weight)
            {
                matrixToEdit = allBiases[startIndex];
            }

            int row = matrixIndex % matrixToEdit.RowCount;
            int column = matrixIndex / matrixToEdit.RowCount;

            float value = matrixToEdit.At(row, column);
            float delta = value * (float)step;

            value += (positive ? delta : -1 * delta);

            matrixToEdit.At(row, column, value);
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
        public float[] generateNewBiasSet(int size)
        {
            List<float> l = new List<float>(size);
            for (int i = 0; i < size; i++)
            {
                l.Add(generateNewBias());
            }
            return l.ToArray();
        }
        public float generateNewWeight()
        {
            return (float)((rand.NextDouble() * 8) - 4);
        }

        public float generateNewBias()
        {
            return (float)((rand.NextDouble() * 30) - 15);
        }

    }
}
