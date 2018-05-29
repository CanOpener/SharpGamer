using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace SharpGamer.Neural_Network_Engine
{
    public static class GeneticLearning
    {
        public static SharpNeuralNetwork produceMutation(SharpNeuralNetwork nn, double mutationRate)
        {
            SharpNeuralNetwork copy = new SharpNeuralNetwork(nn);
            List<Matrix<float>> allWeights = copy.allWeights;
            List<Matrix<float>> allBiases = copy.allBiases;

            foreach (Matrix<float> currentWeight in allWeights)
            {
                for (int i=0; i<currentWeight.RowCount; i++)
                {
                    for (int j=0; j<currentWeight.ColumnCount; j++)
                    {
                        if (new Random().NextDouble() < mutationRate)
                        {
                            currentWeight.At(i, j, copy.generateNewWeight());
                        }
                    }
                }
            }

            foreach (Matrix<float> currentBias in allBiases)
            {
                for (int i = 0; i < currentBias.RowCount; i++)
                {
                    for (int j = 0; j < currentBias.ColumnCount; j++)
                    {
                        if (new Random().NextDouble() < mutationRate)
                        {
                            currentBias.At(i, j, copy.generateNewWeight());
                        }
                    }
                }
            }

            return copy;
        }
    }
}
