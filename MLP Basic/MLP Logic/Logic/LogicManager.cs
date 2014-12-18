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
        private NeuronNetwork network;

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
        private List<TestCase> validationSet;
        private List<TestCase> testSet;

        public void SetNeuronNetwork(NeuronNetworkDTO dto, int inputLength)
        {
            switch (dto.NetworkType)
            {
                case NeuronNetworkType.MLP:
                    network = new MLPNeuronNetwork(dto.NeuronsInLayer, dto.IsBiased, dto.IsUnipolar, -0.5, 0.5, inputLength);
                    break;
                case NeuronNetworkType.Jordan:
                    network = new JordanNeuronNetwork(dto.NeuronsInLayer, dto.IsBiased, dto.IsUnipolar, -0.5, 0.5, inputLength);
                    break;
                case NeuronNetworkType.Elman:
                    network = new ElmanNeuronNetwork(dto.NeuronsInLayer, dto.IsBiased, dto.IsUnipolar, -0.5, 0.5, inputLength);
                    break;
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

        private List<TestCase> SetTrainingSet(List<TestCase> data, int count)
        {
            return data.GetRange(0, count);
        }

        private List<TestCase> SetValidationSet(List<TestCase> data, int count)
        {
            return data.GetRange(data.Count - count, count);
        }

        private List<TestCase> SetTestSet(List<TestCase> data, int count, double percentOfSet)
        {
            int testSetCount = (int)(percentOfSet * count);
            return data.GetRange(data.Count - count, testSetCount);
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
                for (int i = 0; i < validationSet.Count(); i++)
                {
                    if (validationSet[i].Input[j] > currentMax)
                        currentMax = validationSet[i].Input[j];
                    if (validationSet[i].Input[j] < currentMin)
                        currentMin = validationSet[i].Input[j];
                }
                if (currentMin >= currentMax)
                    currentMax += 1;

                currentMin -= (currentMax - currentMin) / 5;
                currentMax += (currentMax - currentMin) / 5;

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

        public async Task<ResultDTO> Run(NeuronNetworkDTO neuronNetworkDto, IProgress<int> progressFunction)
        {
            return await Task.Run(() =>
            {
                List<TestCase> data = CasesCreator.Create(
                    CsvReader.GetData(predictionChoice, CasesCreator.GetTypeByPredictionChoice(predictionChoice)),
                    windowLength, density, step).ToList();
                SetNeuronNetwork(neuronNetworkDto, data[0].Input.Count());

                var percentValidationSet = 0.1;
                var elementsInTrainingSet = (int)(data.Count() * proportionalDivisionTrainingTestData / 100);
                var elementsInTestSet = data.Count - elementsInTrainingSet;
                var elementsInValidationSet = (int)(elementsInTestSet * percentValidationSet);
                trainingSet = SetTrainingSet(data, elementsInTrainingSet);
                testSet = SetTestSet(data, elementsInTestSet, 1 - percentValidationSet);
                validationSet = SetValidationSet(data, elementsInValidationSet);

                FindMaxAndMinInSet();

                if (trainingSet.Count <= 0)
                {
                    throw new ArgumentException(String.Format("Training set contains no items."));
                }

                var errorsPerIterations = new List<double>();
                var errorsPerIterationsInValidationSet = new List<double>();
                var correctDirectionPredictionsRateInIterations = new List<double>();
                var correctUpPredictionsRateInIterations = new List<double>();
                var correctDownPredictionsRateInIterations = new List<double>();
                int totalUp = 0;
                int totalDown = 0;

                for (int currentIteration = 0; currentIteration < iterationNumber; currentIteration++)
                {
                    progressFunction.Report(currentIteration);

                    double errorInIteration = 0;
                    double errorInIterationInValidationSet = 0;
                    int correctDirectionPredictionsInIteration = 0;
                    int correctUpPredictionsInIteration = 0;
                    int correctDownPredictionsInIteration = 0;
                    totalUp = 0;
                    totalDown = 0;

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

                        if ((result[0] - previousCaseResult[0]) * (predictedResult[0] - previousCaseResult[0]) > 0)
                        {
                            // Correct direction prediction
                            correctDirectionPredictionsInIteration++;

                            if (predictedResult[0] - previousCaseResult[0] > 0)
                                correctUpPredictionsInIteration++;
                            else
                                correctDownPredictionsInIteration++;
                        }

                        if (predictedResult[0] - previousCaseResult[0] > 0)
                            totalUp++;
                        else
                            totalDown++;

                        errorInIteration += error;
                    }

                    errorsPerIterations.Add(errorInIteration / trainingSet.Count());
                    correctDirectionPredictionsRateInIterations.Add((double)correctDirectionPredictionsInIteration /
                                                                    trainingSet.Count());
                    correctUpPredictionsRateInIterations.Add((double)correctUpPredictionsInIteration /
                                                                    totalUp);
                    correctDownPredictionsRateInIterations.Add((double)correctDownPredictionsInIteration /
                                                                    totalDown);

                    //Walidacja na zbiorze walidacyjnym
                    for (int currentValidatingIndex = 0;
                        currentValidatingIndex < validationSet.Count();
                        currentValidatingIndex++)
                    {
                        double[] arguments = validationSet[currentValidatingIndex].Input;
                        double[] predictedResult = validationSet[currentValidatingIndex].Output;

                        double[] result = network.Calculate(
                            arguments,
                            maxInputValues,
                            minInputValues,
                            maxOutputValues,
                            minOutputValues,
                            null,
                            learningCoefficient,
                            inertiaCoefficient);

                        double error = Math.Pow(result[0] - predictedResult[0], 2);
                        errorInIterationInValidationSet += error;
                    }
                    errorsPerIterationsInValidationSet.Add(errorInIterationInValidationSet / validationSet.Count());

                    var elemCountInErrorsList = errorsPerIterationsInValidationSet.Count();
                    if (elemCountInErrorsList > 100)
                    {
                      if(errorsPerIterationsInValidationSet.ElementAt(elemCountInErrorsList-1)-errorsPerIterationsInValidationSet.ElementAt(elemCountInErrorsList-2)>0
                         && errorsPerIterationsInValidationSet.ElementAt(elemCountInErrorsList-2)-errorsPerIterationsInValidationSet.ElementAt(elemCountInErrorsList-3)>0
                         && errorsPerIterationsInValidationSet.ElementAt(elemCountInErrorsList-3)-errorsPerIterationsInValidationSet.ElementAt(elemCountInErrorsList-4)>0)
                      {
                          //TODO - za szybko wychodzimy z pętli (inny warunek?)
                          //break;
                      }
                    }
                }

                ResultDTO resultDto = SetResult(network.InputNumber, network.OutputNumber);
                resultDto.TrainingCasesUpPercent = (double)totalUp / trainingSet.Count() * 100;
                resultDto.TrainingCasesDownPercent = (double)totalDown / trainingSet.Count() * 100;
                resultDto.ErrorsPerIterations = errorsPerIterations;
                resultDto.ErrorsPerIterationsInValidationSet = errorsPerIterationsInValidationSet;
                SetResultCorrectDirectionPredictionsRate(resultDto,
                    correctDirectionPredictionsRateInIterations,
                    correctUpPredictionsRateInIterations,
                    correctDownPredictionsRateInIterations);
                return resultDto;
            });
        }


        private void SetResultCorrectDirectionPredictionsRate(ResultDTO result, List<double> factors, List<double> upFactors, List<double> downFactors)
        {
            result.MaxTrainingCorrectDirectionPredictionsRate = factors.Max();
            result.MinTrainingCorrectDirectionPredictionsRate = factors.Min();
            result.AverageTrainingCorrectDirectionPredictionsRate = factors.Average();
            result.FirstTrainingCorrectDirectionPredictionsRate = factors.First();
            result.LastTrainingCorrectDirectionPredictionsRate = factors.Last();

            result.AverageTrainingCorrectUpPredictionsRate = upFactors.Average();
            result.LastTrainingCorrectUpPredictionsRate = upFactors.Last();

            result.AverageTrainingCorrectDownPredictionsRate = downFactors.Average();
            result.LastTrainingCorrectDownPredictionsRate = downFactors.Last();
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

                int totalUp = 0;
                int totalDown = 0;
                int correctUpPred = 0;
                int correctDownPred = 0;
                int correctDirectionPred = 0;

                for (int i = 0; i < testSet.Count; i++)
                {
                    resultDto.NetworkPredictionCaseDay.Add(trainingSet.Count + i);
                    double expetedResult = testSet[i].Output[0];
                    double prevResult = testSet[i].OutputInPreviousCase[0];
                    double networkResult = (network.Calculate(
                        testSet[i].Input,
                        maxInputValues,
                        minInputValues,
                        maxOutputValues,
                        minOutputValues,
                        null,
                        learningCoefficient,
                        inertiaCoefficient))[0];
                    resultDto.NetworkPredictedValue.Add(networkResult);

                    if (expetedResult - prevResult > 0)
                    {
                        totalUp++;
                        if ((expetedResult - prevResult) * (networkResult - prevResult) > 0)
                            correctUpPred++;
                    }
                    else
                    {
                        totalDown++;
                        if ((expetedResult - prevResult) * (networkResult - prevResult) > 0)
                            correctDownPred++;
                    }

                    if ((expetedResult - prevResult) * (networkResult - prevResult) > 0)
                        correctDirectionPred++;
                }

                resultDto.TestCasesUpPercent = (double)totalUp / testSet.Count * 100;
                resultDto.TestCasesDownPercent = (double)totalDown / testSet.Count * 100;
                resultDto.TestCorrectUpPredictionsRate = (double)correctUpPred / totalUp;
                resultDto.TestCorrectDownPredictionsRate = (double)correctDownPred / totalDown;
                resultDto.TestCorrectDirectionPredictionsRate = (double)correctDirectionPred / testSet.Count;

            }
            return resultDto;
        }
    }
}
