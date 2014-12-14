using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Logic.Logic
{
    public static class SigmoidFunctions
    {
        public static double CalcUnipolarFunction(double x)
        {
            return (1.0 / (1.0 + Math.Pow(Math.E, -x)));
        }

        public static double CalcUnipolarFunctionDerivative(double x)
        {
            return x * (1 - x);
        }

        public static double CalcBipolarFunction(double x)
        {
            return Math.Tanh(x);
        }

        public static double CalcBipolarFunctionDerivative(double x)
        {
            return 1 - Math.Pow(x, 2);
        }


    }
}
