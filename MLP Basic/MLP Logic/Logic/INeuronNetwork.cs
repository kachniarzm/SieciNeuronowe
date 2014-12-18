using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Logic.Logic
{
    interface INeuronNetwork
    {
        double[] Calculate(
            double[] arguments,
            List<double> maxInputValues,
            List<double> minInputValues,
            List<double> maxOutputValues,
            List<double> minOutputValues,
            double[] predictedResult = null,
            double learningCoefficient = 0.1,
            double inertia = 0.5);
    }
}
