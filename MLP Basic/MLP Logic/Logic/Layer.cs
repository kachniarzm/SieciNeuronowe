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

        private readonly List<Neuron> neurons;
        private readonly bool isBiased;

        public Layer(int neuronsInPrevLayer, int neuronsInThisLayer, bool isBiased, bool isUnipolar, double minWeight, double maxWeight, LayerType layerType = LayerType.Undefined)
        {
            NeuronsInPrevLayer = neuronsInPrevLayer;
            NeuronsInThisLayer = neuronsInThisLayer;
            neurons = new List<Neuron>();
            this.isBiased = isBiased;

            for (int i = 0; i < neuronsInThisLayer; i++)
            {
                if (layerType == LayerType.ContextLayer)
                {
                    var neuron = new Neuron(2, isUnipolar, minWeight, maxWeight, true);
                    neurons.Add(neuron);
                }
                else
                {
                    var neuron = new Neuron(neuronsInPrevLayer + ((isBiased) ? 1 : 0), isUnipolar, minWeight, maxWeight);
                    neurons.Add(neuron);
                }
                
            }
        }

        public double[] Calculate(double[] arguments, Layer contextLayer = null)
        {
            var result = new double[neurons.Count];

            double[] argumentsExtended = isBiased ? arguments.Concat(new[] { 1.0 }).ToArray() : arguments;
            if (contextLayer != null)
            {
                foreach (var neuron in contextLayer.Neurons)
                {
                    argumentsExtended = argumentsExtended.Concat(new[] { neuron.OutputValue }).ToArray();
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
