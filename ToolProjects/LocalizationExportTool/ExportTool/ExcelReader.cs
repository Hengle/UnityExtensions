using System;
using System.IO;
using ExcelDataReader;

namespace ExportTool
{
    struct ExcelReader
    {
        static string filePath;
        static IExcelDataReader reader;
        static int lineNumber;


        public static void ReadFile(string path, Action<string> readSheet)
        {
            using (var stream = File.Open(filePath = path, FileMode.Open, FileAccess.Read))
            {
                using (reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        lineNumber = 0;
                        readSheet(reader.Name);

                    } while (reader.NextResult());
                }
            }
        }


        public static int fieldCount => reader.FieldCount;


        public static bool ReadLine()
        {
            lineNumber++;
            return reader.Read();
        }


        public static float GetFloat(int i)
        {
            var type = reader.GetFieldType(i);

            if (type == typeof(double))
            {
                return (float)reader.GetDouble(i);
            }

            if (type == typeof(float))
            {
                return reader.GetFloat(i);
            }

            if (type == typeof(int))
            {
                return reader.GetInt32(i);
            }

            if (float.TryParse(reader.GetValue(i)?.ToString(), out float value))
            {
                return value;
            }

            throw Exception("Can't parse number from column " + (i + 1));
        }


        public static string GetString(int i)
        {
            return reader.GetValue(i)?.ToString();
        }


        public static string GetTrimmedString(int i)
        {
            var result = (reader.GetValue(i)?.ToString())?.Trim();

            if (string.IsNullOrEmpty(result))
                throw Exception("Can't get valid text from column " + (i + 1));

            return result;
        }


        public static bool GetYesNo(int i)
        {
            var text = GetTrimmedString(i).ToLower();
            if (text == "yes" || text == "true") return true;
            if (text == "no" || text == "false") return false;

            throw Exception("Can't get 'Yes' or 'No' from column " + (i + 1));
        }


        public static Exception Exception(string message)
        {
            return new Exception(message + "\nFile: " + filePath + ", Sheet: " + reader.Name + ", Line: " + lineNumber);
        }

    } // struct ExcelReader

} // namespace ExportTool