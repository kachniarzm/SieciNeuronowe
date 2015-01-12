using MLP_Data.Enums;

namespace MLP_Logic.DTO
{
    public class EnvironmentDTO : BaseDTO
    {
        public int IterationNumber { get; set; }
        public double LearningCoefficient { get; set; }
        public double InertiaCoefficient { get; set; }
        public double ProportionalDivisionTrainingTestData { get; set; }
        public IndexName PredictionChoice { get; set; }
        public InputDataDateUnits Step { get; set; }
        public InputDataDateUnits Density { get; set; }
        public InputDataDateUnits WindowLength { get; set; }
        public int MaxInputColumns { get; set; }
        public bool UsePca { get; set; }

        public EnvironmentDTO()
        {
        }

        public EnvironmentDTO(string iterationNumber, string learningCoefficient, string inertiaCoefficient, double proportionalDivision, IndexName predictionChoice,
            InputDataDateUnits step, InputDataDateUnits density, InputDataDateUnits windowLength, bool usePca, string maxInputColumns)
        {
            UsePca = usePca;

            int intValue;
            if (!int.TryParse(iterationNumber, out intValue))
            {
                exceptions.Add("Iteration number invalid");
            }
            else
            {
                IterationNumber = intValue;
            }

            if (!int.TryParse(maxInputColumns, out intValue))
            {
                exceptions.Add("Max input column number invalid");
            }
            else
            {
                MaxInputColumns = intValue;
            }

            double value;
            if (!double.TryParse(learningCoefficient, out value))
            {
                exceptions.Add("Learning coefficient invalid");
            }
            else
            {
                LearningCoefficient = value;
            }

            if (!double.TryParse(inertiaCoefficient, out value))
            {
                exceptions.Add("Inertia coefficient invalid");
            }
            else
            {
                InertiaCoefficient = value;
            }

            if (proportionalDivision < 0 && proportionalDivision > 100)
            {
                exceptions.Add("Proportional Division Training and Test data invalid");
            }
            else
            {
                ProportionalDivisionTrainingTestData = proportionalDivision;
            }

            PredictionChoice = predictionChoice;
            if (PredictionChoice == IndexName.Undefined)
            {
                exceptions.Add("Index name undefined");
            }

            Step = step;
            if (Step == InputDataDateUnits.Undefined)
            {
                exceptions.Add("Step is undefined");
            }

            Density = density;
            if (Density == InputDataDateUnits.Undefined)
            {
                exceptions.Add("Density is undefined");
            }

            WindowLength = windowLength;
            if (WindowLength == InputDataDateUnits.Undefined)
            {
                exceptions.Add("Density is undefined");
            }
        }
    }

    public enum TaskType
    {
        Regresion,
        ClasificationOneOutput,
        ClasificationManyOutputs
    }
}
