using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace YoutubeLiveBrowser.Models
{
	public class YoutubeApiService
	{
		//youtubeDataApiを呼ぶだけ
		private YouTubeService _Service;

		public YoutubeApiService(string inApiKey)
		{
			CreateApiService(inApiKey);
		}

		#region CreateApiService
		/// <summary>
		/// サービスの作成
		/// </summary>
		/// <param name="inApiKey"></param>
		private void CreateApiService(string inApiKey)
		{
			var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
				new ClientSecrets
				{
					ClientId = YoutubeLive.ClientId,
					ClientSecret = YoutubeLive.ClientSecert
				},
				new[] { Google.Apis.YouTube.v3.YouTubeService.Scope.Youtube }
				, "user"
				, CancellationToken.None
				, new FileDataStore("YoutubeLiveBrowser")
			).Result;

			_Service = new YouTubeService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = "YoutubeLiveBrowser",
				ApiKey = "AIzaSyBFC4kLWlrYZbddNr3VJRYMfK_ew473mlY",//ApiKey = "AIzaSyBFC4kLWlrYZbddNr3VJRYMfK_ew473mlY",
			});
		}
		#endregion

		#region GetSubscriptionsAsync
		/// <summary>
		/// 指定したチャンネルIDが登録しているチャンネルを取得
		/// </summary>
		/// <param name="channelId"></param>
		/// <returns></returns>
		public async Task<List<Subscription>> GetSubscriptionsAsync(string channelId)
		{
			List<Subscription> subscriptions = new List<Subscription>();

			await Task.Run(() =>
			{
				var listRequest = _Service.Subscriptions.List(YoutubeLive.YoutubePartParameters.snippet.ToString());
				listRequest.ChannelId = channelId;

				var responce = listRequest.Execute();
				subscriptions = responce.Items.ToList();

				string nextPage = responce.NextPageToken;
				while (!string.IsNullOrEmpty(nextPage))
				{
					listRequest.PageToken = nextPage;
					var res = listRequest.Execute();
					subscriptions.AddRange(res.Items.ToList());
					nextPage = res.NextPageToken;
				}
			});

			return subscriptions;
		}
		#endregion

		#region GetChannelName
		/// <summary>
		/// 指定したチャンネルIDの名前
		/// </summary>
		/// <param name="channelId"></param>
		/// <returns></returns>
		public string GetChannelName(string channelId)
		{
			var listRequest = _Service.Channels.List(YoutubeLive.YoutubePartParameters.snippet.ToString());
			listRequest.Id = channelId;
			var responce = listRequest.Execute();

			return responce.Items.First<Channel>().Snippet.Title;
		}
		#endregion

		#region GetChannelNameAsync
		/// <summary>
		/// 指定したチャンネルIDの名前
		/// </summary>
		/// <param name="channelId"></param>
		/// <returns></returns>
		public async Task<string> GetChannelNameAsync(string channelId)
		{
			var listRequest = _Service.Channels.List(YoutubeLive.YoutubePartParameters.snippet.ToString());
			listRequest.Id = channelId;
			var responce = await listRequest.ExecuteAsync();

			return responce.Items.First<Channel>().Snippet.Title;
		}
		#endregion

		#region GetChatIdAsync
		/// <summary>
		/// 指定したチャットIDを取得
		/// </summary>
		/// <param name="channelId"></param>
		/// <returns></returns>
		public async Task<string> GetChatIdAsync(string inVideoId)
		{
			var listRequest = _Service.Videos.List(YoutubeLive.YoutubePartParameters.liveStreamingDetails.ToString());
			listRequest.Id = inVideoId;
			var responce = await listRequest.ExecuteAsync();

			return responce.Items.First().LiveStreamingDetails.ActiveLiveChatId;
		}
		#endregion

	}
}