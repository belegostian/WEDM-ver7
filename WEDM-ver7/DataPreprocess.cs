using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEDM_ver7
{
    class DataPreprocess
    {
        // [I] 放電發數
        public int TOn { get; private set; }

        // [I] 取 峰值    --->    I/O:原始電流/<[序位,峰值]>
        public List<double[]> CurrentIndexOfPeak(List<double> raw_cur)
        {
            // 峰序位, 峰值
            List<double[]> peak = new List<double[]>();
            double[] pair = new double[2];

            // (暫) I值
            double tmp_value = 0;
            // (暫) 狀態
            string state = "cease";
            // (暫) 雜訊時間
            int tmp_toff = 0;

            for (int i = 0; i < raw_cur.Count(); i++)
            {
                // 濾除雜訊
                if (raw_cur[i] < 0.02)                            // 0.02為經驗閥值
                {
                    raw_cur[i] = 0;
                    tmp_toff++;
                }

                switch (state)
                {
                    case "cease":
                        // 區間起始
                        if (raw_cur[i] > 0)
                        {
                            tmp_value = raw_cur[i];
                            state = "discharge";
                        }
                        break;
                    case "discharge":
                        // I陡升
                        if (raw_cur[i] > tmp_value)
                        {
                            tmp_value = raw_cur[i];
                        }
                        // 區間結束
                        else if (raw_cur[i] == 0)
                        {
                            pair[0] = i;
                            pair[1] = tmp_value;
                            peak.Add((double[])pair.Clone());

                            tmp_value = 0;
                            state = "cease";
                        }
                        break;
                }
            }

            TOn = raw_cur.Count() - tmp_toff;
            return peak;
        }

        // [V] 取 峰值, 谷值 (合併、排序、刪去異常值)    --->    I/O:原始電壓/<[序位,極值]>
        public List<double[]> VoltageIndexOfPeakAndVelly(List<double> raw_volt, int evaluation_interval = 500, double depress_ratio = 500, double noise_valve = 0)
        {
            // 峰, 谷, 峰序-峰值, 谷序-谷值
            List<double[]> PeakAndVelly = new List<double[]>();
            double[] peak = new double[2];
            double[] velly = new double[2];

            // (暫) 峰閥值, 谷閥值
            peak[1] = double.NegativeInfinity;
            velly[1] = double.PositiveInfinity;

            // 逐筆找峰/谷值
            for (int i = 0; i < raw_volt.Count(); i++)
            {
                // 以 V值 為 新峰值
                if (raw_volt[i] > peak[1])
                {
                    peak[0] = i;
                    peak[1] = raw_volt[i];
                }
                else if (raw_volt[i] < velly[1])
                {
                    velly[0] = i;
                    velly[1] = raw_volt[i];
                }

                // 之後，若 V值 接近 峰值閥，則有機會為local峰值，也可能是下降起始
                if (raw_volt[i] < peak[1] - noise_valve && peak[1] != double.PositiveInfinity)
                {
                    //於 峰值判斷區間 內，無值 > 原峰值，可為峰值候選
                    if (raw_volt.Skip(i).Take(evaluation_interval).ToArray().Max() < peak[1])
                    {
                        if (peak[1] >= (double)(37 / depress_ratio))    // 37 為經驗閥值
                        {
                            PeakAndVelly.Add((double[])peak.Clone());

                            // 轉而尋找谷值
                            peak[1] = double.PositiveInfinity;
                            velly[1] = double.PositiveInfinity;
                        }
                    }
                }

                if (raw_volt[i] > velly[1] + noise_valve && velly[1] != double.NegativeInfinity)
                {
                    if (raw_volt.Skip(i).Take(evaluation_interval).ToArray().Min() < velly[1])
                    {
                        if (velly[1] < (double)(-73 / depress_ratio))
                        {
                            PeakAndVelly.Add((double[])velly.Clone());

                            peak[1] = double.NegativeInfinity;
                            velly[1] = double.NegativeInfinity;
                        }
                    }
                }

                // 若無預留 峰值判斷區間 時，結束迴圈
                if (i + evaluation_interval >= raw_volt.Count())
                {
                    break;
                }
            }

            return PeakAndVelly;
        }
    }
}
