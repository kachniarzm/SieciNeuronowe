using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Logic.Logic
{
    public static class BackPropagationLearningMethod
    {
        public static void Run(List<Layer> layers, double[] predictedValue, double learningCoefficient, double inertia)
        {
            //liczenie błędu
            for (int i = layers.Count() - 1; i >= 0; i--)
            {
                if (i == layers.Count() - 1) //if last layer
                {
                    for (int j = 0; j < layers[i].Neurons.Count; j++ )
                    {
                        var neuron = layers[i].Neurons[j];
                        neuron.CalculateError(predictedValue[j], layers[i].Neurons.FindIndex(x => x == neuron), null);
                    }
                }
                else
                {
                    foreach (Neuron neuron in layers[i].Neurons)
                    {
                        neuron.CalculateError(null, layers[i].Neurons.FindIndex(x => x == neuron), layers[i + 1]);
                    }
                }
            }
             //Modyfikacja wag
            for (int i = layers.Count() - 1; i >= 0; i--)
            {
                if (i == layers.Count() - 1)
                {
                    foreach (Neuron neuron in layers[i].Neurons)
                    {
                        neuron.ModifyWeight(learningCoefficient, inertia);
                    }
                }
                else
                {
                    foreach (Neuron neuron in layers[i].Neurons)
                    {
                        neuron.ModifyWeight(learningCoefficient, inertia);
                    }
                }
            }
        }
    }
}
