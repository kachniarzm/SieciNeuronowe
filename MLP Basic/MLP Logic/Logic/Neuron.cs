using System;
using System.Linq;

namespace MLP_Logic.Logic
{
    public class Neuron
    {
        public double this[int i] { get { return weights[i]; } set { weights[i] = value; } }
        public int InputNumbers { get { return weights.Count(); } }
        public bool IsUnipolar { get; set; }
        public bool IsContextNeuron { get; set; }
        public double OutputValue { get; set; }
        public double Error { get; set; }
        public double DeltaError { get; set; }
        public double[] Arguments { get; set; }
        
        private readonly double[] weights;

        private void SetWeights(double minWeight, double maxWeight)
        {
            if (weights.Count() == 0)
            {
                throw new ArgumentException("weights.Count() == 0");
            }
            for (int i = 0; i < weights.Count(); i++)
            {
                weights[i] = RandomGenerator.Random.NextDouble() * (maxWeight - minWeight) + minWeight;
            }
        }

        public Neuron(int inputNumbers, bool isUnipolar, double minWeight, double maxWeight, bool isContextLayer = false)
        {
            weights = new double[inputNumbers];
            OutputValue = 0;
            if (isContextLayer)
            {
                weights[0] = 1;
                weights[1] = 0.5;
            }
            else
            {
                SetWeights(minWeight, maxWeight);
            }
            IsUnipolar = isUnipolar;
        }

        public void CalculateError(double? predictedValue = null, int neuronNumberInLayer = -1, Layer nextLayer = null)
        {
            double newError = 0;
            if (predictedValue != null) //Neuron is in output layer     
            {
                newError = (double)predictedValue - OutputValue;
                //this.Error = Math.Pow((double)predictedValue - this.OutputValue, 2);
            }
            else
            {
                if (neuronNumberInLayer < 0 || nextLayer == null) throw new ArgumentException("Wrong parameters in neuron.CalculateError");

                newError += nextLayer.Neurons.Sum(neuronInNextLayer => neuronInNextLayer.Error*neuronInNextLayer[neuronNumberInLayer]);
            }
            DeltaError = newError - Error;
            Error = newError;
        }

        public void ModifyWeight(double learningCoefficient, double inertia)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                if (IsUnipolar)
                    this[i] += learningCoefficient * Error *
                               SigmoidFunctions.CalcUnipolarFunctionDerivative(OutputValue) 
                               * Arguments[i] + inertia * DeltaError;
                else
                    this[i] += learningCoefficient * Error *
                               SigmoidFunctions.CalcBipolarFunctionDerivative(OutputValue) 
                               * Arguments[i] + inertia * DeltaError;
            }
        }

        public double Calculate(double[] arguments)
        {
            double sum = 0;
            Arguments = arguments;
            for (int i = 0; i < weights.Length; i++)
            {
                sum += arguments[i] * weights[i];
            }
            OutputValue = (IsUnipolar) ? SigmoidFunctions.CalcUnipolarFunction(sum) : SigmoidFunctions.CalcBipolarFunction(sum);
            return OutputValue;
        }
    }
}
