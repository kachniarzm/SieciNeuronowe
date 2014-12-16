using MLP_Data.Enums;

namespace MLP_Logic.DTO
{
    public class EnvironmentDTO : BaseDTO
    {
        public int IterationNumber { get; private set; }
        public double LearningCoefficient { get; private set; }
        public double InertiaCoefficient { get; private set; }
        public double ProportionalDivisionTrainingTestData { get; private set; }
        public IndexName PredictionChoice { get; private set; }
        public InputDataDateUnits Step { get; private set; }
        public InputDataDateUnits Density { get; private set; }
        public InputDataDateUnits WindowLength { get; private set; }

        public EnvironmentDTO(string iterationNumber, string learningCoefficient, string inertiaCoefficient, double proportionalDivision, IndexName predictionChoice,
            InputDataDateUnits step, InputDataDateUnits density, InputDataDateUnits windowLength)
        {

            int intValue;
            if (!int.TryParse(iterationNumber, out intValue))
            {
                exceptions.Add("Iteration number invalid");
            }
            else
            {
                IterationNumber = intValue;
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
