using System.Collections.Generic;
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

            //TWORZENIE WARSTWY WEJŚCIOWEJ
            Layer inputLayer = CreateLayer(InputNumber + neuronsInLayer[0],neuronsInLayer[0], LayerType.InputLayer);
            Layers.Add(inputLayer);
            //TWORZENIE JESZCZE JEDNEJ WARSTWY KONTEKSTOWEJ
            ContextLayers.Add(CreateLayer(neuronsInLayer[0], neuronsInLayer[0], LayerType.ContextLayer));
            //-------------------------

            //ODWRÓCENIE KOLEJNOŚCI, ABY NA POCZĄTKU BYŁA WARSTWA WEJŚCIOWA A NA KOŃCU WYJŚCIOWA
            Layers.Reverse();
            ContextLayers.Reverse();
        }
    }
}
