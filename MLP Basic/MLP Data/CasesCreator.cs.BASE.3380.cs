using System;
using System.Collections.Generic;
using MLP_Data.Entity;
using MLP_Data.Enums;

namespace MLP_Data
{
    public class CasesCreator
    {
        // Assumption:
        // collection[0] - most recent day
        // collection[n] - most past day
        public static List<TestCase> Create(List<object> collection,
            InputDataDateUnits windowLength,
            InputDataDateUnits density,
            InputDataDateUnits step = InputDataDateUnits.Day,
            bool formOldest = true,
            bool usePca = true,
            int inputVectorMaxLenght = 10)
        {
            if (collection == null) return null;

            int daysInWindowLength = windowLength.GetDaysNumber();
            int dataFrequency = density.GetDaysNumber();
            int daysInStep = step.GetDaysNumber();
            int itemsInCase = daysInWindowLength / dataFrequency;
            if (itemsInCase <= 0 || (itemsInCase == 1 && density != InputDataDateUnits.Day))
            {
                throw new ArgumentException(
                    String.Format("input data window length {0} is too small for density {1}",
                        windowLength.GetDescription(), density.GetDescription()));
            }

            var casesNumber = (int)Math.Ceiling((decimal)(collection.Count - daysInWindowLength) / daysInStep);
            if (casesNumber <= 0)
            {
                throw new ArgumentException(
                    String.Format("collection ({0} items) too small for input data window length {1} with step {2}",
                        collection.Count, windowLength.GetDescription(), step.GetDescription()));
            }

            int nextCaseDirection = formOldest ? -1 : 1;
            var testCases = new List<TestCase>();

            for (int currentCaseNumber = 0, currentStartDay = (formOldest) ? 1 + (casesNumber - 1) * daysInStep : 1;
                currentCaseNumber < casesNumber;
                currentCaseNumber++, currentStartDay += daysInStep * nextCaseDirection)
            {
                var inputRawData = new List<object>();
                for (int itemsInCurrentCaseNumber = 0; itemsInCurrentCaseNumber < itemsInCase; itemsInCurrentCaseNumber++)
                {
                    inputRawData.Add(collection[currentStartDay + itemsInCurrentCaseNumber * dataFrequency]);
                }
                var testCase = new TestCase { InputRawData = inputRawData, OutputRawData = collection[currentStartDay - 1] };

                testCases.Add(testCase);
            }

            if (usePca)
            {
                PcaOptimizator.Optimize(testCases, inputVectorMaxLenght);
            }         

            return testCases;
        }

        public static Type GetTypeByPredictionChoice(IndexName predictionChoice)
        {
            if (predictionChoice == IndexName.WIG20)
                return typeof(StockExchangeListing);
            if (predictionChoice == IndexName.WIG20withMacro)
                return typeof(StockExchangeListingWithMacro);
            if (predictionChoice == IndexName.SP500)
                return typeof(StockExchangeListingBase);
            if (predictionChoice == IndexName.Wig20ClosingAndVolumeOnly)
                return typeof (StockExchangeListingClosingAndVolumeOnly);
            if (predictionChoice.ToString().Contains("Function")
                || predictionChoice.ToString().Contains("TimeSeries")
                || predictionChoice == IndexName.WIG20Closing
                )
                return typeof(Function1);

            throw new ArgumentException("GetTypeByPredictionChoice");
        }
    }
}
