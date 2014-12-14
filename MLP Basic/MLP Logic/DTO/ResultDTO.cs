using System.Collections.Generic;

namespace MLP_Logic.DTO
{
    public class ResultDTO : BaseDTO
    {
        public List<double> TestCaseDay;
        public List<double> TestCaseValue;

        public List<double> TrainingCaseDay;
        public List<double> TrainingCaseValue;

        public List<double> NetworkPredictionCaseDay;
        public List<double> NetworkPredictedValue;

        public List<double> ErrorsPerIterations;

        public ResultDTO()
        {
        }
    }
}
