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

	//apiで取ってきた生の情報

	#region TextMessageDetails
	/// <summary>
	/// コメントの詳細
	/// </summary>
	public class TextMessageDetails
	{
		public TextMessageDetails(string messageText)
		{
			MessageText = messageText;
		}

		public string MessageText { get; set; }
	}
	#endregion

	#region Snippet
	/// <summary>
	/// Snippet
	/// </summary>
	public class Snippet
	{
		#region コンストラクタ
		public Snippet(dynamic inSnippet)
		{
			Type = (string)inSnippet.type;
			LiveChatId = (string)inSnippet.liveChatId;
			AuthorChannelId = (string)inSnippet.authorChannelId;
			PublishedAt = DateTime.Parse(inSnippet.publishedAt);
			HasDisplayContent = inSnippet.hasDisplayContent;
			DisplayMessage = (string)inSnippet.displayMessage;
			TextMessageDetails = new TextMessageDetails(inSnippet.textMessageDetails.messageText);
		}
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

	#region AuthorDetails
	/// <summary>
	/// AuthorDetails
	/// </summary>
	public class AuthorDetails
	{
		#region コンストラクタ
		public AuthorDetails(dynamic inAuthorDetails)
		{
			ChannelId = (string)inAuthorDetails.channelId;
			DisplayName = (string)inAuthorDetails.displayName;
			ProfileImageUrl = (string)inAuthorDetails.profileImageUrl;
			IsVerified = inAuthorDetails.isVerified;
			IsChatOwner = inAuthorDetails.isChatOwner;
			IsChatSponsor = inAuthorDetails.isChatSponsor;
			IsChatModerator = inAuthorDetails.isChatModerator;
		}
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
	#endregion

	#region YoutubeLiveChatMessageItem
	/// <summary>
	/// コメント1個分の情報
	/// </summary>
	public class YoutubeLiveChatMessageItem : IYoutubeApiResponse
	{
		#region コンストラクタ
		public YoutubeLiveChatMessageItem(dynamic inYoutubeLiveChatMessageItem)
		{
			try
			{
				Kind = inYoutubeLiveChatMessageItem.kind;
				Etag = inYoutubeLiveChatMessageItem.etag;
				Id = inYoutubeLiveChatMessageItem.id;
				Snippet = new Snippet(inYoutubeLiveChatMessageItem.snippet);
				AuthorDetails = new AuthorDetails(inYoutubeLiveChatMessageItem.authorDetails);
			}
			catch(Exception e)
			{
				string errorMessage = e.Message;
				string source = e.Source;
				string stacktrace = e.StackTrace;
			}
		}
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

		//参照用プロパティ
		public string DisplayName { get { return AuthorDetails.DisplayName; } }         //コメントした人の名前
		public DateTime PublishedAt { get { return Snippet.PublishedAt; } }				//コメントされた時間
		public string DisplayMessage { get { return Snippet.DisplayMessage; } }			//コメント
		public bool IsChatOwner { get { return AuthorDetails.IsChatOwner; } }			//配信者コメントか
		public bool IsChatSponsor { get { return AuthorDetails.IsChatSponsor; } }		//スポンサーか
		public bool IsChatModerator { get { return AuthorDetails.IsChatModerator; } }	//モデレータか(スパナ付きか)
	}
	#endregion

	#region PageInfo
	/// <summary>
	/// リクエスト1回で取得した情報の数
	/// </summary>
	public class PageInfo
	{
		public PageInfo(dynamic inPageInfo)
		{
			TotalResults = (int)inPageInfo.totalResults;
			ResultsPerPage = (int)inPageInfo.resultsPerPage;
		}

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
		//リクエストで受け取ったものをそのままぶち込むとき
		public YoutubeLiveChatMessageResponseItem(dynamic response)
		{
			Kind = (string)response.kind;
			Etag = (string)response.etag;
			NextPageToken = (string)response.nextPageToken;
			PollingIntervalMillis = (int)response.pollingIntervalMillis;
			PageInfo = new PageInfo(response.pageInfo);

			ChatMessages = new Dictionary<string, YoutubeLiveChatMessageItem>();
			foreach (dynamic value in response.items)
			{
				ChatMessages.Add(value.id ,new YoutubeLiveChatMessageItem(value));
			}
		}

		public YoutubeLiveChatMessageResponseItem(string kind, 
			string etag, string nextPageToken, 
			int pollingIntervalMillis, 
			PageInfo pageInfo,
			Dictionary<string, YoutubeLiveChatMessageItem> chatMessages)
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

		public Dictionary<string, YoutubeLiveChatMessageItem> ChatMessages { get; set; }//1回のリクエストでとれたコメント

		public void AddChatMessageItem(string message)
		{

		}
	}
	#endregion
}
