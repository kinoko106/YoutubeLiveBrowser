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

		#region
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


		#region
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

		#region
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

		//api使わないもの
		#region GetStreamAsync
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetStreamAsync(string inChannelId)
		{
			string videoId = string.Empty;
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
					//例外にする
					videoId = "ストリーミングが見つかりませんでした";
				}
			});
			return videoId;
		}
		#endregion
	}
}