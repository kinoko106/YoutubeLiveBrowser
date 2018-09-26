using Google.Apis.YouTube.v3.Data;
using Livet.Commands;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YoutubeLiveBrowser.Entity;
using YoutubeLiveBrowser.ViewModels;

namespace YoutubeLiveBrowser.Models
{
	//viewmodel化する
	//SubscriptionHamburgerMenuViewModelでインスタンス作成して、親オブジェクトとして登録、
	//さらに子オブジェクトとしてVideoInfoを登録し、IsOpenを操作できるように変更
	public class VideoListItem
	{
		public VideoListItem(string		 title,
							 BitmapImage image)
		{
			Title = title;
			Image = image;
		}

		//非同期で実行する場合は先に画像の参照先だけを設定しておき、あとで画像の取得と
		public VideoListItem(string title, 
							 string bitmapImageURL)
		{
			Title = title;
			BitmapImageURL = bitmapImageURL;
			TileCommand = new ViewModelCommand(Clicked);
		}

		public string		Title			{ get; set; }
		public string		BitmapImageURL  { get; set; }
		public BitmapImage	Image			{ get; set; }

		public int			ImageWidth	{ get; set; }
		public int			ImageHeight	{ get; set; }
		public int			TextWidth	{ get; set; }
		public int			TextHeight	{ get; set; }
		
		public ViewModelCommand TileCommand { get; set; }//ボタンに個別のイベントを設定したい

		private void Clicked()
		{
			
		}

		private VideoInfoViewModel VideoInfoViewModel { get; set; }

	}

	class SubscriptionMenuModel
	{
		private DataBaseAccess m_DataBaseAccess;
		private YoutubeApiService m_YoutubeApiService;

		private List<Channels> m_Channels;

		public SubscriptionMenuModel()
		{
			m_DataBaseAccess = null;
			m_YoutubeApiService = null;
			m_Channels = new List<Channels>();
		}

		public SubscriptionMenuModel(DataBaseAccess inDataBaseAccess, YoutubeApiService inYoutubeApiService)
		{
			m_DataBaseAccess	= inDataBaseAccess;
			m_YoutubeApiService = inYoutubeApiService;
			m_Channels			= new List<Channels>();
		}

		#region GetChannels 登録チャンネルを取得
		public void GetChannels()
		{
			//string myChannelId = ConfigurationManager.AppSettings["MyChannelId"];
			//var subscription = m_YoutubeApiService.GetSubscriptions(myChannelId);
		}
		#endregion

		#region FindChannelId チャンネル名からID取得
		public string FindChannelId(string inChannelName)
		{
			return m_Channels.Where(x=>x.ChannelName == inChannelName)?.Select(x=>x.ChannelId).First();
		}
		#endregion

		#region CreateBitmapImage BitmapImageを作成
		/// <summary>
		/// BitmapImageを作成
		/// </summary>
		/// <param name="inResourceURL"></param>
		/// <returns></returns>
		private BitmapImage CreateBitmapImage(string inResourceURL)
		{
			var source = new BitmapImage();
			source.BeginInit();
			source.UriSource = new Uri(inResourceURL);
			source.EndInit();

			return source;
		}
		#endregion

		#region チャンネルメニュー用メソッド群
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
				//m_HamburgerItemElements.Add(name, path);//チャンネル名、画像パス
				//内部管理用のリスト
				m_Channels.Add(new Channels(channelId, name));
			}

