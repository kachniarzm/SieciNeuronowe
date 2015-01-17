using MLP_Basic.ViewModels;
using MLP_Logic.DTO;
using MLP_Logic.Logic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MLP_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly LogicManager logicManager;
        private NeuronNetworkDTO neuronNetworkDto;
        private readonly MainWindowModel viewModel;
        private EnvironmentDTO environmentDto;

        public MainWindow()
        {
            viewModel = new MainWindowModel();
            DataContext = viewModel;

            InitializeComponent();
            UpdateTitle();

            logicManager = new LogicManager();
        }

        private async void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            environmentDto = ValidateEnviorment();
            neuronNetworkDto = ValidateNeuronNetwork();
            if (environmentDto==null || neuronNetworkDto == null)
            {
            }
            else
            {
                ((Button)sender).Content = "Calculating...";
                ResultLabel.Visibility = Visibility.Collapsed;
                ProgressBar.Visibility = Visibility.Visible;
                ProgressBar.Value = 0;
                ProgressBar.Maximum = environmentDto.IterationNumber;
                IProgress<int> progressFunction = new Progress<int>(value => { ProgressBar.Value = value; });

                List<ResultDTO> resultList = new List<ResultDTO>();
                ResultDTO result = null;
                double resultError = 0;
                logicManager.SetEnviorment(environmentDto);
#if !DEBUG
                try
                {
#endif
                    int testCasesNumber = 1;

                    for (int i = 0; i < testCasesNumber; i++)
                    {
                        ProgressBar.Value = 0;
                        result = await logicManager.Run(neuronNetworkDto, progressFunction);
                        resultList.Add(result);
                        resultError += result.ErrorsPerIterations.Last();
                    }

                    resultError /= testCasesNumber;

#if !DEBUG
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
#endif
                DrawChart(result);
                FillPerformanceIndicators(result);

                ((Button)sender).Content = "Run";
                ProgressBar.Visibility = Visibility.Collapsed;
                ResultLabel.Visibility = Visibility.Visible;
                MessageBox.Show(String.Format("{0} copied to clipboard", resultError), "Task completed", MessageBoxButton.OK);
                Clipboard.SetText(resultError.ToString());
            }

            ((Button)sender).IsEnabled = true; 
        }

        private void FillPerformanceIndicators(ResultDTO result)
        {
            FillLaxbel(MaxTrainingDirectionsPredRateLabel, result.MaxTrainingCorrectDirectionPredictionsRate);
            FillLaxbel(MinTrainingDirectionsPredRateLabel, result.MinTrainingCorrectDirectionPredictionsRate);
            FillLaxbel(FirstTrainingDirectionsPredRateLabel, result.FirstTrainingCorrectDirectionPredictionsRate);
            FillLaxbel(LastTrainingDirectionsPredRateLabel, result.LastTrainingCorrectDirectionPredictionsRate);
            FillLaxbel(AverageTrainingDirectionsPredRateLabel, result.AverageTrainingCorrectDirectionPredictionsRate);

            TrainingCasesUpPercentLabel.Content = String.Format("{0:F2}%", result.TrainingCasesUpPercent);
            FillLaxbel(LastTrainingUpPredRateLabel, result.LastTrainingCorrectUpPredictionsRate);
            FillLaxbel(AverageTrainingUpPredRateLabel, result.AverageTrainingCorrectUpPredictionsRate);

            TrainingCasesDownPercentLabel.Content = String.Format("{0:F2}%", result.TrainingCasesDownPercent);
            FillLaxbel(LastTrainingDownPredRateLabel, result.LastTrainingCorrectDownPredictionsRate);
            FillLaxbel(AverageTrainingDownPredRateLabel, result.AverageTrainingCorrectDownPredictionsRate);

            FillLaxbel(TestDirectionsPredRateLabel, result.TestCorrectDirectionPredictionsRate);

            TestCasesUpPercentLabel.Content = String.Format("{0:F2}%", result.TestCasesUpPercent);
            FillLaxbel(TestUpPredRateLabel, result.TestCorrectUpPredictionsRate);

            TestCasesDownPercentLabel.Content = String.Format("{0:F2}%", result.TestCasesDownPercent);
            FillLaxbel(TestDownPredRateLabel, result.TestCorrectDownPredictionsRate);

            InputColumnsNumber.Content = result.InputColumns;

            double trend = Math.Max(result.TestCasesDownPercent, result.TestCasesUpPercent) / 100;
            if (result.TestCorrectDirectionPredictionsRate < 0.5)
            {
                ResultLabel.Foreground = new SolidColorBrush(Colors.Red);
                ResultLabel.Content = String.Format("Very bad - Result ({0:F4}) less than 0,5",
                    result.TestCorrectDirectionPredictionsRate);
            }
            else if (result.TestCorrectDirectionPredictionsRate < trend)
            {
                ResultLabel.Foreground = new SolidColorBrush(Colors.DarkOrange);
                ResultLabel.Content = String.Format("Bad - Result ({0:F4}) less than trend ({1:F4})",
                    result.TestCorrectDirectionPredictionsRate,
                    trend);
            }
            else if (result.TestCorrectDirectionPredictionsRate - trend < 0.02)
            {
                ResultLabel.Foreground = new SolidColorBrush(Colors.LawnGreen);
                ResultLabel.Content = String.Format("Can be - Result ({0:F4}) is {1:F4} more than trend ({2:F4})",
                    result.TestCorrectDirectionPredictionsRate,
                    result.TestCorrectDirectionPredictionsRate - trend,
                    trend);
            }
            else
            {
                ResultLabel.Foreground = new SolidColorBrush(Colors.Green);
                ResultLabel.Content = String.Format("Very good - Result ({0:F4}) is {1:F4} more than trend ({2:F4})",
                    result.TestCorrectDirectionPredictionsRate,
                    result.TestCorrectDirectionPredictionsRate - trend,
                    trend);
            }
                
        }

        private void FillLaxbel(Label label, double number)
        {
            label.Content = number.ToString("F4");
            if (number < 0.5)
                label.Foreground = new SolidColorBrush(Colors.Red);
            else if (number < 0.525)
                label.Foreground = new SolidColorBrush(Colors.DarkOrange);
            else if (number < 0.55)
                label.Foreground = new SolidColorBrush(Colors.Yellow);
            else if (number < 0.575)
                label.Foreground = new SolidColorBrush(Colors.LawnGreen);
            else if (number <= 1)
                label.Foreground = new SolidColorBrush(Colors.Green);
            else
                label.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void DrawChart(ResultDTO result)
        {
            viewModel.SpecialCasePlotModel.Series.Clear();
            viewModel.StandardPlotModel.Series.Clear();
            viewModel.SpecialCasePlotModel.Axes.Clear();
            viewModel.StandardPlotModel.Axes.Clear();

            if (result != null)
            {
                viewModel.SpecialCasePlotModel.Title = "Stock Index";

                var networkPredictionLineSeries = new LineSeries
                {
                    LineStyle = LineStyle.Solid,
                    MarkerType = MarkerType.Circle,
                    Title = "Network Prediction"
                };
                networkPredictionLineSeries.MarkerSize = networkPredictionLineSeries.MarkerSize*0.5;
                networkPredictionLineSeries.MarkerFill = OxyColors.Red;
                networkPredictionLineSeries.Color = OxyColors.Red;

                var validationCaseLineSeries = new LineSeries
                {
                    LineStyle = LineStyle.Solid,
                    MarkerType = MarkerType.Circle,
                    Title = "Validation Cases Stock Index"
                };
                validationCaseLineSeries.MarkerSize = validationCaseLineSeries.MarkerSize * 0.5;
                validationCaseLineSeries.MarkerFill = OxyColors.Orange;
                validationCaseLineSeries.Color = OxyColors.Orange;


                var testCaseLineSeries = new LineSeries 
                {
                    LineStyle = LineStyle.Solid,
                    MarkerType = MarkerType.Circle,
                    Title = "Test Cases Stock Index"
                };
                testCaseLineSeries.MarkerSize = testCaseLineSeries.MarkerSize*0.5;
                testCaseLineSeries.MarkerFill = OxyColors.Black;
                testCaseLineSeries.Color = OxyColors.Black;

                var trainingCaseLineSeries = new LineSeries
                {
                    LineStyle = LineStyle.Solid,
                    MarkerType = MarkerType.Circle,
                    Title = "Training Cases Stock Index"
                };
                trainingCaseLineSeries.MarkerSize = trainingCaseLineSeries.MarkerSize * 0.5;
                trainingCaseLineSeries.MarkerFill = OxyColors.Blue;
                trainingCaseLineSeries.Color = OxyColors.Blue;

                for (int i = 0; i < result.TestCaseDay.Count; i++)
                {
                    testCaseLineSeries.Points.Add(new DataPoint(result.TestCaseDay[i], result.TestCaseValue[i]));
                }

                for (int i = 0; i < result.ValidationCaseDay.Count; i++)
                {
                    validationCaseLineSeries.Points.Add(new DataPoint(result.ValidationCaseDay[i],result.ValidationCaseValue[i]));
                }

                for (int i = 0; i < result.TrainingCaseDay.Count; i++)
                {
                    trainingCaseLineSeries.Points.Add(new DataPoint(result.TrainingCaseDay[i], result.TrainingCaseValue[i]));
                }

                for (int i = 0; i < result.NetworkPredictionCaseDay.Count; i++)
                {
                    networkPredictionLineSeries.Points.Add(new DataPoint(result.NetworkPredictionCaseDay[i], result.NetworkPredictedValue[i]));
                }

                viewModel.SpecialCasePlotModel.Series.Add(trainingCaseLineSeries);
                viewModel.SpecialCasePlotModel.Series.Add(validationCaseLineSeries);
                viewModel.SpecialCasePlotModel.Series.Add(testCaseLineSeries);
                viewModel.SpecialCasePlotModel.Series.Add(networkPredictionLineSeries);

                viewModel.StandardPlotModel.Title = "Error per iterations";

                var lineSeries3 = new LineSeries
                {
                    LineStyle = LineStyle.Solid,
                    MarkerType = MarkerType.None,
                    Title = "Training set error"
                };
                for (int i = 0; i < result.ErrorsPerIterations.Count; i++)
                {
                    lineSeries3.Points.Add(new DataPoint(i + 1, result.ErrorsPerIterations[i]));
                }
                viewModel.StandardPlotModel.Series.Add(lineSeries3);

                var lineSeries4 = new LineSeries
                {
                    LineStyle = LineStyle.Solid,
                    MarkerType = MarkerType.None,
                    Title = "Validation set error"
                };

                for (int i = 0; i < result.ErrorsPerIterationsInValidationSet.Count; i++)
                {
                    lineSeries4.Points.Add(new DataPoint(i + 1, result.ErrorsPerIterationsInValidationSet[i]));
                }
                viewModel.StandardPlotModel.Series.Add(lineSeries4);

                var linearAxis1 = new LinearAxis
                {
                    Minimum = 0,
                    MaximumPadding = 0.1,
                    MinimumPadding = 0.1,
                    Position = AxisPosition.Bottom,
                    Title = "Step Unit"
                };
                viewModel.SpecialCasePlotModel.Axes.Add(linearAxis1);

                var linearAxis2 = new LinearAxis {Minimum = result.MinValue, Title = "Index Value", Maximum = result.MaxValue};
                viewModel.SpecialCasePlotModel.Axes.Add(linearAxis2);

                var linearAxis3 = new LinearAxis
                {
                    Minimum = 1,
                    MaximumPadding = 0.1,
                    MinimumPadding = 0.1,
                    Position = AxisPosition.Bottom,
                    Title = "Iteration"
                };
                viewModel.StandardPlotModel.Axes.Add(linearAxis3);

                var linearAxis4 = new LinearAxis {Minimum = 0, Title = "Error"};
                viewModel.StandardPlotModel.Axes.Add(linearAxis4);
            }

            viewModel.StandardPlotModel.InvalidatePlot(true);
            viewModel.SpecialCasePlotModel.InvalidatePlot(true);
            UpdateTitle();
        }

        private NeuronNetworkDTO ValidateNeuronNetwork()
        {
            var neuronNetwork = new NeuronNetworkDTO(
              neuronStructure.Text,
              isUnipolar.IsChecked != null && (bool)isUnipolar.IsChecked,
              isBiased.IsChecked != null && (bool)isBiased.IsChecked,
              viewModel.SelectedNeuronNetworkType
              );

            if (!neuronNetwork.IsValid)
            {
                MessageBox.Show(String.Format("Network structure: {0}", neuronNetwork.Exceptions[0]), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return neuronNetwork;
        }

        private EnvironmentDTO ValidateEnviorment()
        {
            var environment = new EnvironmentDTO(
                iterationNumber.Text,
                learningCoefficient.Text,
                inertiaCoefficient.Text,
                proportionalDivisionSlider.Value,
                viewModel.SelectedIndexName,
                viewModel.SelectedStep, 
                viewModel.SelectedDensity, 
                viewModel.SelectedWindowLength,
                usePca.IsChecked != null && (bool)usePca.IsChecked,
                maxColumns.Text);

            if (!environment.IsValid)
            {
                MessageBox.Show(environment.Exceptions[0], "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return environment;
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpMessage;

            switch (((Button)sender).Tag.ToString())
            {
                case "NeuronStructure":
                    helpMessage = @"List number of neurons in following layers separating with semicolon.
For example: 2;3;4 stands for two neurons in input layer, three in next one and four in last one.";
                    break;
                case "IsUnipolar":
                    helpMessage = @"Set if transition function is unipolar (0 or 1 as a result) or bipolar (-1 or 1 as a result).";
                    break;
                case "IsBiased":
                    helpMessage = @"Set if every neuron has always one additional input with signal value 1 or no.";
                    break;
                default:
                    helpMessage = @"No help";
                    break;
            }

            MessageBox.Show(
                helpMessage,
                "Help",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void UpdateTitle()
        {
            Title = String.Format("MLP Basic by Kachniarz and Luśtyk {0}", DateTime.Now);
        }

        private void usePca_Click(object sender, RoutedEventArgs e)
        {
            if (maxColumns == null) return;

            maxColumns.IsEnabled = usePca.IsChecked == true;
        }
    }
}
