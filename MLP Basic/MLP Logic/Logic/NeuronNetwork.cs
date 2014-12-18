﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLP_Logic.Enums;

namespace MLP_Logic.Logic
{
    public abstract class NeuronNetwork : INeuronNetwork
    {
        public int InputNumber { get; protected set; }
        public int OutputNumber { get; protected set; }
        public List<Layer> Layers { get; protected set; }
        public bool IsBiased { get; protected set; }
        public bool IsUnipolar { get; protected set; }
        public double MinWeight { get; protected set; }
        public double MaxWeight { get; protected set; }

        protected Layer CreateLayer(int neuronsInPrevLayer, int neuronsInThisLayer, LayerType layerType = LayerType.Undefined)
        {
            Layer layer;
            if (layerType == LayerType.ContextLayer)
            {
                layer = new Layer(neuronsInPrevLayer, neuronsInThisLayer, false, IsUnipolar, MinWeight, MaxWeight, layerType);
            }
            else
            {
                layer = new Layer(neuronsInPrevLayer, neuronsInThisLayer, IsBiased, IsUnipolar, MinWeight, MaxWeight, layerType);
            }
            return layer;
        }

        protected double[] ScaleFunctionValue(double[] value, List<double> minFunctionValue, List<double> maxFunctionValue)
        {
            double newMin = IsUnipolar ? 0 : -1;
            double newMax = 1;
            double[] scaledResult = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                scaledResult[i] = ((value[i] - minFunctionValue[i]) / (maxFunctionValue[i] - minFunctionValue[i])) * (newMax - newMin) + newMin;
            }
            return scaledResult;
        }

        protected double[] RescaleFunctionValue(double[] value, List<double> minFunctionValue, List<double> maxFunctionValue)
        {
            double oldMin = IsUnipolar ? 0 : -1;
            double oldMax = 1;
            double[] rescaledResult = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                rescaledResult[i] = ((value[i] - oldMin) / (oldMax - oldMin)) * (maxFunctionValue[i] - minFunctionValue[i]) + minFunctionValue[i];
            }
            return rescaledResult;
        }

        public virtual double[] Calculate(double[] arguments, List<double> maxInputValues, List<double> minInputValues, List<double> maxOutputValues,
            List<double> minOutputValues, double[] predictedResult = null, double learningCoefficient = 0.1, double inertia = 0.5)
        {
            throw new NotImplementedException();
        }
    }
}
