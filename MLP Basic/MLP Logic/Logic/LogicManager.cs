using MLP_Data;
using MLP_Data.Entity;
using MLP_Data.Enums;
using MLP_Logic.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using MLP_Logic.Enums;

namespace MLP_Logic.Logic
{
    public class LogicManager
    {
        private NeuronNetwork network;

        private int iterationNumber;
        private double learningCoefficient;
        private double inertiaCoefficient;
        private double proportionalDivisionTrainingTestData;

        private double maxFunctionValue;
        private double minFunctionValue;

        private InputDataDateUnits step;
        private InputDataDateUnits windowLength;
        private InputDataDateUnits density;
        private IndexName predictionChoice;
        private List<TestCase> trainingSet;
        private List<TestCase> testSet;

        public void SetNeuronNetwork(NeuronNetworkDTO dto, int inputLength)
        {
            switch (dto.NetworkType)
            {
                case NeuronNetworkType.MLP:
                    network = new NeuronNetwork(dto.NeuronsInLayer, dto.IsBiased, dto.IsUnipolar,  -0.5, 0.5, inputLength);
                    break;
                case NeuronNetworkType.Jordan:
                    throw new NotImplementedException();
                case NeuronNetworkType.Elman:
                    throw new NotImplementedException();
                case NeuronNetworkType.Undefined:
                    throw new ArgumentException("Network structure: NeuronNetworkType is Undefined");
            }
        }

        public void SetEnviorment(EnvironmentDTO dto)
        {
            iterationNumber = dto.IterationNumber;
            learningCoefficient = dto.LearningCoefficient;
            inertiaCoefficient = dto.InertiaCoefficient;
            proportionalDivisionTrainingTestData = dto.ProportionalDivisionTrainingTestData;
            step = dto.Step;
            density = dto.Density;
            windowLength = dto.WindowLength;
            predictionChoice = dto.PredictionChoice;
        }

        private List<TestCase> SetTrainingSet(List<TestCase> data, int division)
        {
            return data.GetRange(0, division);
        }

        private List<TestCase> SetTestSet(List<TestCase> data, int division)
        {
            return data.GetRange(division, data.Count - division);
        }

        private void FindMaxAndMinInSet()
        {
            double maxParamOutput = double.MinValue;
            double minParamOutput = double.MaxValue;

            for (int i = 0; i < trainingSet.Count(); i++)
            {
                if (trainingSet[i].Output[0] > maxParamOutput)
                    maxParamOutput = trainingSet[i].Output[0];
                if (trainingSet[i].Output[0] < minParamOutput)
                    minParamOutput = trainingSet[i].Output[0];
            }
            for (int i = 0; i < testSet.Count(); i++)
            {
                if (testSet[i].Output[0] > maxParamOutput)
                    maxParamOutput = testSet[i].Output[0];
                if (testSet[i].Output[0] < minParamOutput)
                    minParamOutput = testSet[i].Output[0];
            }

            maxFunctionValue = maxParamOutput * 2;

            if (minParamOutput < 0)
                throw new ArgumentException("Index has to be positive");
            minFunctionValue = minParamOutput * 0.5;
        }

        public ResultDTO Run(NeuronNetworkDTO neuronNetworkDto)
        {
            List<TestCase> data;     
            data = CasesCreator.Create(CsvReader.GetData(predictionChoice, typeof (StockExchangeListing)),
                   windowLength, density, step).ToList();
            SetNeuronNetwork(neuronNetworkDto, data[0].Input.Count());

            var divisionSet = (int)(data.Count() * proportionalDivisionTrainingTestData / 100);
            trainingSet = SetTrainingSet(data, divisionSet);
            testSet = SetTestSet(data, divisionSet);
            FindMaxAndMinInSet();

            if (trainingSet.Count <= 0)
            {
                throw new ArgumentException(String.Format("Training set contains no items."));
            }

            var errorsPerIterations = new List<double>();
            var correctDirectionPredictionsRateInIterations = new List<double>();

            for (int currentIteration = 0; currentIteration < iterationNumber; currentIteration++)
            {
                double errorInIteration = 0;
                int correctDirectionPredictionsInIteration = 0;

                for (int currentTrainingIndex = 0; currentTrainingIndex < trainingSet.Count(); currentTrainingIndex++)
                {
                    double[] arguments = trainingSet[currentTrainingIndex].Input;
                    double[] predictedResult = trainingSet[currentTrainingIndex].Output;
                    double[] previousCaseResult = trainingSet[currentTrainingIndex].OutputInPreviousCase;

                    double[] result = network.Calculate(arguments, predictedResult,
                        learningCoefficient, inertiaCoefficient, minFunctionValue, maxFunctionValue);

                    double error = Math.Pow(result[0] - predictedResult[0], 2);

                    if ((result[0] - previousCaseResult[0]) * (predictedResult[0] - previousCaseResult[0]) > 0)
                    {
                        // Correct direction prediction
                        correctDirectionPredictionsInIteration++;
                    }

                    errorInIteration += error;
                }

                errorsPerIterations.Add(errorInIteration / trainingSet.Count());
                correctDirectionPredictionsRateInIterations.Add((double)correctDirectionPredictionsInIteration / trainingSet.Count());
            }

            ResultDTO resultDto = SetResult(network.InputNumber, network.OutputNumber);
            resultDto.ErrorsPerIterations = errorsPerIterations;
            return resultDto;
        }

        private ResultDTO SetResult(int inputParams, int outputParams)
        {
            var resultDto = new ResultDTO();

            if (inputParams > 0 && outputParams == 1)
            {
                resultDto.TrainingCaseDay = new List<double>();
                resultDto.TrainingCaseValue = new List<double>();

                for (int i = 0; i < trainingSet.Count; i++)
                {
                    resultDto.TrainingCaseDay.Add(i);
                    resultDto.TrainingCaseValue.Add(trainingSet[i].Output[0]);
                }

                resultDto.TestCaseDay = new List<double>();
                resultDto.TestCaseValue = new List<double>();

                for (int i = 0; i < testSet.Count; i++)
                {
                    resultDto.TestCaseDay.Add(trainingSet.Count + i);
                    resultDto.TestCaseValue.Add(testSet[i].Output[0]);
                }

                resultDto.NetworkPredictionCaseDay = new List<double>();
                resultDto.NetworkPredictedValue = new List<double>();

                for (int i = 0; i < trainingSet.Count; i++)
                {
                    resultDto.NetworkPredictionCaseDay.Add(i);
                    resultDto.NetworkPredictedValue.Add((network.Calculate(trainingSet[i].Input, null, learningCoefficient, inertiaCoefficient, minFunctionValue, maxFunctionValue))[0]);
                }

                for (int i = 0; i < testSet.Count; i++)
                {
                    resultDto.NetworkPredictionCaseDay.Add(trainingSet.Count + i);
                    resultDto.NetworkPredictedValue.Add((network.Calculate(testSet[i].Input, null, learningCoefficient, inertiaCoefficient, minFunctionValue, maxFunctionValue))[0]);
                }

            }
            return resultDto;
        }
    }
}
