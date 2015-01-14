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
        private const int TestCasesNumber = 100;
        private const double ProportionalDivisionTrainingTestData = 0.7;
        private const IndexName PredictionChoice = IndexName.TimeSeriesOur;

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
            InitParameters2();

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

        private static void InitParameters1()
        {
            _iterationNumberList = new List<int>
            {
                500
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
                0, 0.2, 0.4, 0.6, 0.8, 1
            };
            _inertiaCoeficientList = new List<double>
            {
                0, 0.2, 0.4, 0.6, 0.8, 1
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

        private static void InitParameters2()
        {
            _iterationNumberList = new List<int>
            {
                100, 500
            };
            _neuronStructureList = new List<string>
            {
                "2;1", "3;1", "5;1", "10;1", "30;1", "2;2;1", "3;3;1", "5;5;1", "10;10;1"
            };
            _neuronNetworkTypeList = new List<NeuronNetworkType>
            {
                NeuronNetworkType.MLP,
                NeuronNetworkType.Jordan,
                NeuronNetworkType.Elman
            };
            _learningCoeficientList = new List<double>
            {
                0, 0.2, 0.4, 0.6, 0.8, 1
            };
            _inertiaCoeficientList = new List<double>
            {
                0, 0.2, 0.4, 0.6, 0.8, 1
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

                csvFile.AppendLine(String.Format("User:;{0}", Environment.UserName));
                csvFile.AppendLine(String.Format("IndexName:;{0}", PredictionChoice));
                csvFile.AppendLine(String.Format("StartDate:;{0}", startTime.ToString("G")));
                csvFile.AppendLine(String.Format("CalculationTime:;<CalculationTime>"));
                csvFile.AppendLine(String.Format("TestCasesNumber:;{0}", TestCasesNumber));
                csvFile.AppendLine(String.Format("ProportionalDivisionTrainingTestData:;{0}", ProportionalDivisionTrainingTestData));
                csvFile.AppendLine(String.Format("networkType;learningCoefficient;inertiaCoefficient;iterationNumber;neuronStructure;maxInputColumns;windowLength;density;step;resultError"));

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

                                                    double resultError = 0;

                                                    for (var currentTestCase = 0;
                                                        currentTestCase < TestCasesNumber;
                                                        currentTestCase++, currentTestCaseTotal++)
                                                    {
                                                        LogicManager.SetEnviorment(environmentDto);
                                                        ResultDTO result = LogicManager.RunSync(neuronNetworkDto,
                                                            DummyProgessFunction);
                                                        resultError += result.ErrorsPerIterations.Last();
                                                        DateTime aproximateEndTime = startTime.Add(new TimeSpan(0, 0, 0, 0,
                                                            (int)((DateTime.Now - startTime).Duration().TotalMilliseconds * maxTestCaseTotal / currentTestCaseTotal)));
                                                        Console.Title = String.Format("Done {0} from {1} test cases. Aproximate end time: {2}",
                                                            currentTestCaseTotal, maxTestCaseTotal, aproximateEndTime.ToString("F"));
                                                    }

                                                    resultError /= TestCasesNumber;

                                                    var newLine = String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}",
                                                        networkType,
                                                        learningCoefficient,
                                                        inertiaCoefficient,
                                                        iterationNumber,
                                                        neuronStructure.Replace(";", "~"),
                                                        maxInputColumns == 0 ? "no PCA" : maxInputColumns.ToString(),
                                                        windowLength,
                                                        density,
                                                        step,
                                                        resultError);

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
    }
}
