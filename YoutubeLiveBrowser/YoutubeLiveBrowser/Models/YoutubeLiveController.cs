﻿using Codeplex.Data;
using Google.Apis.YouTube.v3.Data;
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

using YoutubeLiveBrowser.Entity;

namespace YoutubeLiveBrowser.Models
{
	class YoutubeLiveController
	{
		//apiを使って取得してきた情報を画面表示用に加工するのがこいつの仕事

		public string ChannelId { get; set; }
		public string VideoId { get; set; }
		public string LiveChatId { get; set; }
		public string APIKey { get; set; }

		public string MyChannelId { get; set; }

		public YoutubeApiService ApiService { get; set; }

		public Dictionary<string, YoutubeLiveComment> LiveComments;
		public ObservableSynchronizedCollection<string> DisplayComments { get; set; }

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YoutubeLiveController()
		{

		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="inChannelId">チャンネルID</param>
		public YoutubeLiveController(string inChannelId,string inAPIKey)
		{
			ChannelId = inChannelId;
			APIKey = inAPIKey;
			DisplayComments = new ObservableSynchronizedCollection<string>();
			LiveComments = new Dictionary<string, YoutubeLiveComment>();
			BindingOperations.EnableCollectionSynchronization(DisplayComments, new object());
			ApiService = new YoutubeApiService(inAPIKey);
		}

		//使ってない
		public YoutubeLiveController(string inChannelId, string inAPIKey, ObservableSynchronizedCollection<string> inComments)
		{
			ChannelId = inChannelId;
			APIKey = inAPIKey;
			MyChannelId = "";
			DisplayComments = new ObservableSynchronizedCollection<string>();
			LiveComments = new Dictionary<string, YoutubeLiveComment>();
			BindingOperations.EnableCollectionSynchronization(DisplayComments, new object());
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
		#endregion

		#region GetStreamAsync
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetStreamAsync()
		{
			await Task.Run(() =>
			{
				string reqString = "https://www.youtube.com/channel/" + ChannelId + "/videos?flow=list&live_view=501&view=2";
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
			var liveChatIdRequest = WebRequest.Create("https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id=" + VideoId + "&key=" + APIKey);

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
							return LiveChatId = "Live Chat IDの取得に失敗しました";
						}
					}
				}
			}
			catch
			{
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
				string reqString = "https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id=" + VideoId + "&key=" + APIKey;
				var liveChatIdRequest = WebRequest.Create("https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id=" + VideoId + "&key=" + APIKey);
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

		#region GetChatComment
		/// <summary>
		/// 非同期でチャット欄のコメントを取り続ける
		/// あとでコレクションをに加算し、observerにaddする仕様に変更
		/// </summary>
		/// <returns></returns>
		/// 
		public void GetChatComment()
		{
			
			var dateTimeNow = System.DateTime.Now;
			dynamic messagesObject = null;
			var commentDiff = new Dictionary<string,YoutubeLiveComment>();

			while (true)
			{
				var messagesRequest = WebRequest.Create("https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet,authorDetails&liveChatId=" + LiveChatId + "&key=" + APIKey);
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
				catch(Exception e)
				{
					string errorMessage = e.Message;
					return;
					//Console.Error.WriteLine("Error: コメントの取得に失敗しました");
				}

				var comments = new Dictionary<string, YoutubeLiveComment>();
				var response = new YoutubeLiveChatMessageResponseItem(messagesObject);
				foreach (var comment in response.ChatMessages)
				{
					string id = comment.Key;
					string userName = comment.Value.DisplayName;
					var item = comment.Value;
					DateTime publishedAt = item.PublishedAt;
					string c = item.DisplayMessage;
					bool isOwner = item.IsChatOwner;
					bool isModerator = item.IsChatModerator;
					bool isChatSponsor = item.IsChatSponsor;

					YoutubeLiveComment newComment = new YoutubeLiveComment(id, userName, publishedAt, c, isOwner, isModerator, isChatSponsor);
					comments.Add(id, newComment);

					if(!LiveComments.ContainsKey(id))
					{
						LiveComments.Add(id, newComment);
					}
				}
			}
		}
		#endregion

		#region GetChatCommentAsync
		public async Task<Dictionary<string, YoutubeLiveComment>> GetChatCommentAsync()
		{
			var Comments = new Dictionary<string, YoutubeLiveComment>();

			dynamic messagesObject = null;
			var commentDiff = new Dictionary<string, YoutubeLiveComment>();

			await Task.Run(() =>
			{
				var messagesRequest = WebRequest.Create("https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet,authorDetails&liveChatId=" + LiveChatId + "&key=" + APIKey);
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

				//SuperChatも一緒にとれる罠、JSONのフォーマットが違うので区別が必要
				var comments = new Dictionary<string, YoutubeLiveComment>();
				var response = new YoutubeLiveChatMessageResponseItem(messagesObject);
				foreach (var comment in response.ChatMessages)
				{
					string id = comment.Key;
					string userName = comment.Value.DisplayName;
					var item = comment.Value;
					DateTime publishedAt = item.PublishedAt;
					string c = item.DisplayMessage;
					bool isOwner = item.IsChatOwner;
					bool isModerator = item.IsChatModerator;
					bool isChatSponsor = item.IsChatSponsor;

					YoutubeLiveComment newComment = new YoutubeLiveComment(id, userName, publishedAt, c, isOwner, isModerator, isChatSponsor);
					comments.Add(id, newComment);

					foreach (var com in comments)
					{
						if (!Comments.ContainsKey(com.Key))
						{
							Comments.Add(com.Key, com.Value);
						}
					}
				}
			});
			return Comments;
		}
		#endregion

		#region GetMyFeedChanneld
		/// <summary>
		/// 最新の動画IDを取得
		/// </summary>
		public string GetMyFeedChanneld()
		{
			var channelList = new List<string>();
			var myFeedChanneldRequest = WebRequest.Create("https://www.youtube.com/feed/channels/" + MyChannelId /*+ "/videos?flow=list&live_view=501&view=2"*/);
			try
			{
				using (var myFeedChanneldResponse = myFeedChanneldRequest.GetResponse())
				{
					using (var myFeedChannelIdStream = new StreamReader(myFeedChanneldResponse.GetResponseStream(), Encoding.UTF8))
					{
						var myFeedChannelIdRegex = new Regex("href=\"\\/watch\\?v=(.+?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);

						var channelIdIdMatch = myFeedChannelIdRegex.Match(myFeedChannelIdStream.ReadToEnd());

						if (!channelIdIdMatch.Success)
						{
							VideoId = "ストリーミングが見つかりませんでした";

							return VideoId;
						}

						var index1 = channelIdIdMatch.Value.LastIndexOf('=') + 1;
						var index2 = channelIdIdMatch.Value.LastIndexOf('"');

						VideoId = channelIdIdMatch.Value.Substring(index1, index2 - index1);
					}
				}
			}
			catch
			{
				return VideoId = "ストリーミングが見つかりませんでした";
			}
			return VideoId;
		}
		#endregion

		public async Task<List<Subscription>> GetSubscriptionAsync(string inChannelId)
		{
			return await ApiService.GetSubscriptionsAsync(inChannelId);
		}

		public async Task<List<string>> GetSubscriptionIdsAsync()
		{
			var ids = await GetSubscriptionAsync("UCeQEXsKfwrG91S1hGBRS-lQ");
			return ids.Select(x => x.Snippet.ResourceId.ChannelId).ToList();
		}

		public async Task<Dictionary<string,string>> GetSubscriptionNamesAsync()
		{
			//var ids = await GetSubscriptionAsync("UCeQEXsKfwrG91S1hGBRS-lQ");
			var ids = await GetSubscriptionAsync("UC6lIYMjiBf9xwxTPtlvaAOw");
			Dictionary<string, string> nameAndIds = new Dictionary<string, string>();

			await Task.Run(() =>
			{
				foreach (string channelId in ids.Select(x => x.Snippet.ResourceId.ChannelId).ToList())
				{
					string name = ApiService.GetChannelName(channelId);
					nameAndIds.Add(channelId, name);
				}
			});

			return nameAndIds;
		}
	}
}
