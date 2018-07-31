using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeLiveBrowser.Models
{
	public interface IYoutubeApiResponse
	{
		string Kind { get; set; } //レスポンスの種類
		string Etag { get; set; } //etag 謎
	}
	//apiから取れる情報はすべて取っておく

	public class TextMessageDetails
	{
		public string MessageText { get; set; }
	}

	#region Snippet
	/// <summary>
	/// Snippet
	/// </summary>
	public class Snippet
	{
		#region コンストラクタ
		public Snippet(string type, 
			string liveChatId, 
			string authorChannelId, 
			DateTime publishedAt, 
			bool hasDisplayContent, 
			string displayMessage, 
			TextMessageDetails textMessageDetails)
		{
			Type = type;
			LiveChatId = liveChatId;
			AuthorChannelId = authorChannelId;
			PublishedAt = publishedAt;
			HasDisplayContent = hasDisplayContent;
			DisplayMessage = displayMessage;
			TextMessageDetails = textMessageDetails;
		}
		#endregion

		public string Type { get; set; }
		public string LiveChatId { get; set; }//ライブチャットID
		public string AuthorChannelId { get; set; }//コメントした人のチャンネルID
		public DateTime PublishedAt { get; set; }//コメントされた日時
		public bool HasDisplayContent { get; set; }//コメントが表示されているか
		public string DisplayMessage { get; set; }//コメント
		public TextMessageDetails TextMessageDetails { get; set; }
	}
	#endregion

	/// <summary>
	/// 
	/// </summary>
	public class AuthorDetails
	{
		#region コンストラクタ
		public AuthorDetails(string channelId, 
			string displayName, 
			string profileImageUrl, 
			bool isVerified, 
			bool isChatOwner, 
			bool isChatSponsor, 
			bool isChatModerator)
		{
			ChannelId = channelId;
			DisplayName = displayName;
			ProfileImageUrl = profileImageUrl;
			IsVerified = isVerified;
			IsChatOwner = isChatOwner;
			IsChatSponsor = isChatSponsor;
			IsChatModerator = isChatModerator;
		}
		#endregion

		public string ChannelId { get; set; }//コメントした人のチャンネルID
		public string DisplayName { get; set; }//コメントしたユーザ名
		public string ProfileImageUrl { get; set; }//コメントした人のアイコンのURL
		public bool IsVerified { get; set; }
		public bool IsChatOwner { get; set; }//配信者か
		public bool IsChatSponsor { get; set; }//スポンサーか
		public bool IsChatModerator { get; set; }//モデレータか(スパナ付きか)
	}

	#region YoutubeLiveChatMessageItem
	/// <summary>
	/// コメント1個分の情報
	/// </summary>
	public class YoutubeLiveChatMessageItem : IYoutubeApiResponse
	{
		#region コンストラクタ
		public YoutubeLiveChatMessageItem(string kind, 
			string etag, 
			string id, 
			Snippet snippet, 
			AuthorDetails authorDetails)
		{
			Kind = kind;
			Etag = etag;
			Id = id;
			Snippet = snippet;
			AuthorDetails = authorDetails;
		}
		#endregion

		public string Kind { get; set; }
		public string Etag { get; set; }
		public string Id { get; set; } //コメントのID
		public Snippet Snippet { get; set; }
		public AuthorDetails AuthorDetails { get; set; }
	}
	#endregion

	#region PageInfo
	/// <summary>
	/// リクエスト1回で取得した情報の数
	/// </summary>
	public class PageInfo
	{
		public PageInfo(int totalResults, int resultsPerPage)
		{
			TotalResults = totalResults;
			ResultsPerPage = resultsPerPage;
		}

		public int TotalResults { get; set; }
		public int ResultsPerPage { get; set; }
	}
	#endregion

	#region YoutubeLiveChatMessageResponseItem 
	/// <summary>
	/// チャットメッセージ取得のリクエスト1回で取得する情報
	/// </summary>
	public class YoutubeLiveChatMessageResponseItem : IYoutubeApiResponse
	{
		#region コンストラクタ
		public YoutubeLiveChatMessageResponseItem(dynamic response)
		{
			PageInfo = new PageInfo((int)response.pageInfo.totalResults, (int)response.pageInfo.resultsPerPage);
		}

		public YoutubeLiveChatMessageResponseItem(string kind, 
			string etag, string nextPageToken, 
			int pollingIntervalMillis, 
			PageInfo pageInfo, 
			List<YoutubeLiveChatMessageItem> chatMessages)
		{
			Kind = kind;
			Etag = etag;
			NextPageToken = nextPageToken;
			PollingIntervalMillis = pollingIntervalMillis;
			PageInfo = pageInfo;
			ChatMessages = chatMessages;
		}
		#endregion

		public string Kind { get; set; }
		public string Etag { get; set; }
		public string NextPageToken { get; set; }
		public int PollingIntervalMillis { get; set; }//競合回避用のインターバル?(ms)
		public PageInfo PageInfo { get; set; }

		public List<YoutubeLiveChatMessageItem> ChatMessages { get; set; }

		public void AddChatMessageItem(string message)
		{

		}
	}
	#endregion

	#region YoutubeLiveStreamInfo あとで定義なおす
	/// <summary>
	/// 配信の情報
	/// </summary>
	public class YoutubeLiveStreamInfo
	{
		public string ChannelId { get; set; }   //チャンネルID
		public string VideoId { get; set; }     //動画ID
		public string LiveChatId { get; set; }  //配信中のチャットID

		//public List<YoutubeLiveCommentDetail> ChatComments { get; set; }    //配信中のコメント
	}
	#endregion
}
