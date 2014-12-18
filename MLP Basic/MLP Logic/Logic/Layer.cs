using System;
using System.Collections.Generic;
using System.Linq;
using MLP_Logic.Enums;

namespace MLP_Logic.Logic
{
    public class Layer
    {
        public int NeuronsInPrevLayer { get; private set; }
        public int NeuronsInThisLayer { get; private set; }
        public List<Neuron> Neurons { get { return neurons; } }

        private List<Neuron> neurons;
        private bool isBiased;
        private LayerType layerType; 

        public Layer(int neuronsInPrevLayer, int neuronsInThisLayer, bool isBiased, bool isUnipolar, double minWeight, double maxWeight, LayerType layerType = LayerType.Undefined)
        {
            NeuronsInPrevLayer = neuronsInPrevLayer;
            NeuronsInThisLayer = neuronsInThisLayer;
            neurons = new List<Neuron>();
            this.isBiased = isBiased;
            this.layerType = layerType;

            for (int i = 0; i < neuronsInThisLayer; i++)
            {
                if (layerType == LayerType.ContextLayer)
                {
                    Neuron neuron = new Neuron(2, isUnipolar, minWeight, maxWeight, true);
                    neurons.Add(neuron);
                }
                else
                {
                    Neuron neuron = new Neuron(neuronsInPrevLayer + ((isBiased) ? 1 : 0), isUnipolar, minWeight, maxWeight);
                    neurons.Add(neuron);
                }
                
            }
        }

        public double[] Calculate(double[] arguments, Layer contextLayer = null)
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
            if (contextLayer != null)
            {
                foreach (var neuron in contextLayer.Neurons)
                {
                    argumentsExtended = argumentsExtended.Concat<double>(new double[] { neuron.OutputValue }).ToArray<double>();
                }
            }


            for (int i = 0; i < neurons.Count; i++)
            {
                result[i] = neurons[i].Calculate(argumentsExtended);
            }

            return result;
        }
    }
}
