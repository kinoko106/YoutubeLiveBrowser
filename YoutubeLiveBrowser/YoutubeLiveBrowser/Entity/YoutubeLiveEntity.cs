using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeLiveBrowser.Entity
{
	//DB保存用に余計な情報をカットしたクラス群

	#region YoutubeLiveStreamInfo
	/// <summary>
	/// 配信の情報
	/// </summary>
	public class YoutubeLiveStreamInfo
	{
		public string ChannelId { get; set; }   //チャンネルID
		public string VideoId { get; set; }     //動画ID
		public string LiveChatId { get; set; }  //配信中のチャットID

		public DateTime LivaStreamStartDate { get; set; }	//配信開始日時
		public DateTime LivaStreamEndDate { get; set; }		//配信終了日時
		public TimeSpan LivaStreamTimeSpan
		{
			get
			{
				return (LivaStreamEndDate - LivaStreamStartDate);//配信時間
			}
		}

		public Dictionary<string, YoutubeLiveComment> Commnents { get; set; }//コメント
	}
	#endregion

	#region YoutubeLiveComment
	/// <summary>
	/// コメント
	/// </summary>
	public class YoutubeLiveComment
	{
		public YoutubeLiveComment(
			string commentId, 
			DateTime publishedAt, 
			string comment, bool 
			isChatOwner, bool 
			isChatSponsor, 
			bool isChatModerator)
		{
			CommentId = commentId;
			PublishedAt = publishedAt;
			Comment = comment;
			IsChatOwner = isChatOwner;
			IsChatSponsor = isChatSponsor;
			IsChatModerator = isChatModerator;
		}

		public string CommentId { get; set; }		//コメントID
		public DateTime PublishedAt { get; set; }	//コメントされた日時
		public string Comment { get; set; }         //コメント
		public bool IsChatOwner { get; set; }		//配信者コメントか
		public bool IsChatSponsor { get; set; }		//スポンサーか
		public bool IsChatModerator { get; set; }	//モデレータか(スパナ付きか)
	}
	#endregion
}
