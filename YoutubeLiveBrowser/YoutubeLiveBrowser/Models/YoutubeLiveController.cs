using Codeplex.Data;
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
		public DataBaseAccess dataBaseAccess { get; set; }

		public YoutubeApiService ApiService { get; set; }

		public Dictionary<string, LiveChatMessage> LiveComments;//放送中のライブからとってきたコメント群、キーはコメントID
		public Dictionary<string, string> SubscriptionNameAndId { get; set; }
		public Dictionary<string, string> LiveChatIds { get; set; }
		
		public List<Channels> Channels { get; set; }
		public List<YoutubeLiveStreamInfo> YoutubeLiveStreamInfos { get; set; }

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

			LiveComments = new Dictionary<string, LiveChatMessage>();
			SubscriptionNameAndId = new Dictionary<string, string>();
			LiveChatIds = new Dictionary<string, string>();

			dataBaseAccess = new DataBaseAccess();
			var streams = dataBaseAccess.GetYoutubeLiveStreamInfo();
			dataBaseAccess.GetYoutubeLiveComments(streams[0]);

			YoutubeLiveStreamInfos = new List<YoutubeLiveStreamInfo>();
			ApiService = new YoutubeApiService(inAPIKey);
		}
		#endregion

		#region GetStreamAsync
		/// <summary>
		/// GetStreamAsync
		/// 最新動画だけAPIで取れない？
		/// </summary>
		/// <param name="inChannelId"></param>
		/// <returns></returns>
		public async Task<string> GetStreamAsync(string inChannelId)
		{
			string videoId = null;
			await Task.Run(() =>
			{
				string reqString = "https://www.youtube.com/channel/" + inChannelId + "/videos?flow=list&live_view=501&view=2";
				var videoIdRequest = WebRequest.Create("https://www.youtube.com/channel/" + inChannelId + "/videos?flow=list&live_view=501&view=2");
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
								videoId = "ストリーミングが見つかりませんでした";
							}

							var index1 = videoIdMatch.Value.LastIndexOf('=') + 1;
							var index2 = videoIdMatch.Value.LastIndexOf('"');

							videoId = videoIdMatch.Value.Substring(index1, index2 - index1);
						}
					}
				}
				catch
				{
					videoId = "ストリーミングが見つかりませんでした";
				}
			});
			return videoId;
		}
		#endregion

		#region GetChatCommentAsync
		/// <summary>
		/// 選択したチャットIDのコメントを取得
		/// </summary>
		/// <param name="inLiveChatId"></param>
		/// <returns></returns>
		public async Task<List<string>> GetChatCommentAsync(string inLiveChatId)
		{
			List<LiveChatMessage> messages = new List<LiveChatMessage>();

			messages = await ApiService.GetChatCommentAsync(inLiveChatId);

			List<string> newComments = new List<string>();
			foreach (var message in messages)
			{
				if (!LiveComments.ContainsKey(message.Id))
				{
					LiveComments.Add(message.Id, message);
					newComments.Add(message.Snippet.PublishedAt.ToString() + ":" +
									message.AuthorDetails.DisplayName + "  " +
									message.Snippet.DisplayMessage);
				}
			}

			return newComments;
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
			var ids = await GetSubscriptionAsync("UCeQEXsKfwrG91S1hGBRS-lQ");//自分のチャンネルID　configから取る
			//var ids = await GetSubscriptionAsync("UC6lIYMjiBf9xwxTPtlvaAOw");//登録チャンネル多いサンプル
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

		#region GetYoutubeLiveStreamInfos
		/// <summary>
		/// 登録しているチャンネルの情報を取得
		/// </summary>
		/// <returns></returns>
		public async Task<List<YoutubeLiveStreamInfo>> GetYoutubeLiveStreamInfos(bool IsRefresh)
		{
			if (IsRefresh)
			{
				var nameAndIds = await GetSubscriptionNamesAsync();

				foreach (var pair in nameAndIds)
				{
					string channelId = pair.Key;
					string channelName = pair.Value;
					string videoId = await GetStreamAsync(channelId);
					string liveChatId = await ApiService.GetChatIdAsync(videoId);

					Channels.Add(new Channels(channelId, channelName));
					YoutubeLiveStreamInfos.Add(new YoutubeLiveStreamInfo(channelId, channelName, videoId, liveChatId));
				}

				return YoutubeLiveStreamInfos;
			}
			else
			{
				return YoutubeLiveStreamInfos;
			}
		}
		#endregion

		#region GetChannelIds
		/// <summary>
		/// チャンネルID一覧
		/// </summary>
		/// <returns></returns>
		public List<string> GetChannelIds()
		{
			return YoutubeLiveStreamInfos?.Select(x => x.ChannelId).ToList();
		}
		#endregion

		#region GetChannelNames
		/// <summary>
		/// チャンネル名一覧
		/// </summary>
		/// <returns></returns>
		public List<string> GetChannelNames()
		{
			return null;
			//return YoutubeLiveStreamInfos?.Select(x => x.ChannelName).ToList();
		}
		#endregion

		#region GetVideoIds
		/// <summary>
		/// 動画ID一覧
		/// </summary>
		/// <returns></returns>
		public List<string> GetVideoIds()
		{
			return YoutubeLiveStreamInfos?.Select(x => x.VideoId).ToList();
		}
		#endregion

		#region GetLiveChatIds
		/// <summary>
		/// チャットID一覧
		/// </summary>
		/// <returns></returns>
		public List<string> GetLiveChatIds()
		{
			return YoutubeLiveStreamInfos?.Select(x => x.LiveChatId).ToList();
		}
		#endregion
	}
}
