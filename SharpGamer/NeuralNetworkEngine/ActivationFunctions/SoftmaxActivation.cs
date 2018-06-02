using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.NeuralNetworkEngine.ActivationFunctions
{
    /*
     * This class performs Softmax activation. Softmax activation
     * works on vectors only by default because it is used mainly
     * for classification.
    */
    class SoftmaxActivation : Activation
    {
        public SoftmaxActivation()
        {
            this.Type = ActivationType.Softmax;
        }

        public override float[] ActivateAll(float[] toActivate)
        {
            float[] activated = new float[toActivate.Length];
            toActivate.CopyTo(activated, 0);

            float sum = 0;
            float maxInput = activated.Max();
            for (var i = 0; i < activated.Length; i++)
            {
                var minusMax = activated[i] - maxInput; // minus maxInput to avoid overflows
                activated[i] = (float)Math.Exp(minusMax);
                sum += activated[i];
            }

            for (var i = 0; i < activated.Length; i++)
            {
                activated[i] = activated[i] / sum;
            }

            return activated;
        }

        // Softmax is by definition not used for single values. The Softmax
        // activation for a layer of neurons depends on the input for every neuron
        // and therfor should never be used for single neurons.
        public override float ActivateOne(float toActivate)
        {
            throw new NotImplementedException("ActivateOne called on Softmax activation. Somewhing is wrong.");
        }
    }
}
