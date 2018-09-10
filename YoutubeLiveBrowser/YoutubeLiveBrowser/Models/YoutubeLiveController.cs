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

		public string MyChannelId { get; set; }

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
			//自分のチャンネルID、api使って情報取る際に使う
			MyChannelId = inChannelId;

			LiveComments			= new Dictionary<string, LiveChatMessage>();
			SubscriptionNameAndId	= new Dictionary<string, string>();
			LiveChatIds				= new Dictionary<string, string>();

			//DB接続、チャンネルとか取る
			dataBaseAccess = new DataBaseAccess();
			var channels = dataBaseAccess.GetChannels();
			//var streams  = dataBaseAccess.GetYoutubeLiveStreamInfo();
			//dataBaseAccess.GetYoutubeLiveComments(streams[0]);

			//DBから取ってきた取得済みチャンネルリスト
			Channels = new List<Channels>();
			Channels.AddRange(channels);

			YoutubeLiveStreamInfos	= new List<YoutubeLiveStreamInfo>();
			ApiService				= new YoutubeApiService(inAPIKey);
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

		#region  GetSubscriptionAsync 登録チャンネルのリスト取得
		/// <summary>
		/// 登録チャンネルのリスト取得
		/// </summary>
		/// <param name="inChannelId">自分のチャンネルID</param>
		/// <returns></returns>
		public async Task<List<Subscription>> GetSubscriptionAsync(string inChannelId)
		{
			return await ApiService.GetSubscriptionsAsync(inChannelId);
		}
		#endregion
		
		#region GetSubscriptionChannels 登録しているチャンネルのリスト
		/// <summary>
		/// 登録しているチャンネルのリスト
		/// </summary>
		/// <returns></returns>
		public async Task GetSubscriptionChannelsAsync()
		{
			//var subscriptions = await GetSubscriptionAsync(MyChannelId);
			await Task.Run(async () =>
			{
				var subscriptions = await GetSubscriptionAsync(MyChannelId);

				foreach (var subscription in subscriptions)
				{
					string channelId = subscription.Snippet.ResourceId.ChannelId;
					string channelName = ApiService.GetChannelName(channelId);

					Channels channel = new Channels(channelId, channelName);

					if (!Channels.Contains(channel))
					{
						Channels.Add(channel);
					}
				}
			});
		}
		#endregion

		#region GetChannelIds チャンネルID一覧
		/// <summary>
		/// チャンネルID一覧
		/// </summary>
		/// <returns></returns>
		public List<string> GetChannelIds()
		{
			return Channels?.Select(x => x.ChannelId).ToList();
		}

		public string GetChannelIdByName(string inChannelName)
		{
			return Channels?.Where(x => x.ChannelName == inChannelName).Select(x => x.ChannelId).First();
		}
		#endregion

		#region GetChannelNames チャンネル名一覧
		/// <summary>
		/// チャンネル名一覧
		/// </summary>
		/// <returns></returns>
		public List<string> GetChannelNames()
		{
			return Channels?.Select(x => x.ChannelName).ToList();
		}
		#endregion

		#region GetVideoIds 指定したチャンネルの動画リストを取得
		/// <summary>
		/// 動画ID一覧
		/// 指定したチャンネルの動画リストを取得
		/// </summary>
		/// <param name="inChannelId">チャンネルID</param>
		/// <returns></returns>
		public List<string> GetVideoIds(string inChannelId)
		{
			return YoutubeLiveStreamInfos?.Where(x => x.ChannelId == inChannelId).Select(x => x.VideoId).ToList();
		}
		#endregion

		#region GetLiveChatIds チャットID一覧
		/// <summary>s
		/// チャットID一覧
		/// </summary>
		/// <returns></returns>
		public List<string> GetLiveChatIds(string inChannelId)
		{
			return  YoutubeLiveStreamInfos?.Where(x => x.ChannelId == inChannelId).Select(x => x.LiveChatId).ToList();
		}
		#endregion

		#region GetYoutubeLiveStreamInfoAsync 指定したチャンネルのStreamをDBから取得
		/// <summary>
		/// DBに登録済みのチャンネルの動画情報を取得
		/// </summary>
		/// <returns></returns>
		public async Task GetYoutubeLiveStreamInfoAsync()
		{
			await Task.Run(() => 
			{
				foreach (Channels channel in Channels)
				{
					var streams = dataBaseAccess.GetYoutubeLiveStreamInfo(channel);

					YoutubeLiveStreamInfos.AddRange(streams);
				}
			});
		}
		#endregion
	}
}