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

		~YoutubeApiService()
		{
			_Service.Dispose();
		}

		//partに複数指定する場合はカンマ区切りで文字列を連結すればok

		#region CreateApiService サービスの作成
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
				ApiKey = inApiKey,
			});
		}
		#endregion

		#region GetSubscriptionsAsync 指定したチャンネルIDが登録しているチャンネルを取得
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

		public List<Subscription> GetSubscriptions(string channelId)
		{
			List<Subscription> subscriptions = new List<Subscription>();

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

			return subscriptions;
		}
		#endregion

		#region GetChannelAsync 指定したチャンネルIDの情報を取得
		/// <summary>
		/// 指定したチャンネルIDの情報を取得
		/// </summary>
		/// <param name="inChannelId"></param>
		/// <returns></returns>
		public async Task<Channel> GetChannelAsync(string inChannelId)
		{
			Channel channel = new Channel();
			var listRequest = _Service.Channels.List(YoutubeLive.YoutubePartParameters.snippet.ToString());
			listRequest.Id = inChannelId;
			var responce = await listRequest.ExecuteAsync();

			return responce.Items.First<Channel>();
		}

		public Channel GetChannel(string inChannelId)
		{
			Channel channel = new Channel();

			var listRequest = _Service.Channels.List(YoutubeLive.YoutubePartParameters.snippet.ToString());
			listRequest.Id = inChannelId;

			var responce = listRequest.Execute();

			return responce.Items.First<Channel>();
		}
		#endregion

		#region GetChannelName 指定したチャンネルIDの名前
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

		#region GetChannelNameAsync 指定したチャンネルIDの名前
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

		#region GetChatIdAsync 指定したチャットIDを取得
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

		#region GetChatCommentAsync 指定したチャットのコメントを取得
		/// <summary>
		/// 指定したチャットのコメントを取得
		/// </summary>
		/// <param name="inLiveChatId"></param>
		/// <returns></returns>
		public async Task<List<LiveChatMessage>> GetChatCommentAsync(string inLiveChatId)
		{
			var listRequest = _Service.LiveChatMessages.List(inLiveChatId, 
															 YoutubeLive.YoutubePartParameters.snippet.ToString() + "," +
															 YoutubeLive.YoutubePartParameters.authorDetails.ToString());
			var responce = await listRequest.ExecuteAsync();

			return responce.Items.ToList();
		}
		#endregion

		#region GetVideos 指定したチャンネルの投稿動画のリスト
		/// <summary>
		/// 指定したチャンネルの投稿動画のリスト
		/// </summary>
		/// <param name="inChannelId"></param>
		/// <returns></returns>
		public List<PlaylistItem> GetVideos(string inChannelId)
		{
			var listRequest = _Service.Channels.List(YoutubeLive.YoutubePartParameters.contentDetails.ToString());
			listRequest.Id = inChannelId;

			var response = listRequest.Execute();

			List<PlaylistItem> playlistItem = new List<PlaylistItem>();
			foreach(var item in response.Items)
			{
				string playListId = item.ContentDetails.RelatedPlaylists.Uploads;

				var playlistRequest = _Service.PlaylistItems.List(YoutubeLive.YoutubePartParameters.snippet.ToString());
				playlistRequest.PlaylistId = playListId;
				playlistRequest.MaxResults = 50;

				var playlisResponse = playlistRequest.Execute();

				// 全部取る
				string nextPage = playlisResponse.NextPageToken;
				while (!string.IsNullOrEmpty(nextPage))
				{
					playlistRequest.PageToken = nextPage;

					var res = playlistRequest.Execute();

					playlistItem.AddRange(res.Items);
					nextPage = res.NextPageToken;
				}

				playlistItem.AddRange(playlisResponse.Items);
			}
			
			return playlistItem;
		}
		#endregion

		#region GetVideosAsync 指定したチャンネルの投稿動画のリスト
		/// <summary>
		/// 指定したチャンネルの投稿動画のリスト
		/// </summary>
		/// <param name="inChannelId"></param>
		/// <returns></returns>
		public async Task<List<PlaylistItem>> GetVideosAsync(string inChannelId)
		{
			var listRequest = _Service.Channels.List(YoutubeLive.YoutubePartParameters.contentDetails.ToString());
			listRequest.Id = inChannelId;

			var response = await listRequest.ExecuteAsync();

			List<PlaylistItem> playlistItem = new List<PlaylistItem>();
			foreach (var item in response.Items)
			{
				string playListId = item.ContentDetails.RelatedPlaylists.Uploads;

				var playlistRequest = _Service.PlaylistItems.List(YoutubeLive.YoutubePartParameters.snippet.ToString());
				playlistRequest.PlaylistId = playListId;
				playlistRequest.MaxResults = 50;

				var playlisResponse = playlistRequest.Execute();

				// 全部取る
				string nextPage = playlisResponse.NextPageToken;
				while (!string.IsNullOrEmpty(nextPage))
				{
					playlistRequest.PageToken = nextPage;

					var res = playlistRequest.Execute();

					playlistItem.AddRange(res.Items);
					nextPage = res.NextPageToken;
				}

				playlistItem.AddRange(playlisResponse.Items);
			}

			return playlistItem;
		}
		#endregion
	}
}