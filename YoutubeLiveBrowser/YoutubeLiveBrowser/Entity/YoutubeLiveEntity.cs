using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeLiveBrowser.Entity
{
	//DB保存用に余計な情報をカットしたクラス群
	public class Channels
	{
		public Channels(string channelId, string channelName)
		{
			ChannelId = channelId;
			ChannelName = channelName;
		}

		public string ChannelId { get; set; }
		public string ChannelName { get; set; }

		public override bool Equals(object obj)
		{
			var channel = (Channels)obj;
			return (channel.ChannelId == ChannelId);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	#region YoutubeLiveStreamInfo
	/// <summary>
	/// 配信の情報
	/// </summary>
	public class YoutubeLiveStreamInfo
	{
		/// <summary>
		/// キー項目だけ
		/// </summary>
		/// <param name="channelId"></param>
		/// <param name="videoId"></param>
		/// <param name="liveChatId"></param>
		public YoutubeLiveStreamInfo(string channelId, 
									 string videoId, 
									string liveChatId)
		{
			ChannelId	= channelId;
			VideoId		= videoId;
			LiveChatId	= liveChatId;
		}

		/// <summary>
		/// ライブ情報を新規作成するときに呼ぶ
		/// </summary>
		/// <param name="channelId"></param>
		/// <param name="channelName"></param>
		/// <param name="videoId"></param>
		/// <param name="liveChatId"></param>
		/// <param name="liveStreamStartDate"></param>
		/// <param name="liveStreamEndDate"></param>
		public YoutubeLiveStreamInfo(string   channelId, 
									 string   videoId, 
									 string   liveChatId, 
									 DateTime liveStreamStartDate, 
									 DateTime liveStreamEndDate)
		{
			ChannelId			= channelId;
			VideoId				= videoId;
			LiveChatId			= liveChatId;
			LiveStreamStartDate = liveStreamStartDate;
			LiveStreamEndDate	= liveStreamEndDate;
			LiveStreamTimeSpan	= liveStreamEndDate - liveStreamStartDate;

			Commnents = new List<YoutubeLiveComment>();
		}

		/// <summary>
		/// DBから読み込んだときはこっちを呼ぶ
		/// </summary>
		/// <param name="channelId"></param>
		/// <param name="videoId"></param>
		/// <param name="liveChatId"></param>
		/// <param name="liveStreamStartDate"></param>
		/// <param name="liveStreamEndDate"></param>
		/// <param name="liveStreamTimeSpan"></param>
		public YoutubeLiveStreamInfo(string   channelId,  
									 string   videoId, 
									 string	  liveChatId, 
									 DateTime liveStreamStartDate, 
									 DateTime liveStreamEndDate, 
									 TimeSpan liveStreamTimeSpan)
		{
			ChannelId			= channelId;
			VideoId				= videoId;
			LiveChatId			= liveChatId;
			LiveStreamStartDate = liveStreamStartDate;
			LiveStreamEndDate	= liveStreamEndDate;
			LiveStreamTimeSpan	= liveStreamTimeSpan;

			Commnents = new List<YoutubeLiveComment>();
		}

		public string	ChannelId			{ get; set; }   //チャンネルID
		public string	VideoId				{ get; set; }   //動画ID
		public string	LiveChatId			{ get; set; }	//配信中のチャットID
		public DateTime LiveStreamStartDate { get; set; }	//配信開始日時
		public DateTime LiveStreamEndDate	{ get; set; }	//配信終了日時
		public TimeSpan LiveStreamTimeSpan	{ get; set; }	//配信時間

		public List<YoutubeLiveComment> Commnents { get; set; }//コメント
	}
	#endregion

	#region YoutubeLiveComment
	/// <summary>
	/// コメント
	/// </summary>
	public class YoutubeLiveComment
	{
		public YoutubeLiveComment(
			string		commentId, 
			string		displayName,
			string		displayMessage,
			DateTime	publishedAt, 
			bool		isChatOwner, 
			bool		isChatSponsor, 
			bool		isChatModerator)
		{
			CommentId		= commentId;
			DisplayName		= displayName;
			DisplayMessage	= displayMessage;
			PublishedAt		= publishedAt;
			IsChatOwner		= isChatOwner;
			IsChatSponsor	= isChatSponsor;
			IsChatModerator = isChatModerator;
		}

		public string	CommentId		{ get; set; }   //コメントID
		public string	DisplayName		{ get; set; }	//コメントした人の名前
		public string	DisplayMessage	{ get; set; }   //コメント
		public DateTime PublishedAt		{ get; set; }   //コメントされた日時
		public bool		IsChatOwner		{ get; set; }	//配信者コメントか
		public bool		IsChatSponsor	{ get; set; }	//スポンサーか
		public bool		IsChatModerator { get; set; }	//モデレータか(スパナ付きか)
	}
	#endregion
}
