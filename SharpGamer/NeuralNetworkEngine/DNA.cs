using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.NeuralNetworkEngine
{
    public interface DNA
    {
        String Id { get; }
        float Diversity { get; set; }
        int Score { get; set; }
        int GenomeSize { get; }
        
        DNA Clone();
        DNA ApplyCrossoverWith(DNA b, double cutOffPoint);
        void MutateGeneAtIndex(int index, double step, bool positive);
    }
}
