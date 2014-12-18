using System;
using System.Collections.Generic;
using MLP_Logic.Enums;

namespace MLP_Logic.Logic
{
    public class MLPNeuronNetwork : NeuronNetwork
    {


        public MLPNeuronNetwork(List<int> neuronsInLayer, bool isBiased, bool isUnipolar, double minWeight, double maxWeight, int inputNumber)
        {
            Layers = new List<Layer>();
            IsBiased = isBiased;
            IsUnipolar = isUnipolar;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            InputNumber = (neuronsInLayer.Count > 0) ? inputNumber : 0; //liczba wejść do neuronu w warstwie wejściowej
            OutputNumber = (neuronsInLayer.Count > 0) ? neuronsInLayer[neuronsInLayer.Count - 1] : 0;//liczba neuronów w warstwie wyjściowej

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
                    neuronsInLayer[neuronsInLayer.Count - 1], LayerType.OutputLayer);
                Layers.Add(outputLayer);
            }
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
                result = Layers[i].Calculate(scaledArguments);
                scaledArguments = result;
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
