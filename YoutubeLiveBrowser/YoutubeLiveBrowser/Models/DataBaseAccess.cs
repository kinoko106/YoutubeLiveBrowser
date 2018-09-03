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
			
			try
			{
				SQLConnection.Open();
				var command = new SqlCommand();
				command.Connection = SQLConnection;
				command.CommandText = @"SELECT * FROM Channels";

				var reader = command.ExecuteReader();
				Dictionary<string, string> datas = new Dictionary<string, string>();

				while (reader.Read())
				{
					string channelId = (string)reader["ChannelId"];
					string channelName = (string)reader["ChannelName"];

					datas.Add(channelId, channelName);
				}
			}
			catch (Exception e)
			{
				string err = e.Source + " : " + e.Message;
			}
			finally
			{

			}
		}
	}
}
