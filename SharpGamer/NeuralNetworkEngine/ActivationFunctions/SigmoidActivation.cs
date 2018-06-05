using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.NeuralNetworkEngine.ActivationFunctions
{
    /*
     * This class performs Sigmoid activation.
     * Classical activation functin used a lot prior
     * to the discovery of the practicality of the
     * Relu activation function
    */
    class SigmoidActivation : Activation
    {
        public SigmoidActivation()
        {
            this.Type = ActivationType.Sigmoid;
        }

        // Perform Sigmoid activation on one float value
        public override float ActivateOne(float toActivate)
        {
            return (float)MathNet.Numerics.SpecialFunctions.Logistic((double)toActivate);
        }
    }
}
