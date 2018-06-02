using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.NeuralNetworkEngine.ActivationFunctions
{
    /*
     * This enum holds all implemented activation
     * function types.
    */
    public enum ActivationType
    {
        Linear,
        Sigmoid,
        Softmax,
        Relu
    }

    /*
     * Base class from which all activation
     * functions will inherid.
    */
    public class Activation
    {
        protected ActivationType Type { get; set; }
        public ActivationType getType() { return this.Type; }

        public Activation() {
            this.Type = ActivationType.Linear;
        }

        /*
         * Static Method which returns an instance of the
         * Activation Function class corresponding to the 
         * given ActivationType
        */
        public static Activation CreateActivation(ActivationType t)
        {
            switch (t)
            {
                case ActivationType.Linear:
                    return new Activation();
                case ActivationType.Sigmoid:
                    return new SigmoidActivation();
                case ActivationType.Relu:
                    return new ReluActivation();
                case ActivationType.Softmax:
                    return new ReluActivation();
                default:
                    throw new ArgumentException("Un recognised Activation Type");
            }
        }

        // Use activation function on all values in given float array.
        public virtual float[] ActivateAll(float[] toActivate)
        {
            float[] activatedValues = new float[toActivate.Length];
            for (var i=0; i<toActivate.Length; i++)
            {
                activatedValues[i] = ActivateOne(toActivate[i]);
            }
            return activatedValues;
        }

        // Use activation function on one float value.
        // Simple linear activation here. input is output.
        public virtual float ActivateOne(float toActivate)
        {
            return toActivate;
        }
    }
}
