using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Logic.Logic
{
    public class NeuronNetwork
    {
        public int InputNumber { get; private set; }
        public int OutputNumber { get; private set; }
        public List<Layer> Layers { get; private set; }
        public bool IsBiased { get; private set; }
        public bool IsUnipolar { get; private set; }
        public double MinWeight { get; private set; }
        public double MaxWeight { get; private set; }
        private Random Random { get; set; }


        public NeuronNetwork(List<int> neuronsInLayer, bool isBiased, bool isUnipolar, double minWeight, double maxWeight, int inputNumber)
        {
            Layers = new List<Layer>();
            IsBiased = isBiased;
            IsUnipolar = isUnipolar;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            Random = new Random(Guid.NewGuid().GetHashCode());
            InputNumber = (neuronsInLayer.Count > 0) ? inputNumber : 0; //liczba wejść do neuronu w warstwie wejściowej
            OutputNumber = (neuronsInLayer.Count > 0) ? neuronsInLayer[neuronsInLayer.Count - 1] : 0;//liczba neuronów w warstwie wyjściowej

            //TWORZENIE WARSTWY WEJŚCIOWEJ
            Layer inputLayer = CreateLayer(InputNumber,neuronsInLayer[0]);
            Layers.Add(inputLayer);
            //-------------------------
            //TWORZENIE WARSTW UKRYTYCH i WYJŚCIOWEJ
            for (int i = 1; i < neuronsInLayer.Count; i++)
            {
                Layer layer = CreateLayer(neuronsInLayer[i - 1], neuronsInLayer[i]);
                Layers.Add(layer);
            }

        }

        private Layer CreateLayer(int neuronsInPrevLayer, int neuronsInThisLayer)
        {
            Layer Layer = new Layer(neuronsInPrevLayer, neuronsInThisLayer, IsBiased, IsUnipolar, MinWeight, MaxWeight, Random);
            return Layer;
        }

        public double[] Calculate(double[] arguments, double[] predictedResult = null, double learningCoefficient = 0.1, double inertia = 0.5,
            double minFunctionValue = -1, double maxFunctionValue = 1)
        {
            double[] result = null;
            var scaledArguments = ScaleFunctionValue(arguments, minFunctionValue, maxFunctionValue);

            for (int i = 0; i < Layers.Count; i++)
            {
                result = Layers[i].Calculate(scaledArguments);
                scaledArguments = result;
            }
            if (predictedResult != null)
            {
                double[] scaledPredictedValue = ScaleFunctionValue(predictedResult, minFunctionValue, maxFunctionValue);
                BackPropagationLearningMethod.Run(Layers, scaledPredictedValue, learningCoefficient, inertia);
            }

            double[] rescaledResult = RescaleFunctionValue(result, minFunctionValue, maxFunctionValue);

            return rescaledResult;
        }

        private double[] ScaleFunctionValue(double[] value, double minFunctionValue, double maxFunctionValue)
        {
            double newMin = IsUnipolar ? 0 : -1;
            double newMax = 1;

            double[] scaledResult = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                scaledResult[i] = ((value[i] - minFunctionValue) / (maxFunctionValue - minFunctionValue)) * (newMax - newMin) + newMin;
            }

            return scaledResult;
        }

        private double[] RescaleFunctionValue(double[] value, double minFunctionValue, double maxFunctionValue)
        {
            double oldMin = IsUnipolar ? 0 : -1;
            double oldMax = 1;

            double[] rescaledResult = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                rescaledResult[i] = ((value[i] - oldMin) / (oldMax - oldMin)) * (maxFunctionValue - minFunctionValue) + minFunctionValue;
            }

            return rescaledResult;
        }
    }
}
