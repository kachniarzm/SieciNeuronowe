using MLP_Data.Enums;
using MLP_Logic.Enums;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Basic.ViewModels
{
    class MainWindowModel:  INotifyPropertyChanged
    {
        private PlotModel specialCasePlotModel;
        public PlotModel SpecialCasePlotModel
        {
            get { return specialCasePlotModel; }
            set { specialCasePlotModel = value; OnPropertyChanged("SpecialCasePlotModel"); }
        }

        private PlotModel standardPlotModel;
        public PlotModel StandardPlotModel
        {
            get { return standardPlotModel; }
            set { standardPlotModel = value; OnPropertyChanged("StandardPlotModel"); }
        }


        private IndexName _selectedIndexName;
        public IndexName SelectedIndexName
        {
            get { return _selectedIndexName; }
            set
            {
                _selectedIndexName = value;
                OnPropertyChanged("SelectedIndexName");
            }
        }

        public IEnumerable<IndexName> IndexNameValues
        {
            get { return Enum.GetValues(typeof(IndexName)).Cast<IndexName>(); }
        }

        private InputDataDateUnits _selectedStep;
        public InputDataDateUnits SelectedStep
        {
            get { return _selectedStep; }
            set
            {
                _selectedStep = value;
                OnPropertyChanged("SelectedStep");
            }
        }
        public IEnumerable<InputDataDateUnits> StepValues
        {
            get { return Enum.GetValues(typeof(InputDataDateUnits)).Cast<InputDataDateUnits>(); }
        }

        private InputDataDateUnits _selectedDensity;
        public InputDataDateUnits SelectedDensity
        {
            get { return _selectedDensity; }
            set
            {
                _selectedDensity = value;
                OnPropertyChanged("SelectedDensity");
            }
        }
        public IEnumerable<InputDataDateUnits> DensityValues
        {
            get { return Enum.GetValues(typeof(InputDataDateUnits)).Cast<InputDataDateUnits>(); }
        }

        private InputDataDateUnits _selectedWindowLength;
        public InputDataDateUnits SelectedWindowLength
        {
            get { return _selectedWindowLength; }
            set
            {
                _selectedWindowLength = value;
                OnPropertyChanged("SelectedWindowLength");
            }
        }
        public IEnumerable<InputDataDateUnits> WindowLengthValues
        {
            get { return Enum.GetValues(typeof(InputDataDateUnits)).Cast<InputDataDateUnits>(); }
        }

        private NeuronNetworkType _selectedNeuronNetworkType;
        public NeuronNetworkType SelectedNeuronNetworkType
        {
            get { return _selectedNeuronNetworkType; }
            set
            {
                _selectedNeuronNetworkType = value;
                OnPropertyChanged("SelectedNeuronNetworkType");
            }
        }
        public IEnumerable<NeuronNetworkType> NeuronNetworkTypeValues
        {
            get { return Enum.GetValues(typeof(NeuronNetworkType)).Cast<NeuronNetworkType>(); }
        }

        public MainWindowModel()
        {
            SpecialCasePlotModel = new PlotModel();
            StandardPlotModel = new PlotModel();

            InitializePlot();
        }

        private void InitializePlot()
        {
            SpecialCasePlotModel.Title = "Result";
            StandardPlotModel.Title = "Result";
        }

        public event PropertyChangedEventHandler PropertyChanged;
 
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
