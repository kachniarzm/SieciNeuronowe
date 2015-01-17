using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MLP_Data.Enums;
using MLP_Logic.DTO;
using MLP_Logic.Enums;
using MLP_Logic.Logic;

namespace MLP_Test_Automata
{
    class Program
    {
        private const int TestCasesNumber = 3;
        private const double ProportionalDivisionTrainingTestData = 70.0;
        private const IndexName PredictionChoice = IndexName.WIG20withMacro;

        private static List<int> _iterationNumberList;
        private static List<string> _neuronStructureList;
        private static List<NeuronNetworkType> _neuronNetworkTypeList;
        private static List<double> _learningCoeficientList;
        private static List<double> _inertiaCoeficientList;
        private static List<int> _maxInputColumnsList;// 0 means do not use PCA
        private static List<InputDataDateUnits> _densityList;
        private static List<InputDataDateUnits> _windowLengthList;
        private static List<InputDataDateUnits> _stepList;

        private static readonly LogicManager LogicManager = new LogicManager();
        private static readonly IProgress<int> DummyProgessFunction = new Progress<int>(value => { });

        static void Main()
        {

            InitParametersWig20();
            //InitParametersTimeSeries();


#if DEBUG
                Console.WriteLine("Debug mode. If starting long tests better choose Release mode and restart. Continue anyway? (press enter for yes)");
                Console.ReadLine();
#endif

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Started with debugging. If starting long tests better choose start without debugging and restart. Continue anyway? (press enter for yes)");
                Console.ReadLine();
            }

            Run();
        }

        private static void InitParametersWig20()
        {
            _iterationNumberList = new List<int>
            {
                100
            };
            _neuronStructureList = new List<string>
            {
                "5;1"
            };
            _neuronNetworkTypeList = new List<NeuronNetworkType>
            {
                NeuronNetworkType.MLP,
                NeuronNetworkType.Jordan,
                NeuronNetworkType.Elman
            };
            _learningCoeficientList = new List<double>
            {
                0.8
            };
            _inertiaCoeficientList = new List<double>
            {
                0.1
            };
            _maxInputColumnsList = new List<int> // 0 means do not use PCA
            {
                0
            };
            _densityList = new List<InputDataDateUnits>
            {
                InputDataDateUnits.Day
            };
            _windowLengthList = new List<InputDataDateUnits>
            {
                //InputDataDateUnits.Day
                InputDataDateUnits.Week
            };
            _stepList = new List<InputDataDateUnits>
            {
                InputDataDateUnits.Day
            };
        }

        private static void InitParametersTimeSeries()
        {
            _iterationNumberList = new List<int>
            {
                100
            };
            _neuronStructureList = new List<string>
            {
                "5;1","10;1","20;1","30;1","5;5;1","20;20;1"
            };
            _neuronNetworkTypeList = new List<NeuronNetworkType>
            {
                NeuronNetworkType.MLP,
                NeuronNetworkType.Jordan,
                NeuronNetworkType.Elman
            };
            _learningCoeficientList = new List<double>
            {
                1
            };
            _inertiaCoeficientList = new List<double>
            {
                0
            };
            _maxInputColumnsList = new List<int> // 0 means do not use PCA
            {
                0
            };
            _densityList = new List<InputDataDateUnits>
            {
                InputDataDateUnits.Day
            };
            _windowLengthList = new List<InputDataDateUnits>
            {
                InputDataDateUnits.Day
            };
            _stepList = new List<InputDataDateUnits>
            {
                InputDataDateUnits.Day
            };
        }

   

