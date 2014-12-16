using System;
using System.Collections.Generic;
using System.Linq;

namespace MLP_Logic.Logic
{
    public class Layer
    {
        public int NeuronsInPrevLayer { get; private set; }
        public int NeuronsInThisLayer { get; private set; }
        public List<Neuron> Neurons { get { return neurons; } }

        private List<Neuron> neurons;
        private bool isBiased;

        public Layer(int neuronsInPrevLayer, int neuronsInThisLayer, bool isBiased, bool isUnipolar, double minWeight, double maxWeight, Random random)
        {
            NeuronsInPrevLayer = neuronsInPrevLayer;
            NeuronsInThisLayer = neuronsInThisLayer;
            neurons = new List<Neuron>();
            this.isBiased = isBiased;

            for (int i = 0; i < neuronsInThisLayer; i++)
            {
                Neuron neuron = new Neuron(neuronsInPrevLayer + ((isBiased) ? 1 : 0), isUnipolar, minWeight, maxWeight, random);
                neurons.Add(neuron);
            }
        }

        public double[] Calculate(double[] arguments)
        {
            double[] result = new double[neurons.Count];
            double[] argumentsExtended;

            if (isBiased)
            {
                argumentsExtended = arguments.Concat<double>(new double[] { 1.0 }).ToArray<double>();
            }
            else
            {
                argumentsExtended = arguments;
            }

            for (int i = 0; i < neurons.Count; i++)
            {
                result[i] = neurons[i].Calculate(argumentsExtended);
            }

            return result;
        }
    }
}
