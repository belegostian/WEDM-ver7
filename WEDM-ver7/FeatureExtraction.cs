using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEDM_ver7
{
    class FeatureExtraction
    {
        // 放電電壓區間序位配
        private List<int[]> VoltagePeakVelly { get; set; }
        // 平均單發間隙電壓
        private List<double> GapVoltAvg { get; set; }

        // [特] 放電區間數
        public int ExecuteCount { get; private set; }
        // [特] 正常放電發數
        public int NormalExecute { get; private set; }
        // [特] 異常放電發數
        public int AbnormalExecute { get; private set; }

        // [特] 平均間隙電壓 Gap_voltage_average
        public double GapVoltageAverage(List<double[]> cur_peak, List<double[]> volt_peak_velly, List<double> raw_volt, double depress_ratio = 500)
        {
            // [特] 平均單發間隙電壓
            List<double> gap_volt_avg = new List<double>();

            // [暫] 放電區間
            List<int[]> interval_list = new List<int[]>();
            int[] interval = new int[2];
            // [暫] V 序位
            int tmp_index = 0;
            // [暫] V 基準閥值
            double Baseline;
            // [暫] V 區間電壓總和
            double Sum = 0;

            // 找 I-峰值 是否存在對應的 V-放電區間，若有則此放電區間為有效區間
            for (int i = 0; i < cur_peak.Count; i++)
            {
                // i 與 j 值是相關的
                for (int j = tmp_index; j < volt_peak_velly.Count - 1; j++)
                {
                    // i 已錯過配對區間
                    if (cur_peak[i][0] < volt_peak_velly[j][0])
                    {
                        break;
                    }

                    // 序位配對
                    if (volt_peak_velly[j][0] <= cur_peak[i][0] && cur_peak[i][0] < volt_peak_velly[j + 1][0])
                    {
                        // 此為 峰-谷 對
                        if (volt_peak_velly[j][1] > 0)
                        {
                            //找到下一個谷值    --->    應該還可以優化
                            for (int k = j; k < volt_peak_velly.Count; k++)
                            {
                                if (volt_peak_velly[k][1] < (-70 / depress_ratio))
                                {
                                    interval[0] = (int)volt_peak_velly[j][0];
                                    interval[1] = (int)volt_peak_velly[k][0];
                                    interval_list.Add((int[])interval.Clone());
                                    break;
                                }
                            }
                        }
                        // 此為 谷-峰 對
                        if (volt_peak_velly[j][1] < 0)
                        {
                            for (int k = j; k < volt_peak_velly.Count; k++)
                            {
                                if (volt_peak_velly[k][1] > (37 / depress_ratio))
                                {
                                    interval[0] = (int)volt_peak_velly[j][0];
                                    interval[1] = (int)volt_peak_velly[k][0];
                                    interval_list.Add((int[])interval.Clone());
                                    break;
                                }
                            }
                        }
                    }
                    tmp_index++;
                }
            }

            // 取放電區間平均電壓
            for (int i = 0; i < interval_list.Count; i++)
            {
                // 峰-谷 對
                if (raw_volt[interval_list[i][0]] > raw_volt[interval_list[i][1]])
                {
                    // 谷值為基準
                    Baseline = raw_volt.Skip(interval_list[i][0]).Take(interval_list[i][1]).ToArray().Min();
                    // 取得區間內原始數據與基準之差值和
                    for (int j = interval_list[i][0]; j < interval_list[i][1]; j++)
                    {
                        Sum = raw_volt[j] - Baseline;
                    }
                    gap_volt_avg.Add(Sum * depress_ratio / (interval_list[i][1] - interval_list[i][0]));
                }

                // 谷-峰 對
                if (raw_volt[interval_list[i][0]] < raw_volt[interval_list[i][1]])
                {
                    Baseline = raw_volt.Skip(interval_list[i][0]).Take(interval_list[i][1]).ToArray().Max();
                    for (int j = interval_list[i][0]; j < interval_list[i][1]; j++)
                    {
                        Sum = Baseline - raw_volt[j];
                    }
                    gap_volt_avg.Add(Sum * depress_ratio / (interval_list[i][1] - interval_list[i][0]));
                }
            }

            VoltagePeakVelly = interval_list;
            ExecuteCount = interval_list.Count();
            GapVoltAvg = gap_volt_avg;
            return gap_volt_avg.Average();
        }

        // [特] 均放電時長 Ton_point
        public double TOnAverage(List<double[]> cur_peak, int pre_ton)
        {
            // [特] 均放電時長
            double t_on_avg;

            // [暫] 放電值/=0者
            int t_on_point;

            // 引用RawDataProcess TOn，與每組放電之始/末點
            t_on_point = pre_ton + (cur_peak.Count() * 2);
            // 平均一發?毫秒 (*4是因為取樣頻率125K)
            t_on_avg = ((double)t_on_point / (double)cur_peak.Count()) * 4;

            return t_on_avg;
        }

        // [特] 區間內放電數分布 Normal/Abnormal_discharge_distribution
        public List<int> DischargeRecord(List<double[]> cur_peak)
        {
            // [特] 放電 Normal/Abnormal_discharge
            Dictionary<int, int> discharge_record = new Dictionary<int, int>();
            List<int> record = new List<int>();

            // [暫] I 序位
            int tmp_index = 0;
            // [暫] I 發數
            int peak_count = 0;

            // 在放電區間中
            for (int i = 0; i < this.VoltagePeakVelly.Count(); i++)
            {
                //計算每個區間的電流峰值發數
                for (int j = tmp_index; j < cur_peak.Count(); j++)
                {
                    if (cur_peak[j][0] > this.VoltagePeakVelly[i][0] && cur_peak[j][0] < this.VoltagePeakVelly[i][1])
                    {
                        peak_count++;
                    }
                    else if (cur_peak[j][0] > this.VoltagePeakVelly[i][1])
                    {
                        tmp_index = j;
                        break;
                    }
                }

                //紀錄發數
                if (!discharge_record.ContainsKey(peak_count))
                {
                    discharge_record[peak_count] = 1;
                }
                else
                {
                    discharge_record[peak_count]++;
                }
                peak_count = 0;
            }

            for (int i = 0; i <= discharge_record.Keys.Max(); i++)
            {
                if (discharge_record.ContainsKey(i))
                {
                    record.Add(discharge_record.GetValueOrDefault(i));
                }
                else
                {
                    record.Add(0);
                }
            }            // record = discharge_record.Select(kvp => kvp.Key).ToList();

            return record;
        }

        // [特] 正異常發數比 Abnormal_ratio
        public double AbnormalRatio(List<int> discharge_record)
        {
            double ratio;

            int normal = discharge_record[1];
            int total = 0;

            for (int i = 0; i < discharge_record.Count; i++)
            {
                if (i == 1)
                {
                    continue;
                }
                total += discharge_record[i];
            }
            ratio = (double)normal / (double)total;

            NormalExecute = normal;
            AbnormalExecute = total - normal;
            return ratio;
        }

        // [特] 區間平均能量 Energy_average
        public double EnergyAverage(List<double> raw_cur)
        {
            // 單發平均能量
            List<double> avg_energy = new List<double>();


            for (int i = 0; i < this.VoltagePeakVelly.Count; i++)
            {
                double tmp_sum_current = 0;
                for (int j = VoltagePeakVelly[i][0]; j <= VoltagePeakVelly[i][1]; j++)
                {
                    tmp_sum_current = tmp_sum_current + raw_cur[j];
                }

                double avg_current = tmp_sum_current / (VoltagePeakVelly[i][1] - VoltagePeakVelly[i][0]);
                avg_energy.Add(avg_current * this.GapVoltAvg[i]);
            }

            return avg_energy.Average();
        }

        // [特] 大中小電流發數 L/M/S_current_count
        public int[] CurSizeCount(List<double[]> cur_peak)
        {
            int[] lms = new int[3];
            int l = 0, m = 0, s = 0;
            for (int i = 0; i < cur_peak.Count(); i++)
            {
                if (cur_peak[i][1] > 2)
                {
                    l++;
                }
                else if (cur_peak[i][1] > 1)
                {
                    m++;
                }
                else
                {
                    s++;
                }
            }

            lms[0] = l;
            lms[1] = m;
            lms[2] = s;

            return lms;
        }
    }
}
