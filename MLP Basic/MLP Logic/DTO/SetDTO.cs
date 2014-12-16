using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MLP_Logic.DTO
{
    public class SetDTO : BaseDTO
    {
        public List<Dictionary<string, double>> Examples { get { return examples; } }
        public List<string> Keys { get { return keys; } }

        private string filename;
        private List<Dictionary<string, double>> examples;
        private List<string> keys;

        public SetDTO(string filename)
        {
            try
            {
                this.filename = filename;

                this.examples = new List<Dictionary<string, double>>();
                this.keys = new List<string>();

                var reader = new StreamReader(File.OpenRead(filename));

                string line = reader.ReadLine();
                string[] values = line.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    keys.Add(values[i]);
                }

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    values = line.Split(',');

                    Dictionary<string, double> dictionary = new Dictionary<string, double>();

                    for (int i = 0; i < values.Length; i++)
                    {
                        double value;
                        if (!double.TryParse(values[i], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out value))
                        {
                            throw new FormatException("Format invalid");
                        }
                        dictionary.Add(keys[i], value);
                    }

                    this.examples.Add(dictionary);
                }
            }
            catch(Exception exception)
            {
                exceptions.Add(String.Format("{0}", exception.Message));
            }
        }

        
    }
}
