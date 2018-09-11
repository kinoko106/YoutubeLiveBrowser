using Google.Apis.YouTube.v3.Data;
using Livet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoutubeLiveBrowser.Models;
using YoutubeLiveBrowser.ViewModels;

namespace YoutubeLiveBrowser.ViewModels
{
	class PopupTabViewModel : ControlViewModelBase
	{
		YoutubeApiService youtubeApiService;

		public PopupTabViewModel(int inMainwindowHeight,int MainWindowWidth)
		{
			// サイズの定義
			Height			 = 50;
			Width			 = 50;
			ItemsPanelHeight = (inMainwindowHeight - 20) - Height;
			ItemsPanelWidth  = 200;

			// ローカル変数の準備
			ImageItems = new ObservableSynchronizedCollection<ImageSource>();

			// 必要な情報を取得
			string myChannelId = ConfigurationManager.AppSettings["MyChannelId"];
			string apiKey = ConfigurationManager.AppSettings["APIKey"];

			// APIサービスの開始
			youtubeApiService = new YoutubeApiService(apiKey);

			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\yamato.jpg");

			// 登録チャンネルのリストを取得
			List<Subscription> subscriptions = youtubeApiService.GetSubscriptions(myChannelId);
			foreach (Subscription subscription in subscriptions)
			{
				string title = subscription.Snippet.ChannelTitle;
				string channelId = subscription.Snippet.ResourceId.ChannelId;

				var channel = youtubeApiService.GetChannel(channelId);

				var path = channel.Snippet.Thumbnails.Medium.Url;

				AddItem(path);
			}
			//Task.Run(async () =>
			//{
			//	foreach (Subscription subscription in subscriptions)
			//	{
			//		string title = subscription.Snippet.ChannelTitle;
			//		string channelId = subscription.Snippet.ResourceId.ChannelId;

			//		var channel = await youtubeApiService.GetChannelAsync(channelId);

			//		var path = channel.Snippet.Thumbnails;
			//	}
			//});
		}

		public void AddItem(string inResourceURL)
		{
			var source = new BitmapImage();
			source.BeginInit();
			source.UriSource = new Uri(inResourceURL);
			source.EndInit();

			ImageItems.Add(source);
		}

		#region ImageItems
		private ObservableSynchronizedCollection<ImageSource> _ImageItems;

		public ObservableSynchronizedCollection<ImageSource> ImageItems
		{
			get
			{ return _ImageItems; }
			set
			{
				if (_ImageItems == value)
					return;
				_ImageItems = value;
				RaisePropertyChanged(nameof(ImageItems));
			}
		}
		#endregion

		#region ItemsPanelWidth
		private int _ItemsPanelWidth;

		public int ItemsPanelWidth
		{
			get
			{ return _ItemsPanelWidth; }
			set
			{
				if (_ItemsPanelWidth == value)
					return;
				_ItemsPanelWidth = value;
				RaisePropertyChanged(nameof(ItemsPanelWidth));
			}
		}
		#endregion

		#region ItemsPanelHeight
		private int _ItemsPanelHeight;

		public int ItemsPanelHeight
		{
			get
			{ return _ItemsPanelHeight; }
			set
			{
				if (_ItemsPanelHeight == value)
					return;
				_ItemsPanelHeight = value;
				RaisePropertyChanged(nameof(ItemsPanelHeight));
			}
		}
		#endregion
	}
}
