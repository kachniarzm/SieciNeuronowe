using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MLP_Data.Enums;

namespace MLP_Data
{
    public class CsvReader
    {
        public static List<object> GetData(IndexName indexName, Type type, bool hasHeaderLine = true)
        {
            if (indexName == IndexName.Undefined)
                throw new ArgumentException("indexName cannot be undefined");

            var fileFullPath = GetFileFullPath(indexName.ToString());

            var result = new List<object>();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using (var reader = new StreamReader(File.OpenRead(fileFullPath)))
            {
                if (hasHeaderLine)
                    reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        string[] values = line.Split(';');
                        var entity = Activator.CreateInstance(type);

                        for (int i = 0; i < properties.Length && i < values.Length; i++)
                        {
                            var value = ParseValue(properties[i], values[i]);
                            properties[i].SetValue(entity, value, null);
                        }

                        var indexNameProperty = properties.FirstOrDefault(x => x.Name == Configuration.StockIndexPropertyName);
                        if (indexNameProperty != null)
                            indexNameProperty.SetValue(entity, indexName, null);

                        result.Add(entity);
                    }
                }
            }

            return result;
        }

        private static object ParseValue(PropertyInfo property, string value)
        {
            if (property.PropertyType == typeof(double))
            {
                value = value.Replace("%", String.Empty);
                return double.Parse(value);
            }
            if (property.PropertyType == typeof(int))
            {
                return int.Parse(value);
            }
            if (property.PropertyType == typeof(string))
            {
                return value;
            }
            if (property.PropertyType == typeof(DateTime))
            {
                return DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
            if (property.PropertyType == typeof (IndexName))
            {
                return IndexName.Undefined;
            }
            throw new FormatException();
        }

        private static string GetFileFullPath(string filename)
        {
            var directory = Directory.GetParent(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetDirectories().ToList().Exists(x => x.Name == Configuration.TestDataFolderName))
            {
                directory = directory.Parent;
            }

            if (directory == null)
                throw new FileNotFoundException(String.Format("Cannot find '{0}' folder with data", Configuration.TestDataFolderName));

            directory = directory.GetDirectories().ToList().SingleOrDefault(x => x.Name == Configuration.TestDataFolderName);

            // ReSharper disable once PossibleNullReferenceException
            var filedInfo = directory.GetFiles().ToList().SingleOrDefault(x => x.Name == filename + ".csv");
            if (filedInfo == null)
                throw new FileNotFoundException(String.Format("Cannot find '{0}.csv' file with data", filename));

            return filedInfo.FullName;
        }
    }
}