			return items;
		}
		#endregion

		#region CreateHamburgerMenuItemMaterialAsync Menu用のアイテムの素材を作成(非同期)
		/// <summary>
		/// Menu用のアイテムを作成(非同期)
		/// </summary>
		/// <returns></returns>
		public async Task<Dictionary<string, string>> CreateHamburgerMenuItemMaterialAsync()
		{
			Dictionary<string, string> items = new Dictionary<string, string>();

			string myChannelId = ConfigurationManager.AppSettings["MyChannelId"];

			var subscriptions = await m_YoutubeApiService.GetSubscriptionsAsync(myChannelId);
			foreach (Subscription subscription in subscriptions)
			{
				string	channelId = subscription.Snippet.ResourceId.ChannelId;
				var		channel	  = await m_YoutubeApiService.GetChannelAsync(channelId);

				string name = channel.Snippet.Title;
				string path = channel.Snippet.Thumbnails.Default__.Url;

				items.Add(name, path);

				// 内部管理用のリスト
				m_Channels.Add(new Channels(channelId, name));
			}

			return items;
		}
		#endregion

		#region CreateHamburgerMenuImageItems Menu用のアイテムを作成
		/// <summary>
		/// Menu用のアイテムを作成
		/// </summary>
		/// <returns></returns>
		public async Task<List<HamburgerMenuImageItem>> CreateHamburgerMenuImageItemsAsync()
		{
			List<HamburgerMenuImageItem> items = new List<HamburgerMenuImageItem>();

			// まず最初にコントロール作成用の素材を作る
			var listMaterials = await CreateHamburgerMenuItemMaterialAsync();
			//全体をUIスレッドに投げる
			App.Current.Dispatcher.Invoke(() =>
			{
				foreach (var listMaterial in listMaterials)
				{
					string resourceUrl = listMaterial.Value;
					// 素材のから表示するサムネイルのBitMapを作成 UIスレッドでやる
					var bitmap = CreateBitmapImage(resourceUrl);
					// 名前、画像が揃ったらコントロールを作成 UIスレッドでやる
					var item = CreateHamburgerMenuImageItem(bitmap, listMaterial.Key);

					items.Add(item);
				}
			});

			// 旧 作成アイテム毎にUIスレッド呼んでる
			//foreach(var listMaterial in listMaterials)
			//{
			//	string resourceUrl = listMaterial.Value;
			//	// 素材のから表示するサムネイルのBitMapを作成 UIスレッドでやる
			//	var bitmap = await App.Current.Dispatcher.InvokeAsync(() => CreateBitmapImage(resourceUrl));
			//	// 名前、画像が揃ったらコントロールを作成 UIスレッドでやる
			//	var item   = await App.Current.Dispatcher.InvokeAsync(() => CreateHamburgerMenuImageItemAsync(bitmap, listMaterial.Key));

			//	items.Add(item);
			//}

			return items;
		}
		#endregion

		#region CreateHamburgerMenuImageItem ハンバーガーメニューアイテムを作成
		/// <summary>
		/// ハンバーガーメニューアイテムを作成
		/// </summary>
		/// <param name="inResourceURL"></param>
		/// <param name="inLabel"></param>
		/// <returns></returns>
		private HamburgerMenuImageItem CreateHamburgerMenuImageItem(string inResourceURL,
																	string inLabel)
		{
			HamburgerMenuImageItem item = new HamburgerMenuImageItem();

			item.Thumbnail	= CreateBitmapImage(inResourceURL);
			item.Label		= inLabel;

			return item;
		}

		private HamburgerMenuImageItem CreateHamburgerMenuImageItem(BitmapImage inImage,
																	string inLabel)
		{
			HamburgerMenuImageItem item = new HamburgerMenuImageItem();

			item.Thumbnail	= inImage;
			item.Label		= inLabel;

			return item;
		}
		#endregion
		#endregion

		#region 動画メニュー用メソッド群

		#region CreateVideoListItems 指定したチャンネルの動画リストを作成
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
		#endregion

		#region CreateVideoListItemsAsync 指定したチャンネルの動画リストを作成
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inChannelId"></param>
		/// <returns></returns>
		public async Task<List<VideoListItem>> CreateVideoListItemsAsync(string inChannelId)
		{
			List<VideoListItem> items = new List<VideoListItem>();

			var playlistItems = await m_YoutubeApiService.GetVideosAsync(inChannelId);
			foreach (PlaylistItem item in playlistItems)
			{
				string title = item.Snippet.Title;
				string url   = item.Snippet.Thumbnails.Medium.Url;

				VideoListItem videoItem = new VideoListItem(title, url);
				//videoItem.TileCommand = new ListenerCommand<VideoListItem>(SelectVideoItem);
				//videoItem.TileCommand = new ViewModelCommand(SelectVideoItem);
				items.Add(videoItem);
			}

			return items;
		}
		#endregion

		#region CreateVideoListItemsAsync 指定したチャンネルの動画リストを作成(チャンネル名から)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inChannelName"></param>
		/// <returns></returns>
		public async Task<List<VideoListItem>> CreateVideoListItemByNameAsync(string inChannelName)
		{
			string id = FindChannelId(inChannelName);
			var items = await CreateVideoListItemsAsync(id);

			foreach (VideoListItem item in items)
			{
				string imageUrl = item.BitmapImageURL;
				item.Image = CreateBitmapImage(imageUrl);
			}

			//bitmapの作成はやっぱりUIスレッドで作る
			App.Current.Dispatcher.Invoke(() =>
			{
				foreach (VideoListItem item in items)
				{
					string imageUrl = item.BitmapImageURL;
					item.Image = CreateBitmapImage(imageUrl);
				}
			});

			return items;
		}
		#endregion

		#region CreateVideoListItemAsync チャンネルIDから
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inChannelId"></param>
		/// <returns></returns>
		public async Task<List<VideoListItem>> CreateVideoListItemAsync(string inChannelId)
		{
			var items = await CreateVideoListItemsAsync(inChannelId);

			// UIスレッドに投げる
			App.Current.Dispatcher.Invoke(() =>
			{
				foreach (VideoListItem item in items)
				{
					string imageUrl = item.BitmapImageURL;
					item.Image = CreateBitmapImage(imageUrl);
				}
			});

			return items;
		}


		private VideoListItem CreateVideoListItem(string inTitle,string inResourceURL)
		{
			BitmapImage image = CreateBitmapImage(inResourceURL);

			return new VideoListItem(inTitle, image);
		}
		#endregion

		#endregion
	}
}
