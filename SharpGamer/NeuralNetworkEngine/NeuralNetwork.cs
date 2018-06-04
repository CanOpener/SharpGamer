using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGamer.NeuralNetworkEngine.ActivationFunctions;
using MathNet.Numerics.LinearAlgebra;

namespace SharpGamer.NeuralNetworkEngine
{
    /*
     * This class holds a neural network with the ability
     * to feedforward inputs and return outputs. Implements
     * the DNA interface for use with Genetic algorithms.
    */
    class NeuralNetwork : DNA
    {
        private string id;
        public string Id { get => id; }
        private int inputLayerSize;
        public int InputLayerSize { get => inputLayerSize; }
        private int numNonInputLayers;
        public int NumNonInputLayers { get => numNonInputLayers; }

        private List<Matrix<float>> layerWeights;
        public List<Matrix<float>> LayerWeights { get => layerWeights; }
        private List<Matrix<float>> layerBiases;
        public List<Matrix<float>> LayerBiases { get => layerBiases; }
        private List<Activation> layerActivations;
        public List<Activation> LayerActivations { get => layerActivations; }
        public Random Rand { get; set; }

        // These Properties are needed for the DNA interface
        public float Diversity { get; set; }
        public int Score { get; set; }
        public int GenomeSize {
            get {
                int gs = 0;
                for (var i=0; i<numNonInputLayers; i++)
                {
                    gs += (layerWeights[i].RowCount * layerWeights[i].ColumnCount);
                    gs += (layerBiases[i].RowCount * layerBiases[i].ColumnCount);
                }
                return gs;
            }
        }

        /*
         * Default constructor. Used with intent to build the neural network
         * with AddLayer()
        */
        public NeuralNetwork(int inputSize, Random r)
        {
            System.Diagnostics.Debug.Assert(inputSize > 0);

            this.id = System.Guid.NewGuid().ToString();
            this.inputLayerSize = inputSize;
            this.Rand = r;

            this.layerWeights = new List<Matrix<float>>();
            this.layerBiases = new List<Matrix<float>>();
            this.layerActivations = new List<Activation>();
        }

        /*
         * Public constructor for all in one line instantiation.
        */
        public NeuralNetwork(int inLayerSize, List<int> additionalLayerSizes, List<ActivationType> activations, Random r)
        {
            System.Diagnostics.Debug.Assert(additionalLayerSizes.Count() == activations.Count());
            
            this.id = System.Guid.NewGuid().ToString();
            this.inputLayerSize = inLayerSize;
            this.Rand = r;

            this.layerWeights = new List<Matrix<float>>(additionalLayerSizes.Count());
            this.layerBiases = new List<Matrix<float>>(additionalLayerSizes.Count());
            this.layerActivations = new List<Activation>(additionalLayerSizes.Count());

            for (var i=0; i<additionalLayerSizes.Count(); i++)
            {
                AddLayer(additionalLayerSizes[i], activations[i]);
            }
        }

        /*
         * Public constructor will constrct the neural network from the "DNA strand" given to it.
         * Based on the layer information it will copy the floats from the dna to their associated
         * weight/bias positions. Used for DNA purposes
        */
        public NeuralNetwork(int inLayerSize, List<int> additionalLayerSizes, List<ActivationType> activations, float[] values, Random r)
        {
            System.Diagnostics.Debug.Assert(additionalLayerSizes.Count() == activations.Count());

            this.id = System.Guid.NewGuid().ToString();
            this.inputLayerSize = inLayerSize;
            this.Rand = r;

            this.layerWeights = new List<Matrix<float>>(additionalLayerSizes.Count());
            this.layerBiases = new List<Matrix<float>>(additionalLayerSizes.Count());
            this.layerActivations = new List<Activation>(additionalLayerSizes.Count());

            int previousLayerSize = inLayerSize;
            int valuesIndex = 0;
            for (int i=0; i<additionalLayerSizes.Count(); i++)
            {
                int layerSize = additionalLayerSizes[i];
                ActivationType activation = activations[i];
                if (i > 0)
                {
                    previousLayerSize = additionalLayerSizes[i-1];
                }

                var weightsData = values.Skip(valuesIndex).Take(layerSize * previousLayerSize).ToArray();
                valuesIndex += layerSize * previousLayerSize;
                var biasData = values.Skip(valuesIndex).Take(layerSize).ToArray();
                valuesIndex += layerSize;

                Matrix<float> weights = Matrix<float>.Build.Dense(layerSize, previousLayerSize, weightsData);
                Matrix<float> biases = Matrix<float>.Build.Dense(layerSize, 1, biasData);
                layerWeights.Add(weights);
                layerBiases.Add(biases);

                layerActivations.Add(Activation.CreateActivation(activation));
                numNonInputLayers++;
            }
        }