        private static void Run()
        {
            var csvFile = new StringBuilder();
            var startTime = DateTime.Now;

            try
            {
                var currentTestCaseTotal = 0;
                var maxTestCaseTotal = _windowLengthList.Count *
                                  _densityList.Count *
                                  _stepList.Count *
                                  _maxInputColumnsList.Count *
                                  _neuronStructureList.Count *
                                  _iterationNumberList.Count *
                                  _inertiaCoeficientList.Count *
                                  _learningCoeficientList.Count *
                                  _neuronNetworkTypeList.Count *
                                  TestCasesNumber;

                CreateHeadInformationInCSVFile(csvFile, startTime);

                foreach (var windowLength in _windowLengthList)
                {
                    foreach (var density in _densityList)
                    {
                        foreach (var step in _stepList)
                        {
                            foreach (var maxInputColumns in _maxInputColumnsList)
                            {
                                foreach (var neuronStructure in _neuronStructureList)
                                {
                                    foreach (var iterationNumber in _iterationNumberList)
                                    {
                                        foreach (var inertiaCoefficient in _inertiaCoeficientList)
                                        {
                                            foreach (var learningCoefficient in _learningCoeficientList)
                                            {
                                                EnvironmentDTO environmentDto = new EnvironmentDTO()
                                                {
                                                    IterationNumber = iterationNumber,
                                                    LearningCoefficient = learningCoefficient,
                                                    InertiaCoefficient = inertiaCoefficient,
                                                    ProportionalDivisionTrainingTestData = ProportionalDivisionTrainingTestData,
                                                    Density = density,
                                                    WindowLength = windowLength,
                                                    Step = step,
                                                    PredictionChoice = PredictionChoice,
                                                    UsePca = maxInputColumns > 0,
                                                    MaxInputColumns = maxInputColumns
                                                };

                                                foreach (var networkType in _neuronNetworkTypeList)
                                                {
                                                    NeuronNetworkDTO neuronNetworkDto = new NeuronNetworkDTO(
                                                        neuronStructure,
                                                        true, true, networkType);

                                                    double testCorrectDirectionPredictionsRate;
                                                    double trend;
                                                    double lastTrainingCorrectDirectionPredictionsRate;
                                                    double testCorrectUpPredictionsRate;
                                                    double testCorrectDownPredictionsRate;
                                                    var resultError = RunTestCaseComputingOnNeuronNetwork(currentTestCaseTotal, environmentDto, neuronNetworkDto, 
                                                        startTime, maxTestCaseTotal, out testCorrectDirectionPredictionsRate, out trend,
                                                        out lastTrainingCorrectDirectionPredictionsRate, out testCorrectUpPredictionsRate,
                                                        out testCorrectDownPredictionsRate);

                                                    resultError /= TestCasesNumber;
                                                    var newLine = CreateNewLineToCSVFile(networkType, learningCoefficient, inertiaCoefficient,
                                                        iterationNumber, neuronStructure, maxInputColumns, windowLength, density, step, resultError,
                                                        testCorrectDirectionPredictionsRate, testCorrectDownPredictionsRate, testCorrectUpPredictionsRate, 
                                                        trend, lastTrainingCorrectDirectionPredictionsRate);

                                                    Console.WriteLine(newLine);
                                                    csvFile.AppendLine(newLine);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var endTime = DateTime.Now;
                var calculationTime = endTime - startTime;
                csvFile.Replace("<CalculationTime>", calculationTime.ToString(@"hh\:mm\:ss"));

                File.WriteAllText(String.Format(@"../../../Result data/result_{0}.csv", startTime.ToString("G").Replace(" ", "_").Replace(":", ".")), csvFile.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();

                File.WriteAllText(String.Format(@"../../../Result data/result_{0}_ErrorOccurs.csv", startTime.ToString("G").Replace(" ", "_").Replace(":", ".")), csvFile.ToString());
            }
        }

        private static void CreateHeadInformationInCSVFile(StringBuilder csvFile, DateTime startTime)
        {
            csvFile.AppendLine(String.Format("User:;{0}", Environment.UserName));
            csvFile.AppendLine(String.Format("IndexName:;{0}", PredictionChoice));
            csvFile.AppendLine(String.Format("StartDate:;{0}", startTime.ToString("G")));
            csvFile.AppendLine(String.Format("CalculationTime:;<CalculationTime>"));
            csvFile.AppendLine(String.Format("TestCasesNumber:;{0}", TestCasesNumber));
            csvFile.AppendLine(String.Format("ProportionalDivisionTrainingTestData:;{0}", ProportionalDivisionTrainingTestData));
            if (PredictionChoice == IndexName.WIG20withMacro ||
                PredictionChoice == IndexName.Wig20ClosingAndVolumeOnly ||
                PredictionChoice == IndexName.WIG20Closing)
            {
                csvFile.AppendLine(
                    String.Format(
                        "networkType;learningCoefficient;inertiaCoefficient;iterationNumber;neuronStructure;maxInputColumns;windowLength;density;step;resultError;TestCorrectDirectionPredictionsRate;TestCorrectDownPredictionsRate;TestCorrectUpPredictionsRate;Trend;LastTrainingCorrectDirectionPredictionsRate"));
            }
            else if (PredictionChoice == IndexName.TimeSeriesOur)
            {
                csvFile.AppendLine(
                    String.Format(
                        "networkType;learningCoefficient;inertiaCoefficient;iterationNumber;neuronStructure;maxInputColumns;windowLength;density;step;resultError"));
            }
            else
            {
                Console.WriteLine("Error: PredictionChoice={0} Not Implemented", PredictionChoice);
                Console.ReadLine();
                return;
            }
        }

        private static string CreateNewLineToCSVFile(NeuronNetworkType networkType, double learningCoefficient,
            double inertiaCoefficient, int iterationNumber, string neuronStructure, int maxInputColumns,
            InputDataDateUnits windowLength, InputDataDateUnits density, InputDataDateUnits step, double resultError,
            double testCorrectDirectionPredictionsRate, double testCorrectDownPredictionsRate,
            double testCorrectUpPredictionsRate, double trend, double lastTrainingCorrectDirectionPredictionsRate)
        {
            string newLine = "";
            if (PredictionChoice == IndexName.WIG20withMacro ||
                PredictionChoice == IndexName.Wig20ClosingAndVolumeOnly ||
                PredictionChoice == IndexName.WIG20Closing)
            {
                newLine =
                    String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}",
                        networkType,
                        learningCoefficient,
                        inertiaCoefficient,
                        iterationNumber,
                        neuronStructure.Replace(";", "~"),
                        maxInputColumns == 0
                            ? "no PCA"
                            : maxInputColumns.ToString(),
                        windowLength,
                        density,
                        step,
                        resultError,
                        testCorrectDirectionPredictionsRate,
                        testCorrectDownPredictionsRate,
                        testCorrectUpPredictionsRate,
                        trend,
                        lastTrainingCorrectDirectionPredictionsRate);
            }
            else if (PredictionChoice == IndexName.TimeSeriesOur)
            {
                newLine =
                    String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}",
                        networkType,
                        learningCoefficient,
                        inertiaCoefficient,
                        iterationNumber,
                        neuronStructure.Replace(";", "~"),
                        maxInputColumns == 0
                            ? "no PCA"
                            : maxInputColumns.ToString(),
                        windowLength,
                        density,
                        step,
                        resultError);
            }
            else
            {
                Console.WriteLine("Error: PredictionChoice={0} Not Implemented", PredictionChoice);
                Console.ReadLine();
                return "Not implemented";
            }
            return newLine;
        }

        private static double RunTestCaseComputingOnNeuronNetwork(int currentTestCaseTotal, EnvironmentDTO environmentDto,
            NeuronNetworkDTO neuronNetworkDto, DateTime startTime, int maxTestCaseTotal,
            out double testCorrectDirectionPredictionsRate, out double trend,
            out double lastTrainingCorrectDirectionPredictionsRate, out double testCorrectUpPredictionsRate,
            out double testCorrectDownPredictionsRate)
        {
            double resultError = 0;
            testCorrectDirectionPredictionsRate = 0;
            trend = 0;
            lastTrainingCorrectDirectionPredictionsRate = 0;
            testCorrectUpPredictionsRate = 0;
            testCorrectDownPredictionsRate = 0;

            for (var currentTestCase = 0;
                currentTestCase < TestCasesNumber;
                currentTestCase++, currentTestCaseTotal++)
            {
                LogicManager.SetEnviorment(environmentDto);
                ResultDTO result = LogicManager.RunSync(neuronNetworkDto,
                    DummyProgessFunction);
                resultError += result.ErrorsPerIterations.Last();
                testCorrectDirectionPredictionsRate =
                    result.TestCorrectDirectionPredictionsRate;
                testCorrectUpPredictionsRate =
                    result.TestCorrectUpPredictionsRate;
                testCorrectDownPredictionsRate =
                    result.TestCorrectDownPredictionsRate;

                trend = Math.Max(result.TestCasesDownPercent, result.TestCasesUpPercent)/100;
                lastTrainingCorrectDirectionPredictionsRate =
                    result.LastTrainingCorrectDirectionPredictionsRate;
                DateTime aproximateEndTime = startTime.Add(new TimeSpan(0, 0, 0, 0,
                    (int) ((DateTime.Now - startTime).Duration().TotalMilliseconds*maxTestCaseTotal/currentTestCaseTotal)));
                Console.Title = String.Format("Done {0} from {1} test cases. Aproximate end time: {2}",
                    currentTestCaseTotal, maxTestCaseTotal, aproximateEndTime.ToString("F"));
            }
            return resultError;
        }
    }
}
