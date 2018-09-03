using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeLiveBrowser.Models
{
	public class DataBaseAccess
	{
		public SqlConnection SQLConnection { get; set; }

		public DataBaseAccess()
		{
			//接続文字列の設定
			SQLConnection.ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];


		}
	}
}
