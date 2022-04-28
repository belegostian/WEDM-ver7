using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEDM_ver7
{
    class DataAccess
    {
        public List<T> LoadData<T, U>(string sql, U parameters, string connectionstring)
        {
            //開啟(關閉)與資料庫的聯線
            using (IDbConnection connection = new MySqlConnection(connectionstring))
            {
                //分別傳入指令型式與指令參數
                List<T> rows = connection.Query<T>(sql, parameters).ToList();

                return rows;
            }

        }

        public void SaveData<T>(string sql, T parameters, string connectionstring)
        {
            using (IDbConnection connection = new MySqlConnection(connectionstring))
            {
                connection.Execute(sql, parameters);
            }

        }
    }
}
