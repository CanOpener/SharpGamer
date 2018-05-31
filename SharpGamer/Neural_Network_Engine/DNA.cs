using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.Neural_Network_Engine
{
    public interface DNA
    {
        int getScore();
        int getGenomeSize();
        DNA applyCrossOver(DNA b);
        void mutateAt(int index, double step, bool positive);
        DNA clone();
    }
}
