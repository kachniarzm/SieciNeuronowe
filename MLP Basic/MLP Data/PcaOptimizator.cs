using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace MLP_Data
{
    public static class PcaOptimizator
    {
        public static void Optimize(List<TestCase> testCases, int inputVectorMaxLenght)
        {
            int n = testCases[0].Input.Length;

            if (inputVectorMaxLenght >= n)
                return;

            Matrix<double> autocerelationTrainingPatternMatrix = Matrix<double>.Build.Dense(n, n);

            foreach (TestCase t in testCases)
            {
                Vector<double> x = Vector.Build.DenseOfEnumerable(t.Input);
                Matrix<double> newMatrix = Matrix<double>.Build.Dense(n, n);
                x.OuterProduct(x, newMatrix);
                autocerelationTrainingPatternMatrix = autocerelationTrainingPatternMatrix.Add(newMatrix);
            }

            autocerelationTrainingPatternMatrix.Divide(n);
            var eigenvaluesContainer = autocerelationTrainingPatternMatrix.Evd();
            Matrix<double> transformationMatrix = eigenvaluesContainer.EigenVectors.SubMatrix(0, n, n - inputVectorMaxLenght, inputVectorMaxLenght);
            
            foreach (TestCase t in testCases)
            {
                Vector<double> x = Vector.Build.DenseOfEnumerable(t.Input);
                t.Input = transformationMatrix.LeftMultiply(x).ToArray();
            }

        }
    }
}