        /*
         * This constructor clones the given neural network.
        */
        public NeuralNetwork(NeuralNetwork nn)
        {
            // ID always generated fresh
            this.id = System.Guid.NewGuid().ToString();
            this.inputLayerSize = nn.InputLayerSize;
            this.numNonInputLayers = nn.NumNonInputLayers;
            this.Rand = nn.Rand;

            this.layerWeights = new List<Matrix<float>>(nn.NumNonInputLayers);
            this.layerBiases = new List<Matrix<float>>(nn.NumNonInputLayers);
            this.layerActivations = new List<Activation>(nn.NumNonInputLayers);

            for (var i = 0; i < nn.NumNonInputLayers; i++)
            {
                Matrix<float> weights = nn.LayerWeights[i].Clone();
                Matrix<float> biases = nn.LayerBiases[i].Clone();
                Activation activation = Activation.CreateActivation(nn.LayerActivations[i].getType());

                this.layerWeights.Add(weights);
                this.layerBiases.Add(biases);
                this.layerActivations.Add(activation);
            }
        }

        /*
         * Adds a layer to the neural network.
         * This layer is added to the end so it will
         * become the output layer. Use this if building
         * your network line by line instead of all in the
         * constructor.
        */
        public void AddLayer(int layerSize, ActivationType activation)
        {
            int previousLayerSize = inputLayerSize;
            if (numNonInputLayers > 0)
            {
                previousLayerSize = LayerSize(NumNonInputLayers-1);
            }
            
            var weightsData = GenerateFloats(layerSize * previousLayerSize, true);
            var biasData = GenerateFloats(layerSize, false);

            Matrix<float> weights = Matrix<float>.Build.Dense(layerSize, previousLayerSize, weightsData);
            Matrix<float> biases = Matrix<float>.Build.Dense(layerSize, 1, biasData);
            layerWeights.Add(weights);
            layerBiases.Add(biases);

            layerActivations.Add(Activation.CreateActivation(activation));
            numNonInputLayers++;
        }

        /*
         * This function takes the given input matrix and feeds
         * it forward through all the layers of the network. The 
         * result of the output layer is then returned as a Matrix
         * of floats.
        */
        public Matrix<float> ProcessInput(Matrix<float> input)
        {
            if (input.RowCount * input.ColumnCount != inputLayerSize)
            {
                throw new ArgumentException("input size invalid");
            }

            Matrix<float> previousActivation = input;
            for (var i = 0; i < NumNonInputLayers; i++)
            {
                previousActivation = FeedForwardLayer(previousActivation, i);
            }

            return previousActivation;
        }

        /*
         * This function feeds the given input through the specified
         * layer of the network and returns its activated output. 
         * Throws an index out of bounds exception if the layer index
         * is invalid. 0 is the index of the first hidden layer.
        */
        public Matrix<float> FeedForwardLayer(Matrix<float> input, int layerIndex)
        {
            if (layerIndex < 0 || layerIndex >= NumNonInputLayers)
            {
                throw new IndexOutOfRangeException($"layer Index out of range. Layer index: {layerIndex}, Num hidden layers: {NumNonInputLayers}");
            }

            Matrix<float> weights = layerWeights[layerIndex];
            if (input.RowCount  != weights.ColumnCount)
            {
                throw new ArgumentException("input size invalid");
            }

            var multiplication = weights.Multiply(input);
            var withBiases = multiplication.Add(layerBiases[layerIndex]);
            
            float[] activated = layerActivations[layerIndex].ActivateAll(withBiases.Column(0).ToArray());
            return CreateMatrix.Dense<float>(activated.Length, 1, activated);
        }

        /*
         * This function is needed for the DNA Interface.
         * This is used in genetic algorithms
        */
        public DNA Clone()
        {
            return new NeuralNetwork(this);
        }

