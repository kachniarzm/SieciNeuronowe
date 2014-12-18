using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MLP_Data.Attributes;

namespace MLP_Data
{
    public class TestCase
    {
        private double[] _input;
        private double[] _output;
        private double[] _outputInPreviousCase;
        private List<object> _inputRawData;
        private object _outputRawData;

        public double[] Input
        {
            get { return _input; }
        }

        public double[] Output
        {
            get { return _output; }
        }

        public double[] OutputInPreviousCase
        {
            get { return _outputInPreviousCase; }
        }

        public List<object> InputRawData 
        {
            get { return _inputRawData; }
            set { _inputRawData = value; CalculateInput(); }
        }

        public object OutputRawData
        {
            get { return _outputRawData; }
            set { _outputRawData = value; CalculateOutput(); }
        }

        private void CalculateInput()
        {
            Type type = _inputRawData.FirstOrDefault().GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var inputList = new List<double>();

            foreach (var item in _inputRawData)
            {
                if (properties.Length > 0)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (Attribute.IsDefined(properties[i], typeof(OscilatorAttribute)) && item != _inputRawData.First())
                            continue;

                        if (properties[i].PropertyType == typeof (double))
                        {
                            inputList.Add((double) properties[i].GetValue(item));
                        }
                    }
                }
                else
                {
                    inputList.Add(double.Parse(item.ToString()));
                }
            }

            _input = inputList.ToArray();

            var outputPoperty = properties.FirstOrDefault(x => x.Name == Configuration.OutputPropertyName);
            if (outputPoperty != null)
            {
                _outputInPreviousCase = new[] { (double)outputPoperty.GetValue(_inputRawData[0]) };
            }
            else
            {
                _outputInPreviousCase = new[] { double.Parse(_inputRawData[0].ToString()) };
            }     
        }

        private void CalculateOutput()
        {
            Type type = _outputRawData.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var outputPoperty = properties.FirstOrDefault(x => x.Name == Configuration.OutputPropertyName);
            if (outputPoperty != null)
            {
                _output = new[] { (double)outputPoperty.GetValue(_outputRawData) };
            }
            else
            {
                _output = new[] { double.Parse(_outputRawData.ToString()) };
            }     
        }
    }
}
