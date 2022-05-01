using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WEDM_ver7
{
    class Program
    {
        static void Main(string[] args)
        {
            // SQL DB PART

            // 計時器
            Stopwatch stopWatch = new Stopwatch();

            // 原始資料取得
            DataAccess data_access = new DataAccess();
            List<double> raw_volt = new List<double>();
            List<double> raw_cur = new List<double>();


            string v_sql = "SELECT Voltage FROM `wedm_data_27`.`250k` LIMIT 125000;";
            string c_sql = "SELECT Current FROM `wedm_data_27`.`250k` LIMIT 125000;";


            // 計時開始
            stopWatch.Start();

            raw_volt = data_access.LoadData<double, dynamic>(v_sql, new { }, "server=127.0.0.1;uid=root;pwd=Yytsai33664488;database=wedm_data_27"); // Kurufinwe2022
            raw_cur = data_access.LoadData<double, dynamic>(c_sql, new { }, "server=127.0.0.1;uid=root;pwd=Yytsai33664488;database=wedm_data_27"); // Yytsai33664488

            // 資料預處理
            DataPreprocess preprocess = new DataPreprocess();
            List<double[]> cur_peak = new List<double[]>();
            List<double[]> volt_peak_velly = new List<double[]>();
            int pre_ton;

            cur_peak = preprocess.CurrentIndexOfPeak(raw_cur);
            volt_peak_velly = preprocess.VoltageIndexOfPeakAndVelly(raw_volt);
            pre_ton = preprocess.TOn;

            // 特徵提取
            FeatureExtraction feature_extraction = new FeatureExtraction();
            double gap_volt_avg;
            int execute_count;
            double ton;
            List<int> discharge_record = new List<int>();
            double abnormal_ratio;
            int normal_execute;
            int abnormal_execute;
            double energy_avg;
            int[] cur_size_count = new int[3];


            gap_volt_avg = feature_extraction.GapVoltageAverage(cur_peak, volt_peak_velly, raw_volt);
            execute_count = feature_extraction.ExecuteCount;
            ton = feature_extraction.TOnAverage(cur_peak, pre_ton);
            discharge_record = feature_extraction.DischargeRecord(cur_peak);
            abnormal_ratio = feature_extraction.AbnormalRatio(discharge_record);
            normal_execute = feature_extraction.ExecuteCount;
            abnormal_execute = feature_extraction.AbnormalExecute;
            energy_avg = feature_extraction.EnergyAverage(raw_cur);
            cur_size_count = feature_extraction.CurSizeCount(cur_peak);


            // 結果呈現
            Console.WriteLine("gap_volt_avg = {0}", gap_volt_avg);
            Console.WriteLine("execute_count = {0}", execute_count);
            Console.WriteLine("ton = {0}", ton);
            // Console.WriteLine("discharge_record[1] = {0}", discharge_record[1]);
            Console.WriteLine("abnormal_ratio = {0}", abnormal_ratio);
            Console.WriteLine("normal_execute = {0}", normal_execute);
            Console.WriteLine("abnormal_execute = {0}", abnormal_execute);
            Console.WriteLine("energy_avg = {0}", energy_avg);
            Console.WriteLine("cur_size_count L/M/S = {0}/{1}/{2}", cur_size_count[0], cur_size_count[1], cur_size_count[2]);






            //AI PART
            var module_folder = @"C:\Users\user\source\repos\WEDM-ver7\Keras";
            var module_name = "Keras";
            var class_name = "LoanModel";
            var def_name = "predict_this";

            //Model input arguments preparation
            var methodArguments = new PythonCallerArgs();
            CSV_Standardized std = new CSV_Standardized();
            std.calculate();

            methodArguments.AddArg("OV", ((16 - std.averange_x[0]) / std.deviation_x[0]));
            methodArguments.AddArg("Ton", ((13 - std.averange_x[1]) / std.deviation_x[1]));
            methodArguments.AddArg("Toff", ((10 - std.averange_x[2]) / std.deviation_x[2]));
            methodArguments.AddArg("Aon", ((8 - std.averange_x[3]) / std.deviation_x[3]));
            methodArguments.AddArg("Aoff", ((8 - std.averange_x[4]) / std.deviation_x[4]));
            methodArguments.AddArg("SV", ((38 - std.averange_x[5]) / std.deviation_x[5]));
            methodArguments.AddArg("WT", ((8 - std.averange_x[6]) / std.deviation_x[6]));
            methodArguments.AddArg("WF", ((7 - std.averange_x[7]) / std.deviation_x[7]));
            methodArguments.AddArg("WA", ((8 - std.averange_x[8]) / std.deviation_x[8]));
            methodArguments.AddArg("F", ((8 - std.averange_x[9]) / std.deviation_x[9]));
            methodArguments.AddArg("SG", ((6 - std.averange_x[10]) / std.deviation_x[10]));
            methodArguments.AddArg("A", ((7 - std.averange_x[11]) / std.deviation_x[11]));
            methodArguments.AddArg("T", ((40 - std.averange_x[12]) / std.deviation_x[12]));
            methodArguments.AddArg("Gap_Voltage_average", ((gap_volt_avg - std.averange_x[13]) / std.deviation_x[13]));
            methodArguments.AddArg("T_on", ((ton - std.averange_x[14]) / std.deviation_x[14]));
            methodArguments.AddArg("Normal_discharge_interval", ((normal_execute - std.averange_x[15]) / std.deviation_x[15]));
            methodArguments.AddArg("Abnormal_discharge_interval", ((abnormal_execute - std.averange_x[16]) / std.deviation_x[16]));
            methodArguments.AddArg("Execute", ((execute_count - std.averange_x[17]) / std.deviation_x[17]));
            methodArguments.AddArg("Ratio", ((abnormal_ratio - std.averange_x[18]) / std.deviation_x[18]));
            methodArguments.AddArg("Energy_average", ((energy_avg - std.averange_x[19]) / std.deviation_x[19]));
            methodArguments.AddArg("Small_current", ((cur_size_count[2] - std.averange_x[20]) / std.deviation_x[20]));
            methodArguments.AddArg("Middle_current", ((cur_size_count[1] - std.averange_x[21]) / std.deviation_x[21]));
            methodArguments.AddArg("Big_current", ((cur_size_count[0] - std.averange_x[22]) / std.deviation_x[22]));

            methodArguments.AddMetaArg("caller", "WindowsToPythonAI");

            // Now we can create a caller object and bind it to the model
            var pyCaller = new PythonCaller(module_folder, module_name);
            Dictionary<string, string> resultJson = pyCaller.CallClassMethod(class_name, def_name, methodArguments);

            // 計時結束
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine();
            Console.WriteLine("prediction = : {0} ;", ((Convert.ToDouble(resultJson["prediction"].Substring(2, resultJson["prediction"].Length - 4)) * std.deviation_y) + std.averange_y));
            Console.WriteLine("Run Time = : {0} ;", elapsedTime);
        }
    }
}
