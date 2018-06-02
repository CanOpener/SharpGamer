using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace SharpGamer.NeuralNetworkEngine
{
    /*
     * This class holds a neural network with the ability
     * to feedforward inputs and return outputs. Implements
     * the DNA interface for use with Genetic algorithms.
    */
    class NeuralNetwork
    {
        public string Id { get; }
        public int InputLayerSize { get; }
        public int NumLayers { get; }
        public List<Matrix<float>> AllWeights { get; }
        public List<Matrix<float>> AllBiases { get; }
        public Random Rand { get; set; }

        // All weight and bias properties in one array.
        private float[] dnaStrand;
        
        /*
         * Public constructor for all in one line instantiation.
        */ 
        public NeuralNetwork(int inLayerSize)
        {
            this.InputLayerSize = inLayerSize;
        }

    }
}
