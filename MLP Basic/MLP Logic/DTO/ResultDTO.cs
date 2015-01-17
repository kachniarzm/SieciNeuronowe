using System.Collections.Generic;

namespace MLP_Logic.DTO
{
    public class ResultDTO : BaseDTO
    {
        public double MinValue;
        public double MaxValue;

        public List<double> TestCaseDay;
        public List<double> TestCaseValue;

        public List<double> TrainingCaseDay;
        public List<double> TrainingCaseValue; 
        
        public List<double> ValidationCaseDay;
        public List<double> ValidationCaseValue;

        public List<double> NetworkPredictionCaseDay;
        public List<double> NetworkPredictedValue;

        public List<double> ErrorsPerIterations;
        public List<double> ErrorsPerIterationsInValidationSet;

        public double LastTrainingCorrectUpPredictionsRate;
        public double AverageTrainingCorrectUpPredictionsRate;

        public double LastTrainingCorrectDownPredictionsRate;
        public double AverageTrainingCorrectDownPredictionsRate;

        public double MaxTrainingCorrectDirectionPredictionsRate;
        public double MinTrainingCorrectDirectionPredictionsRate;
        public double FirstTrainingCorrectDirectionPredictionsRate;
        public double LastTrainingCorrectDirectionPredictionsRate;
        public double AverageTrainingCorrectDirectionPredictionsRate;

        public double TrainingCasesUpPercent;
        public double TrainingCasesDownPercent;

        public double TestCorrectUpPredictionsRate;
        public double TestCorrectDownPredictionsRate;
        public double TestCorrectDirectionPredictionsRate;

        public double TestCasesUpPercent;
        public double TestCasesDownPercent;

        public int InputColumns;

        public ResultDTO()
        {
        }
    }
}
