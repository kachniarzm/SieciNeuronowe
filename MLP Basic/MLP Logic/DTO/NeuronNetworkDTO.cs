using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLP_Data.Enums;
using MLP_Logic.Enums;

namespace MLP_Logic.DTO
{
    public class NeuronNetworkDTO : BaseDTO
    {
        public List<int> NeuronsInLayer { get; private set; }
        public bool IsUnipolar { get; private set; }
        public bool IsBiased { get; private set; }
        public NeuronNetworkType NetworkType { get; set; }

        public NeuronNetworkDTO(string neuronStructure, bool isUnipolar, bool isBiased, NeuronNetworkType neuronNetworkType)
        {
            NeuronsInLayer = new List<int>();
            try
            {
                string[] raw = neuronStructure.Split(new char[] { ';' });
                for (int i = 0; i < raw.Length; i++)
                {
                    int data = int.Parse(raw[i]);
                    if (data <= 0)
                    {
                        throw new ArgumentException("Number of neurons in layer has to be a positive number");
                    }
                    NeuronsInLayer.Add(data);
                }
                NetworkType = neuronNetworkType;
                if (NetworkType == NeuronNetworkType.Undefined)
                {
                    exceptions.Add("NetworkType is undefined");
                }
               

            }
            catch(Exception exception)
            {
                exceptions.Add(String.Format("{0}", exception.Message));
            }

            IsUnipolar = isUnipolar;
            IsBiased = isBiased;
        }
    }


}
