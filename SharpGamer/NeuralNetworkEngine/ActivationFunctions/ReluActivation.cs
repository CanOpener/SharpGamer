using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.NeuralNetworkEngine.ActivationFunctions
{
    /*
     * RELU is an activation function which truncates all
     * values below 0 to 0 and all values above stay the same.
     * Seems to work better than sigmoid and tanh for user
     * guided learning but its worth a try with genetic learning.
     * https://en.wikipedia.org/wiki/Rectifier_(neural_networks)
    */
    class ReluActivation : Activation
    {
        public ReluActivation()
        {
            this.Type = ActivationType.Softmax;
        }

        public override float ActivateOne(float toActivate)
        {
            return Math.Max(0f, toActivate);
        }
    }
}