        /*
         * This function is needed for the DNA Interface.
         * It finds the weight/bias of the associated index
         * and changes it. The change is the step * current value
         * in the (bool)positive/negative direction
        */
        public void MutateGeneAtIndex(int index, double step, bool positive)
        {
            int currentIndex = 0;
            for (int i = 0; i < layerWeights.Count(); i++)
            {
                int matrixCount = layerWeights[i].RowCount * layerWeights[i].ColumnCount;
                if (matrixCount + currentIndex > index)
                {
                    int matrixIndex = index - currentIndex;
                    int row = matrixIndex % layerWeights[i].RowCount;
                    int column = matrixIndex / layerWeights[i].RowCount;

                    float value = layerWeights[i].At(row, column);
                    float delta = value * (float)step;
                    value += (positive ? delta : (-1 * delta));

                    layerWeights[i].At(row, column, value);
                    return;
                }
                currentIndex += matrixCount;

                int biasMatrixCount = layerBiases[i].RowCount * layerBiases[i].ColumnCount;
                if (biasMatrixCount + currentIndex > index)
                {
                    int matrixIndex = index - currentIndex;
                    int row = matrixIndex % layerBiases[i].RowCount;
                    int column = matrixIndex / layerBiases[i].RowCount;

                    float value = layerBiases[i].At(row, column);
                    float delta = value * (float)step;
                    value += (positive ? delta : (-1 * delta));

                    layerBiases[i].At(row, column, value);
                    return;
                }
                currentIndex += biasMatrixCount;
            }
            Console.WriteLine($"Mutation Failed. Could not find index {index}");
        }

        /*
         * This function combines all the values of the weights and biases
         * of each layer and returns them as one big column major
         * weight -> bias major array.
        */
        public float[] GetDNAStrand()
        {
            List<float> dnaStrand = new List<float>(GenomeSize);
            for (var i=0; i<numNonInputLayers; i++)
            {
                dnaStrand.AddRange(layerWeights[i].ToColumnMajorArray());
                dnaStrand.AddRange(layerBiases[i].ToColumnMajorArray());
            }
            return dnaStrand.ToArray();
        }
        
        /*
         * This function spaws a child neural network thats made from
         * a combination of this network and the given network b.
         * The cut off point for the joining is given as a double
         * between 0 and 1.
        */
        public DNA ApplyCrossoverWith(DNA b, double cutOffPoint)
        {
            NeuralNetwork parentB = (NeuralNetwork)b;

            int childInputLayerSize = inputLayerSize;
            List<int> childLayerSizes = new List<int>(numNonInputLayers);
            List<ActivationType> childLayerActivations = new List<ActivationType>(numNonInputLayers);
            for (var i=0; i<numNonInputLayers; i++)
            {
                childLayerSizes.Add(layerBiases[i].RowCount);
                childLayerActivations.Add(layerActivations[i].getType());
            }

            float[] childDnaValues = parentB.GetDNAStrand();

            int cutOffIndex = (int)(cutOffPoint * (double)(childDnaValues.Length-1));
            GetDNAStrand().Take(cutOffIndex).ToArray().CopyTo(childDnaValues, 0);

            return new NeuralNetwork(childInputLayerSize, childLayerSizes, childLayerActivations, childDnaValues, Rand);
        }

        /*
         * Returns the number of neurons in the layer
         * ar the given index. 0 is the first hidden layer
         * or the output layer in a no-hidden layer network.
        */
        private int LayerSize(int layer)
        {
            System.Diagnostics.Debug.Assert(layer >= 0 && layer < LayerBiases.Count());
            return LayerBiases[layer].RowCount;
        }

        /*
         * Generates a list (of size num) of random floats.
         * If isWeight is true will use random weight generation
         * for the floats. Otherwise it will use random Bias generation
        */
        private float[] GenerateFloats(int num, bool isWeight)
        {
            float[] floats = new float[num];
            for (var i = 0; i < num; i++)
            {
                if (isWeight)
                {
                    floats[i] = GenerateNewWeight();
                }
                else
                {
                    floats[i] = GenerateNewBias();
                }
            }
            return floats;
        }

        /*
         * Generates a random weight.
        */
        private float GenerateNewWeight()
        {
            return (float)((Rand.NextDouble() * 8) - 4);
        }

        /*
         * Generates a random bias.
        */
        private float GenerateNewBias()
        {
            return (float)((Rand.NextDouble() * 20) - 10);
        }
    }
}
