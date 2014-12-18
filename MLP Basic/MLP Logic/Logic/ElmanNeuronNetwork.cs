using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using MLP_Logic.Enums;

namespace MLP_Logic.Logic
{
    /// <summary>
    /// każda wasrstwa ukryta ma swoją warstwę kontekstową
    /// wyjścia z warstwy kontekstowej są wejściem do warstwy ukrytej
    /// </summary>
    class ElmanNeuronNetwork : NeuronNetwork
    {
        public List<Layer> ContextLayers { get; private set; }
        private double[] contextNeuronArguments = new double[2];

        public ElmanNeuronNetwork(List<int> neuronsInLayer, bool isBiased, bool isUnipolar, double minWeight, double maxWeight, int inputNumber)
        {
            Layers = new List<Layer>();
            ContextLayers = new List<Layer>();
            IsBiased = isBiased;
            IsUnipolar = isUnipolar;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            OutputNumber = (neuronsInLayer.Count > 0) ? neuronsInLayer[neuronsInLayer.Count - 1] : 0;//liczba neuronów w warstwie wyjściowej
            InputNumber = (neuronsInLayer.Count > 0) ? inputNumber : 0; //liczba wejść do neuronu w warstwie wejściowej 
            
            if (neuronsInLayer.Count - 2 >= 0)
            {
                //TWORZENIE WARSTWY WYJŚCIOWEJ
                Layer outputLayer = CreateLayer(neuronsInLayer[neuronsInLayer.Count - 2],
                    neuronsInLayer[neuronsInLayer.Count - 1], LayerType.OutputLayer);
                Layers.Add(outputLayer);

                for (int i = neuronsInLayer.Count - 2; i >=1; i--)
                {
                    //TWORZENIE WARSTW UKRYTYCH
                    Layer layer = CreateLayer(neuronsInLayer[i - 1] + neuronsInLayer[i], neuronsInLayer[i], LayerType.HiddenLayer);
                    Layers.Add(layer);
                    //TWORZENIE WARSTWY KONTEKSTOWEJ
                    ContextLayers.Add(CreateLayer(neuronsInLayer[i], neuronsInLayer[i], LayerType.ContextLayer));
                }
            }

            //TWORZENIE PIERWSZEJ WARSTWY (wejściowa, ale jest to jednocześnie pierwsza warstwa ukryta)
            Layer inputLayer = CreateLayer(InputNumber + neuronsInLayer[0],neuronsInLayer[0], LayerType.InputLayer);
            Layers.Add(inputLayer);
            //TWORZENIE PIERWSZEJ WARSTWY KONTEKSTOWEJ
            ContextLayers.Add(CreateLayer(neuronsInLayer[0], neuronsInLayer[0], LayerType.ContextLayer));
            //-------------------------
            //ODWRÓCENIE KOLEJNOŚCI, ABY NA POCZĄTKU BYŁA WARSTWA WEJŚCIOWA A NA KOŃCU WYJŚCIOWA
            Layers.Reverse();
            ContextLayers.Reverse();
        }

        public override double[] Calculate(
         double[] arguments,
         List<double> maxInputValues,
         List<double> minInputValues,
         List<double> maxOutputValues,
         List<double> minOutputValues,
         double[] predictedResult = null,
         double learningCoefficient = 0.1,
         double inertia = 0.5)
        {
            double[] result = null;
            var scaledArguments = ScaleFunctionValue(arguments, minInputValues, maxInputValues);

            for (int i = 0; i < Layers.Count; i++)
            {
                if (i < ContextLayers.Count)
                    result = Layers[i].Calculate(scaledArguments, ContextLayers[i]);
                else result = Layers[i].Calculate(scaledArguments);
                scaledArguments = result;
            }
            for (int i = 0; i < ContextLayers.Count; i++)
            {
                for (int j = 0; j < ContextLayers[i].Neurons.Count; j++)
                {
                    contextNeuronArguments[0] = Layers[i].Neurons.ElementAt(j).OutputValue;
                    contextNeuronArguments[1] = ContextLayers[i].Neurons.ElementAt(j).OutputValue;
                    ContextLayers[i].Neurons.ElementAt(j).Calculate(contextNeuronArguments);
                }
            }

            if (predictedResult != null)
            {
                double[] scaledPredictedValue = ScaleFunctionValue(predictedResult, minOutputValues, maxOutputValues);
                BackPropagationLearningMethod.Run(Layers, scaledPredictedValue, learningCoefficient, inertia);
            }

            double[] rescaledResult = RescaleFunctionValue(result, minOutputValues, maxOutputValues);

            return rescaledResult;
        }

    }
}
