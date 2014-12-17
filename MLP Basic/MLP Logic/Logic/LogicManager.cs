using MLP_Data;
using MLP_Data.Entity;
using MLP_Data.Enums;
using MLP_Logic.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLP_Logic.Enums;

namespace MLP_Logic.Logic
{
    public class LogicManager
    {
        private MLPNeuronNetwork network;

        private int iterationNumber;
        private double learningCoefficient;
        private double inertiaCoefficient;
        private double proportionalDivisionTrainingTestData;

        private List<double> maxInputValues;
        private List<double> minInputValues;
        private List<double> maxOutputValues;
        private List<double> minOutputValues;

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
                    network = new MLPNeuronNetwork(dto.NeuronsInLayer, dto.IsBiased, dto.IsUnipolar,  -0.5, 0.5, inputLength);
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
            maxInputValues = new List<double>();
            minInputValues = new List<double>();

            for (int j = 0; j < trainingSet[0].Input.Length; j++)
            {
                double currentMax = double.MinValue;
                double currentMin = double.MaxValue;

                for (int i = 0; i < trainingSet.Count(); i++)
                {
                    if (trainingSet[i].Input[j] > currentMax)
                        currentMax = trainingSet[i].Input[j];
                    if (trainingSet[i].Input[j] < currentMin)
                        currentMin = trainingSet[i].Input[j];
                }

                for (int i = 0; i < testSet.Count(); i++)
                {
                    if (testSet[i].Input[j] > currentMax)
                        currentMax = testSet[i].Input[j];
                    if (testSet[i].Input[j] < currentMin)
                        currentMin = testSet[i].Input[j];
                }

                if (currentMin >= currentMax)
                    currentMax += 1;

                maxInputValues.Add(currentMax);
                minInputValues.Add(currentMin); 
            }

            maxOutputValues = new List<double>();
            minOutputValues = new List<double>();

            for (int j = 0; j < trainingSet[0].Output.Length; j++)
            {
                double currentMax = double.MinValue;
                double currentMin = double.MaxValue;

                for (int i = 0; i < trainingSet.Count(); i++)
                {
                    if (trainingSet[i].Output[j] > currentMax)
                        currentMax = trainingSet[i].Output[j];
                    if (trainingSet[i].Output[j] < currentMin)
                        currentMin = trainingSet[i].Output[j];
                }

                for (int i = 0; i < testSet.Count(); i++)
                {
                    if (testSet[i].Output[j] > currentMax)
                        currentMax = testSet[i].Output[j];
                    if (testSet[i].Output[j] < currentMin)
                        currentMin = testSet[i].Output[j];
                }

                if (currentMin >= currentMax)
                    currentMax += 1;

                maxOutputValues.Add(currentMax);
                minOutputValues.Add(currentMin);
            }
        }

        public async Task<ResultDTO> Run(NeuronNetworkDTO neuronNetworkDto)
        {
            return await Task.Run(() =>
            {
                List<TestCase> data;
                data =
                    CasesCreator.Create(
                        CsvReader.GetData(predictionChoice, CasesCreator.GetTypeByPredictionChoice(predictionChoice)),
                        windowLength, density, step).ToList();
                SetNeuronNetwork(neuronNetworkDto, data[0].Input.Count());

                var divisionSet = (int) (data.Count()*proportionalDivisionTrainingTestData/100);
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

                    for (int currentTrainingIndex = 0;
                        currentTrainingIndex < trainingSet.Count();
                        currentTrainingIndex++)
                    {
                        double[] arguments = trainingSet[currentTrainingIndex].Input;
                        double[] predictedResult = trainingSet[currentTrainingIndex].Output;
                        double[] previousCaseResult = trainingSet[currentTrainingIndex].OutputInPreviousCase;

                        double[] result = network.Calculate(
                            arguments,
                            maxInputValues,
                            minInputValues,
                            maxOutputValues,
                            minOutputValues,
                            predictedResult,
                            learningCoefficient,
                            inertiaCoefficient);

                        double error = Math.Pow(result[0] - predictedResult[0], 2);

                        if ((result[0] - previousCaseResult[0])*(predictedResult[0] - previousCaseResult[0]) > 0)
                        {
                            // Correct direction prediction
                            correctDirectionPredictionsInIteration++;
                        }

                        errorInIteration += error;
                    }

                    errorsPerIterations.Add(errorInIteration/trainingSet.Count());
                    correctDirectionPredictionsRateInIterations.Add((double) correctDirectionPredictionsInIteration/
                                                                    trainingSet.Count());
                }

                ResultDTO resultDto = SetResult(network.InputNumber, network.OutputNumber);
                resultDto.ErrorsPerIterations = errorsPerIterations;
                SetResultCorrectDirectionPredictionsRate(resultDto, correctDirectionPredictionsRateInIterations);
                return resultDto;
            });
        }

        private void SetResultCorrectDirectionPredictionsRate(ResultDTO result, List<double> factors)
        {
            result.MaxCorrectDirectionPredictionsRate = factors.Max();
            result.MinCorrectDirectionPredictionsRate = factors.Min();
            result.AverageCorrectDirectionPredictionsRate = factors.Average();
            result.FirstCorrectDirectionPredictionsRate = factors.First();
            result.LastCorrectDirectionPredictionsRate = factors.Last();
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
                    resultDto.NetworkPredictedValue.Add((network.Calculate(
                        trainingSet[i].Input,
                        maxInputValues,
                        minInputValues,
                        maxOutputValues,
                        minOutputValues,
                        null,
                        learningCoefficient,
                        inertiaCoefficient))[0]);
                }

                for (int i = 0; i < testSet.Count; i++)
                {
                    resultDto.NetworkPredictionCaseDay.Add(trainingSet.Count + i);
                    resultDto.NetworkPredictedValue.Add((network.Calculate(
                        testSet[i].Input,
                        maxInputValues,
                        minInputValues,
                        maxOutputValues,
                        minOutputValues, 
                        null,
                        learningCoefficient,
                        inertiaCoefficient))[0]);
                }

            }
            return resultDto;
        }
    }
}
