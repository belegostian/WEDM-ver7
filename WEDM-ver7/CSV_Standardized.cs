using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEDM_ver7
{
    public class CustomArray<T>
    {
        public T[] GetColumn(T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public T[] GetRow(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }
    }

    class CSV_Standardized
    {
        public double[,] train_x { get; private set; }
        public double[] train_y { get; private set; }
        public double[,] test_x { get; private set; }
        public double[] test_y { get; private set; }
        public double[] averange_x { get; private set; }
        public double averange_y { get; private set; }
        public double[] deviation_x { get; private set; }
        public double deviation_y { get; private set; }



        static double standardDeviation(IEnumerable<double> sequence)
        {
            double result = 0;

            if (sequence.Any())
            {
                double average = sequence.Average();
                double sum = sequence.Sum(d => Math.Pow(d - average, 2));
                result = Math.Sqrt((sum) / sequence.Count());
            }
            return result;
        }

        public void calculate()
        {
            //資料讀取
            double[,] data_x = new double[162, 23];
            double[] data_y = new double[162];

            using (var reader = new StreamReader(@"C:\Users\user\source\repos\WEDM-ver7\WEDM-ver7\FeatureData.csv"))
            {
                for (int i = 0; !reader.EndOfStream; i++)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (i == 0)
                    {
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < values.Count() - 1; j++)
                        {
                            data_x[i - 1, j] = double.Parse(values[j], CultureInfo.InvariantCulture.NumberFormat);
                        }
                        data_y[i - 1] = double.Parse(values[23], CultureInfo.InvariantCulture.NumberFormat);
                    }
                }
            }

            // 翻轉陣列
            double[,] tmp_x = new double[23, 162];

            for (int i = 0; i < 23; i++)
            {
                for (int j = 0; j < 162; j++)
                {
                    tmp_x[i, j] = data_x[j, i];
                }
            }




            // 平均
            double[] avg_x = new double[23];
            double avg_y;

            CustomArray<double> customarray = new CustomArray<double>();
            for (int i = 0; i < 23; i++)
            {
                avg_x[i] = customarray.GetRow(tmp_x, i).Average();
            }
            avg_y = data_y.Average();

            // 標準差
            double[] std_x = new double[23];
            double std_y = 0;

            for (int i = 0; i < 23; i++)
            {
                std_x[i] = standardDeviation(customarray.GetRow(tmp_x, i));
            }
            std_y = standardDeviation(data_y);




            // 標準化
            for (int j = 0; j < 162; j++)
            {
                for (int i = 0; i < 23; i++)
                {
                    tmp_x[i, j] = (tmp_x[i, j] - avg_x[i]) / std_x[i];
                }
                data_y[j] = (data_y[j] - avg_y) / std_y;
            }

            // 翻轉陣列回復，順便切分
            double[,] x = new double[130, 23]; //
            double[] y = new double[130];
            double[,] valid_x = new double[32, 23]; //
            double[] valid_y = new double[32];

            for (int i = 0; i < 130; i++)
            {
                for (int j = 0; j < 23; j++) //
                {
                    x[i, j] = tmp_x[j, i];
                }
                y[i] = data_y[i];
            }
            for (int i = 130; i < 162; i++)
            {
                for (int j = 0; j < 23; j++) //
                {
                    valid_x[i - 130, j] = tmp_x[j, i];
                }
                valid_y[i - 130] = data_y[i];
            }


            // 傳值
            averange_x = avg_x;
            averange_y = avg_y;
            deviation_x = std_x;
            deviation_y = std_y;
            train_x = x;
            train_y = y;
            test_x = valid_x;
            test_y = valid_y;
        }
    }
}
