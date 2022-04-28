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
            var module_folder = @"C:\Users\user\source\repos\WEDM-ver6\Keras";
            var module_name = "RunKeras";
            var class_name = "LoanModel";
            var def_name = "predict_this";

            //Model input arguments preparation
            var methodArguments = new PythonCallerArgs();

            methodArguments.AddArg("OV", 16);
            methodArguments.AddArg("Ton", 13);
            methodArguments.AddArg("Toff", 10);
            methodArguments.AddArg("Aon", 8);
            methodArguments.AddArg("Aoff", 8);
            methodArguments.AddArg("SV", 38);
            methodArguments.AddArg("WT", 8);
            methodArguments.AddArg("WF", 7);
            methodArguments.AddArg("WA", 8);
            methodArguments.AddArg("F", 8);
            methodArguments.AddArg("SG", 6);
            methodArguments.AddArg("A", 7);
            methodArguments.AddArg("T", 40);
            methodArguments.AddArg("Gap_Voltage_average", gap_volt_avg);
            methodArguments.AddArg("T_on", ton);
            methodArguments.AddArg("Normal_discharge_interval", normal_execute);
            methodArguments.AddArg("Abnormal_discharge_interval", abnormal_execute);
            methodArguments.AddArg("Execute", execute_count);
            methodArguments.AddArg("Ratio", abnormal_ratio);
            methodArguments.AddArg("Energy_average", energy_avg);
            methodArguments.AddArg("Small_current", cur_size_count[2]);
            methodArguments.AddArg("Middle_current", cur_size_count[1]);
            methodArguments.AddArg("Big_current", cur_size_count[0]);

            methodArguments.AddMetaArg("caller", "WindowsToPythonAI");

            // Now we can create a caller object and bind it to the model
            var pyCaller = new PythonCaller(module_folder, module_name);
            Dictionary<string, string> resultJson = pyCaller.CallClassMethod(class_name, def_name, methodArguments);

            // 計時結束
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);


            Console.WriteLine("prediction = : {0} ;", resultJson["prediction"]);
            Console.WriteLine("Run Time = : {0} ;", elapsedTime);
        }
    }
}
