using Google.Apis.YouTube.v3.Data;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace YoutubeLiveBrowser.Models
{
	public class VideoListItem
	{
		public VideoListItem(string		 title,
							 BitmapImage image)
		{
			Title = title;
			Image = image;
		}

		public string Title { get; set; }
		public BitmapImage Image { get; set; }
	}

	class SubscriptionMenuModel
	{
		private DataBaseAccess m_DataBaseAccess;
		private YoutubeApiService m_YoutubeApiService;

		public SubscriptionMenuModel()
		{

		}

		public SubscriptionMenuModel(DataBaseAccess inDataBaseAccess, YoutubeApiService inYoutubeApiService)
		{
			m_DataBaseAccess	= inDataBaseAccess;
			m_YoutubeApiService = inYoutubeApiService;
		}

		#region CreateHamburgerMenuImageItems Menu用のアイテムを作成
		/// <summary>
		/// Menu用のアイテムを作成
		/// </summary>
		/// <returns></returns>
		public List<HamburgerMenuImageItem> CreateHamburgerMenuImageItems()
		{
			List<HamburgerMenuImageItem> items = new List<HamburgerMenuImageItem>();

			string myChannelId = ConfigurationManager.AppSettings["MyChannelId"];

			var subscriptions = m_YoutubeApiService.GetSubscriptions(myChannelId);
			foreach (Subscription subscription in subscriptions)
			{
				string channelId = subscription.Snippet.ResourceId.ChannelId;
				var channel = m_YoutubeApiService.GetChannel(channelId);

				string name = channel.Snippet.Title;
				string path	= channel.Snippet.Thumbnails.Default__.Url;

				items.Add(CreateHamburgerMenuImageItem(path, name));
			}

			return items;
		}
		#endregion

		#region CreateHamburgerMenuImageItemsAsync Menu用のアイテムを作成(非同期)
		/// <summary>
		/// Menu用のアイテムを作成(非同期)
		/// </summary>
		/// <returns></returns>
		public async Task<List<HamburgerMenuImageItem>> CreateHamburgerMenuImageItemsAsync()
		{
			List<HamburgerMenuImageItem> items = new List<HamburgerMenuImageItem>();

			string myChannelId = ConfigurationManager.AppSettings["MyChannelId"];

			var subscriptions = await m_YoutubeApiService.GetSubscriptionsAsync(myChannelId);
			foreach (Subscription subscription in subscriptions)
			{
				string channelId = subscription.Snippet.ResourceId.ChannelId;
				var channel = await m_YoutubeApiService.GetChannelAsync(channelId);

				string name = channel.Snippet.Title;
				string path = channel.Snippet.Thumbnails.Default__.Url;

				items.Add(CreateHamburgerMenuImageItem(path, name));
			}

			return items;
		}
		#endregion

		private HamburgerMenuImageItem CreateHamburgerMenuImageItem(string inResourceURL,
																	string inLabel)
		{
			HamburgerMenuImageItem item = new HamburgerMenuImageItem();

			item.Thumbnail  = CreateBitmapImage(inResourceURL);
			item.Label		= inLabel;

			return item;
		}

		private BitmapImage CreateBitmapImage(string inResourceURL)
		{
			var source = new BitmapImage();
			source.BeginInit();
			source.UriSource = new Uri(inResourceURL);
			source.DownloadCompleted += (sender, args) =>
			{
				var image = (BitmapImage)sender;
				image.Freeze();
			};
			source.EndInit();
			
			return source;
		}

		/// <summary>
		/// 指定したチャンネルの動画リストを作成
		/// </summary>
		/// <param name="inChannelId"></param>
		/// <returns></returns>
		public List<VideoListItem> CreateVideoListItems(string inChannelId)
		{
			List<VideoListItem> items = new List<VideoListItem>();

			var playlistItems = m_YoutubeApiService.GetVideos(inChannelId);
			foreach(PlaylistItem item in playlistItems)
			{
				string title = item.Snippet.Title;
				string url	 = item.Snippet.Thumbnails.Medium.Url;

				VideoListItem videoItem = CreateVideoListItem(title, url);

				items.Add(videoItem);
			}

			return items;
		}

		private VideoListItem CreateVideoListItem(string inTitle,string inResourceURL)
		{
			BitmapImage image = CreateBitmapImage(inResourceURL);

			return new VideoListItem(inTitle, image);
		}
	}
}
