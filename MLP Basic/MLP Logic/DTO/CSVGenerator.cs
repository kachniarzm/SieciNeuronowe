using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MLP_Logic.DTO
{
    public static class CSVGenerator
    {
        public static void CreateCSVFile(String url, List<String> inputKeys, List<String> outputKeys, ResultDTO resultDTO, TaskType type)
        {
            CultureInfo oldCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            CultureInfo customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;

            string filePath = url;
            string delimeter = ",";
            int LineNumber = 0;
            if (type == TaskType.Regresion) LineNumber = resultDTO.NetworkPredictionCaseDay.Count + 1;
            //else LineNumber = resultDTO.Case2TestArgument1.Count + 1;
            int KeysNumber = inputKeys.Count + outputKeys.Count;
            string[][] outputTestData = new string[LineNumber][];
            for (int i = 0; i < outputTestData.Count(); i++)
                outputTestData[i] = new string[KeysNumber];
            for (int i = 0; i < inputKeys.Count; i++)
                outputTestData[0][i] = inputKeys[i];
            for (int i = inputKeys.Count; i < KeysNumber; i++)
                outputTestData[0][i] = outputKeys[i - inputKeys.Count];

            if (type == TaskType.Regresion)//1 wejście 1 wyjście
            {
                for (int i = 1; i < LineNumber; i++)
                {
                    outputTestData[i][0] = resultDTO.NetworkPredictionCaseDay[i - 1].ToString();
                    outputTestData[i][1] = resultDTO.NetworkPredictedValue[i - 1].ToString();
                }
            }
            //else if (type == TaskType.ClasificationOneOutput || type == TaskType.ClasificationManyOutputs)//2 wejścia 1 wyjście
            //{
            //    for (int i = 1; i < LineNumber; i++)
            //    {
            //        outputTestData[i][0] = resultDTO.Case2TestArgument1[i - 1].ToString();
            //        outputTestData[i][1] = resultDTO.Case2TestArgument2[i - 1].ToString();
            //        outputTestData[i][2] = resultDTO.Case2TestValue[i - 1].ToString();
            //    }
            //}

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < outputTestData.Count(); i++)
            {
                sb.AppendLine(string.Join(delimeter, outputTestData[i]));
            }

            File.WriteAllText(filePath, sb.ToString());
            Thread.CurrentThread.CurrentCulture = oldCulture;
        }
    }
}
