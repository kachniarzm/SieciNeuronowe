using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MLP_Data.Enums;
using MLP_Logic.DTO;
using MLP_Logic.Enums;
using MLP_Logic.Logic;
using NLog;

namespace MLP_Test_Automata
{
    class Program
    {
        private const int TestCasesNumber = 100;
        private const int IterationNumber = 500;
        private const string NeuronStructure = "5;1";
        private static readonly List<NeuronNetworkType> NeuronNetworkTypes = new List<NeuronNetworkType>{
            NeuronNetworkType.MLP,
            NeuronNetworkType.Jordan,
            NeuronNetworkType.Elman};
        private static readonly List<double> LcList = new List<double>{
            0, 0.2, 0.4, 0.6, 0.8, 1};
        private static readonly List<double> IcList = new List<double>{
            0, 0.2, 0.4, 0.6, 0.8, 1};

        private static readonly LogicManager LogicManager = new LogicManager();
        private static readonly IProgress<int> DummyProgessFunction = new Progress<int>(value => {});
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
       
        static void Main()
        {
            Run();
        }

        private async static void Run()
        {
            var csvFile = new StringBuilder();
            foreach (var icItem in IcList)
            {
                foreach (var lcItem in LcList)
                {
                    EnvironmentDTO environmentDto = new EnvironmentDTO()
                    {
                        IterationNumber = IterationNumber,
                        LearningCoefficient = lcItem,
                        InertiaCoefficient = icItem,
                        ProportionalDivisionTrainingTestData = 0.7,
                        Density = InputDataDateUnits.Day,
                        WindowLength = InputDataDateUnits.Day,
                        Step = InputDataDateUnits.Day,
                        PredictionChoice = IndexName.TimeSeriesOur,
                        UsePca = false,
                        MaxInputColumns = 0
                    };

                    foreach (var networkType in NeuronNetworkTypes)
                    {
                        NeuronNetworkDTO neuronNetworkDto = new NeuronNetworkDTO(NeuronStructure, true, true,
                            networkType);

                        List<ResultDTO> resultList = new List<ResultDTO>();
                        double resultError = 0;

                        for (int i1 = 0; i1 < TestCasesNumber; i1++)
                        {
                            LogicManager.SetEnviorment(environmentDto);
                            ResultDTO result = LogicManager.RunSync(neuronNetworkDto, DummyProgessFunction);
                            resultList.Add(result);
                            resultError += result.ErrorsPerIterations.Last();
                        }

                        resultError /= TestCasesNumber;
                        Console.WriteLine("{0} / LC {1} / IC {2}: {3}", networkType, lcItem, icItem, resultError);

                        var newLine = string.Format("{0},{1},{2},{3}{4}", networkType, lcItem, icItem, resultError, Environment.NewLine);
                        csvFile.Append(newLine);

                        
                        //Logger.Info("{0} / LC {1} / IC {2}: {3}", networkType, lcItem, icItem, resultError);
                    }
                }
            }
            File.WriteAllText(@"../../../Test data/csvTestFile.csv", csvFile.ToString());
        }
    }
}
