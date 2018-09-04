using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeLiveBrowser.Entity;

namespace YoutubeLiveBrowser.Models
{
	public class DataBaseAccess
	{
		public SqlConnection SqlConnection { get; set; }
		public SqlTransaction SqlTransaction { get; set; }

		public DataBaseAccess()
		{
			//接続文字列の設定
			SqlConnection = new SqlConnection();
			SqlConnection.ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
			
			try
			{
				SqlConnection.Open();
			}
			catch (Exception e)
			{
				string err = e.Source + " : " + e.Message;
			}
			finally
			{

			}
		}

		#region チャンネル取得
		/// <summary>
		/// 登録しているチャンネルの情報
		/// </summary>
		/// <returns></returns>
		public List<Channels> GetChannels()
		{
			List<Channels> channels = new List<Channels>();
			var command = new SqlCommand();

			try
			{
				command.Connection = SqlConnection;
				command.CommandText = @"SELECT * FROM Channels";

				var reader = command.ExecuteReader();
				Dictionary<string, string> datas = new Dictionary<string, string>();

				while (reader.Read())
				{
					string channelId = (string)reader["ChannelId"];
					string channelName = (string)reader["ChannelName"];

					Channels channel = new Channels(channelId, channelName);

					channels.Add(channel);
				}

				return channels;
			}
			catch(SqlException sqlEx)
			{
				string srr = sqlEx.StackTrace;

				SqlConnection.Close();  //閉じて
				SqlConnection.Dispose();//解放する
			}
			finally
			{
				SqlConnection.Close();	//閉じて
				//SQLConnection.Dispose();//解放する
			}
			return null;
		}
		#endregion

		#region youtubeライブ情報取得
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public List<YoutubeLiveStreamInfo> GetYoutubeLiveStreamInfo()
		{
			List<YoutubeLiveStreamInfo> streamInfos = new List<YoutubeLiveStreamInfo>();
			var command = new SqlCommand();

			if(SqlConnection.State == ConnectionState.Closed)
			{
				SqlConnection.Open();
			}

			try
			{
				command.Connection = SqlConnection;
				command.CommandText = @"SELECT * FROM YoutubeLiveStreamInfo";

				var reader = command.ExecuteReader();

				while (reader.Read())
				{
					string	 videoId				= (string)reader["VideoId"];
					string	 channelId				= (string)reader["ChannelId"];
					string	 liveChatId				= (string)reader["LiveChatId"];
					DateTime liveStreamStartDate	= (DateTime)reader["LiveStreamStartDate"];
					DateTime liveStreamEndDate		= (DateTime)reader["LiveStreamEndDate"];
					int		 liveStreamTimeSpan		= (int)reader["LiveStreamTimeSpan"];

					TimeSpan span = new TimeSpan(liveStreamTimeSpan);

					YoutubeLiveStreamInfo streamInfo = new YoutubeLiveStreamInfo(videoId,
																				 channelId,
																				 liveChatId,
																				 liveStreamStartDate,
																				 liveStreamEndDate,
																				 span);

					streamInfos.Add(streamInfo);
				}

				return streamInfos;
			}
			catch (SqlException sqlEx)
			{
				string srr = sqlEx.StackTrace;

				SqlConnection.Close();  //閉じて
				SqlConnection.Dispose();//解放する
			}
			finally
			{
				SqlConnection.Close();	//閉じて
				//SQLConnection.Dispose();//解放する
			}
			return null;
		}
		#endregion

		#region youtubeライブコメント取得
		/// <summary>
		/// 動画ごとにコメントを取得
		/// </summary>
		/// <returns></returns>
		private const string SelectSQL = @"SELECT * FROM YoutubeLiveComment where LiveChatId = @LiveChatId and LiveChatId = @LiveChatId";
		public List<YoutubeLiveComment> GetYoutubeLiveComments(YoutubeLiveStreamInfo streamInfo)
		{
			List<YoutubeLiveComment> comments = new List<YoutubeLiveComment>();
			var command = new SqlCommand();

			if (SqlConnection.State == ConnectionState.Closed)
			{
				SqlConnection.Open();
			}

			try
			{
				command.Connection = SqlConnection;
				command.CommandText = SelectSQL;

				command.Parameters.AddWithValue("@VieoId", streamInfo.VideoId);
				command.Parameters.AddWithValue("@LiveChatId", streamInfo.LiveChatId);

				var reader = command.ExecuteReader();

				while (reader.Read())
				{
					string	 videoId			= (string)reader["VideoId"];
					string	 liveChatId			= (string)reader["LiveChatId"];
					string	 commentId			= (string)reader["CommentId"];
					string	 displayName		= (string)reader["DisplayName"];
					string	 displayMessage		= (string)reader["DisplayMessage"];
					DateTime publishedAt		= (DateTime)reader["PublishedAt"];
					bool	 isChatOwner		= (bool)reader["IsChatOwner"];
					bool	 isChatSponsor		= (bool)reader["IsChatSponsor"];
					bool	 isChatModerator	= (bool)reader["IsChatModerator"];

					YoutubeLiveComment comment = new YoutubeLiveComment(commentId,
																		   displayName,
																		   displayMessage,
																		   publishedAt,
																		   isChatOwner,
																		   isChatSponsor,
																		   isChatModerator);
					
					comments.Add(comment);
				}

				comments = comments.OrderBy(x => x.PublishedAt).ThenBy(x => x.CommentId).ToList();
				streamInfo.Commnents.AddRange(comments);

				return comments;
			}
			catch (SqlException sqlEx)
			{
				string srr = sqlEx.StackTrace;

				SqlConnection.Close();  //閉じて
				SqlConnection.Dispose();//解放する
			}
			finally
			{
				//SQLConnection.Close();	//閉じて
				//SQLConnection.Dispose();//解放する
			}
			return null;
		}
		#endregion
	}
}
