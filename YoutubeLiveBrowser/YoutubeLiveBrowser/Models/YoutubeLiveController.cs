using Codeplex.Data;
using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YoutubeLiveBrowser.Models
{
	class YoutubeLiveController
	{
		public string ChannelId { get; set; }
		public string VideoId { get; set; }
		public string LiveChatId { get; set; }

		public ObservableSynchronizedCollection<string> Comments { get; set; }

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YoutubeLiveController()
		{
			//使ってない
			//string path = YoutubeLive.AuthUrl;

			//var content = new Dictionary<string, string>() {
			//  { "code", "" },
			//  { "client_id", YoutubeLive.ClientId },
			//  { "client_secret", YoutubeLive.ClientSecert },
			//  { "redirect_uri",  "http://localhost:8080" },
			//  { "grant_type", "authorization_code" },
			//  { "access_type", "offline" },
			//};

			//WebRequest request = WebRequest.Create(YoutubeLive.TokenUrl);
			//request.Method = "POST";
			//request.ContentLength;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="inChannelId">チャンネルID</param>
		public YoutubeLiveController(string inChannelId)
		{
			ChannelId = inChannelId;
			Comments = new ObservableSynchronizedCollection<string>();
			BindingOperations.EnableCollectionSynchronization(Comments, new object());
		}
		#endregion

		#region GetStream
		/// <summary>
		/// 最新の動画IDを取得
		/// </summary>
		public string GetStream()
		{
			var videoIdRequest = WebRequest.Create("https://www.youtube.com/channel/" + ChannelId + "/videos?flow=list&live_view=501&view=2");
			try
			{ 
				using (var videoIdResponse = videoIdRequest.GetResponse())
				{
					using (var videoIdStream = new StreamReader(videoIdResponse.GetResponseStream(), Encoding.UTF8))
					{
						var videoIdRegex = new Regex("href=\"\\/watch\\?v=(.+?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);

						var videoIdMatch = videoIdRegex.Match(videoIdStream.ReadToEnd());

						if (!videoIdMatch.Success)
						{
							VideoId = "ストリーミングが見つかりませんでした";
							//Console.Error.WriteLine("Error: ストリーミングが見つかりませんでした");
							//Console.ReadKey();

							return VideoId;
						}

						var index1 = videoIdMatch.Value.LastIndexOf('=') + 1;
						var index2 = videoIdMatch.Value.LastIndexOf('"');

						VideoId = videoIdMatch.Value.Substring(index1, index2 - index1);
					}
				}
			}
			catch
			{
				
				return VideoId = "ストリーミングが見つかりませんでした";
			}
			return VideoId;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetStreamAsync()
		{
			await Task.Run(() =>
			{
				var videoIdRequest = WebRequest.Create("https://www.youtube.com/channel/" + ChannelId + "/videos?flow=list&live_view=501&view=2");
				try
				{
					using (var videoIdResponse = videoIdRequest.GetResponse())
					{
						using (var videoIdStream = new StreamReader(videoIdResponse.GetResponseStream(), Encoding.UTF8))
						{
							var videoIdRegex = new Regex("href=\"\\/watch\\?v=(.+?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);

							var videoIdMatch = videoIdRegex.Match(videoIdStream.ReadToEnd());

							if (!videoIdMatch.Success)
							{
								VideoId = "ストリーミングが見つかりませんでした";
							}

							var index1 = videoIdMatch.Value.LastIndexOf('=') + 1;
							var index2 = videoIdMatch.Value.LastIndexOf('"');

							VideoId = videoIdMatch.Value.Substring(index1, index2 - index1);
						}
					}
				}
				catch
				{
					VideoId = "ストリーミングが見つかりませんでした";
				}
			});
			return VideoId;
		}
		#endregion

		#region GetChatId
		/// <summary>
		/// YoutubeのライブチャットIDを取得
		/// 動画ID取得後に実行する
		/// </summary>
		public string GetChatId()
		{
			var liveChatIdRequest = WebRequest.Create("https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id=" + VideoId + "&key=" + YoutubeLive.APIKey);

			try
			{
				using (var liveChatIdResponse = liveChatIdRequest.GetResponse())
				{
					using (var liveChatIdStream = new StreamReader(liveChatIdResponse.GetResponseStream(), Encoding.UTF8))
					{
						var liveChatIdObject = DynamicJson.Parse(liveChatIdStream.ReadToEnd());

						LiveChatId = liveChatIdObject.items[0].liveStreamingDetails.activeLiveChatId;

						if (LiveChatId == null)
						{
							//Console.Error.WriteLine("Error: Live Chat IDの取得に失敗しました");
							//Console.ReadKey();
							return LiveChatId = "Live Chat IDの取得に失敗しました";
						}
					}
				}
			}
			catch
			{
				//Console.Error.WriteLine("Error: Live Chat IDの取得に失敗しました");
				//Console.ReadKey();

				return LiveChatId = "Live Chat IDの取得に失敗しました";
			}
			return LiveChatId;
		}
		#endregion

		#region GetChatIdAsync
		/// <summary>
		/// チャットIDを取得(非同期版)
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetChatIdAsync()
		{
			await Task.Run(() =>
			{
				var liveChatIdRequest = WebRequest.Create("https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id=" + VideoId + "&key=" + YoutubeLive.APIKey);
				try
				{
					using (var liveChatIdResponse = liveChatIdRequest.GetResponse())
					{
						using (var liveChatIdStream = new StreamReader(liveChatIdResponse.GetResponseStream(), Encoding.UTF8))
						{
							var liveChatIdObject = DynamicJson.Parse(liveChatIdStream.ReadToEnd());

							LiveChatId = liveChatIdObject.items[0].liveStreamingDetails.activeLiveChatId;

							if (LiveChatId == null)
							{
								LiveChatId = "Live Chat IDの取得に失敗しました";
							}
						}
					}
				}
				catch
				{
					LiveChatId = "Live Chat IDの取得に失敗しました";
				}
			});
			return LiveChatId;
		}
		#endregion

		#region GetChatCommentAsync
		/// <summary>
		/// 非同期でチャット欄のコメントを取り続ける
		/// あとでコレクションをに加算し、observerにaddする仕様に変更
		/// </summary>
		/// <returns></returns>
		/// 
		public void GetChatComment()
		{
			var messagesRequest = WebRequest.Create("https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet,authorDetails&liveChatId=" + LiveChatId + "&key=" + YoutubeLive.APIKey);
			var dateTimeNow = System.DateTime.Now;

			dynamic messagesObject = null;

			try
			{
				using (var messagesResponse = messagesRequest.GetResponse())
				{
					using (var messagesStream = new StreamReader(messagesResponse.GetResponseStream()))
					{
						messagesObject = DynamicJson.Parse(messagesStream.ReadToEnd());
					}
				}
			}
			catch
			{
				//Console.Error.WriteLine("Error: コメントの取得に失敗しました");
			}

			var MessageIds = new List<string>();
			var Messages = new Dictionary<string, object[]>();
			var messagesIdsOld = new List<string>();
			var messagesIdsDiff = new List<string>();

			var messageItems = new List<YoutubeLiveChatMessageItem>();
			var pageInfo = new PageInfo((int)messagesObject.pageInfo.totalResults, (int)messagesObject.pageInfo.resultsPerPage);
			var responseItem = new YoutubeLiveChatMessageResponseItem(messagesObject.kind, messagesObject.etag, messagesObject.nextPageToken, messagesObject.pollingIntervalMillis,pageInfo, messageItems);

			foreach (var value in messagesObject.items)
			{
				//var snippet = new YoutubeLiveChatMessageItem(value.kind,value.etag)

				MessageIds.Add(value.id);

				Messages.Add(value.id, new object[]
				{
						value.authorDetails.displayName,
						value.snippet.textMessageDetails.messageText,
						value.authorDetails.isChatOwner
				});
			}

			messagesIdsDiff = new List<string>(MessageIds);
			messagesIdsDiff.RemoveAll(messagesIdsOld.Contains);

			foreach (var value in messagesIdsDiff)
			{
				var displayName = Messages[value][0];
				var messageText = Messages[value][1];
				var isChatOwner = Messages[value][2];
			}
		}

		public async Task GetChatCommentAsync()
		{
			Comments = new ObservableSynchronizedCollection<string>();
			await Task.Run(() => 
			{
				while (true)
				{
					System.Threading.Thread.Sleep(500);//コメントを取得する処理
					Comments.Add("aaa");
				}
			});
		}

		#endregion
	}
}
