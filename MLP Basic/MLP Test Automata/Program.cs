using System;
using System.Collections.Generic;
using System.Linq;
using MLP_Data.Enums;
using MLP_Logic.DTO;
using MLP_Logic.Enums;
using MLP_Logic.Logic;
using NLog;

namespace MLP_Test_Automata
{
    class Program
    {
        private const int TestCasesNumber = 10;
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
                            ResultDTO result = await LogicManager.Run(neuronNetworkDto, DummyProgessFunction);
                            resultList.Add(result);
                            resultError += result.ErrorsPerIterations.Last();
                        }

                        resultError /= TestCasesNumber;

                        //Logger.Info("{0} / LC {1} / IC {2}: {3}", networkType, lcItem, icItem, resultError);
                    }
                }
            }
        }
    }
}
