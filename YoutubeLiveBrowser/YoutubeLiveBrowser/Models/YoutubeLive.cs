using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeLiveBrowser.Models
{
	class YoutubeLive
	{
		public static string APIPath = "https://accounts.google.com/o/oauth2/v2/auth?response_type=code";
		public static string ClientId = "681645994603-504ct38mbada3ltl3n8oe7g9bja6chnc.apps.googleusercontent.com";
		public static string ClientSecert = "iEk0qX2un9EKnee4y6DvXmlz";
		public static string RequestUrl = "http://localhost:8080";
		public static string TokenUrl = "https://www.googleapis.com/oauth2/v4/token";
		public static string APIKey = "AIzaSyAigMDKsNYQp6jkFEfb9KADSRuzum-61BI";

		public static string AuthUrl
		{
			get
			{
				string path = APIPath
							+ "&client_id=" + ClientId
							+ "&redirect_uri=" + RequestUrl
							+ "&scope=" + "https://www.googleapis.com/auth/youtube.readonly"
							+ "&access_type=" + "offline";
				return path;
			}
		}
	}

	//apiから取れる情報はすべて取っておく不要な情報を削って別に持っておく

	/// <summary>
	/// 
	/// </summary>
	public class AuthorDetails
	{

	}
	/// <summary>
	/// 
	/// </summary>
	public class Snippet
	{

	}

	/// <summary>
	/// コメント1個分の情報
	/// </summary>
	public class YoutubeLiveCommentDetail
	{
		public DateTime CommentDateTime;//コメントした時分秒
		public TimeSpan CommentRelativeTime;//配信内のコメント時間
		public string CommentId { get; set; }
		public string DisplayName { get; set; }//コメントしたユーザ名
		public string MessageText { get; set; }//コメント
		public bool isChatOwner { get; set; }//isowner
	}

	/// <summary>
	/// 配信の情報
	/// </summary>
	public class YoutubeLiveStreamInfo
	{
		public string ChannelId { get; set; }   //チャンネルID
		public string VideoId { get; set; }     //動画ID
		public string LiveChatId { get; set; }  //配信中のチャットID

		public List<YoutubeLiveCommentDetail> ChatComments { get; set; }    //配信中のコメント
	}
}
