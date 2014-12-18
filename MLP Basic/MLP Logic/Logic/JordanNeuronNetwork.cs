using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLP_Logic.Enums;

namespace MLP_Logic.Logic
{
    /// <summary>
    /// SIEC JORDANA
    /// - warstwa kontekstowa ma tyle węzłów ile węzłów jest w warstwie wyjściowej
    /// - dane z warstwy wyjściowej do warstwy kontekstowej docierają z wagą 1 (zawsze)
    /// - dane z warstwy wyjściowej podawane są na wejście sieci (czyli dodatkowy neuron wejściowy w naszym przypadku jest potrzebny)
    /// - neurony w warstwie kontekstowej z wagą 0.5 przekazują sobie rekurencyjnie poprzednio posiadaną wartość
    /// - istnieje tylko jedna warstwa kontekstowa
    /// </summary>
    class JordanNeuronNetwork : NeuronNetwork
    {
        public List<Layer> ContextLayer { get; private set; }
        private double[] contextNeuronArguments = new double[2];

        public JordanNeuronNetwork(List<int> neuronsInLayer, bool isBiased, bool isUnipolar, double minWeight, double maxWeight, int inputNumber)
        {
            Layers = new List<Layer>();
            ContextLayer = new List<Layer>();
            IsBiased = isBiased;
            IsUnipolar = isUnipolar;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            OutputNumber = (neuronsInLayer.Count > 0) ? neuronsInLayer[neuronsInLayer.Count - 1] : 0;//liczba neuronów w warstwie wyjściowej
            InputNumber = (neuronsInLayer.Count > 0) ? inputNumber + OutputNumber : 0; //liczba wejść do neuronu w warstwie wejściowej + OutputNumber wynikający z warstwy kontekstowej 

            //TWORZENIE WARSTWY WEJŚCIOWEJ
            Layer inputLayer = CreateLayer(InputNumber,neuronsInLayer[0], LayerType.InputLayer);
            Layers.Add(inputLayer);
            //-------------------------
            //TWORZENIE WARSTW UKRYTYCH i WYJŚCIOWEJ
            for (int i = 1; i < neuronsInLayer.Count-1; i++)
            {
                Layer layer = CreateLayer(neuronsInLayer[i - 1], neuronsInLayer[i], LayerType.HiddenLayer);
                Layers.Add(layer);
            }
            if (neuronsInLayer.Count - 2 >= 0)
            {
                Layer outputLayer = CreateLayer(neuronsInLayer[neuronsInLayer.Count - 2],
                    neuronsInLayer[neuronsInLayer.Count - 1], LayerType.HiddenLayer);
                Layers.Add(outputLayer);
            }
            //-------------------------
            //TWORZENIE WARSTWY KONTEKSTOWEJ
            ContextLayer.Add(CreateLayer(neuronsInLayer[neuronsInLayer.Count - 1], neuronsInLayer[neuronsInLayer.Count - 1], LayerType.ContextLayer)); 
            
            //-------------------------
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
                result = Layers[i].Calculate(scaledArguments,ContextLayer[0]);
                scaledArguments = result;
            }
             for (int i=0; i< ContextLayer[0].Neurons.Count;i++)
             {
                 contextNeuronArguments[0] = Layers[Layers.Count - 1].Neurons.ElementAt(i).OutputValue;
                 contextNeuronArguments[1] = ContextLayer[0].Neurons.ElementAt(i).OutputValue;
                 ContextLayer[0].Neurons.ElementAt(i).Calculate(contextNeuronArguments);
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
