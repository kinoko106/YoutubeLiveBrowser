using Livet;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoutubeLiveBrowser.Models;

namespace YoutubeLiveBrowser.ViewModels
{
	class SubscriptionHamburgerMenuViewModel : ControlViewModelBase
	{
		SubscriptionMenuModel model;

		/// <summary>
		/// デザイナ用引数なしコンストラクタ
		/// </summary>
		public SubscriptionHamburgerMenuViewModel()
		{
			Height = 580;//windowタイトルを除いた高さ
			Width  = 900;
			SubScriptionMenuHeight = Height;
			SubScriptionMenuWidth = 280;
			VideoListPanelHeight = Height;
			VideoListPanelWidth = 700;
			VideoListPanelMargin = new Thickness(48, 0, 0, 0);

			IsPaneOpen = false;

			Items = new ObservableSynchronizedCollection<HamburgerMenuImageItem>();
			VideoListItems = new ObservableSynchronizedCollection<VideoListItem>();

			//VideoListItems.Add(AddVideoItems("item1", @"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\zui.jpg"));
			//VideoListItems.Add(AddVideoItems("item2", @"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\syokaku1.jpg"));
			
			//for (int i = 0; i < 20;i++)
			//{
			//	VideoListItems2.Add(AddVideoItems());
			//	//VideoListItems.Add(AddTile());
			//}
		}

		public SubscriptionHamburgerMenuViewModel(int MainwindowHeight,
												  int MainwinndowWidth,
												  DataBaseAccess	inDataBaseAccess,
												  YoutubeApiService inYoutubeApiService)
			: base(inDataBaseAccess, inYoutubeApiService)
		{
			model = new SubscriptionMenuModel(inDataBaseAccess, inYoutubeApiService); ;

			//コントロールの初期化
			Height					= MainwindowHeight - 20;
			Width					= MainwinndowWidth;
			SubScriptionMenuHeight	= Height;
			SubScriptionMenuWidth	= 280;
			VideoListPanelHeight	= Height;
			VideoListPanelWidth		= 700;
			VideoListPanelMargin	= new Thickness(48, 0, 0, 0);
			IsPaneOpen = false;

			//MenuItemの生成
			Items = new ObservableSynchronizedCollection<HamburgerMenuImageItem>();
			BindingOperations.EnableCollectionSynchronization(Items, new object());

			var items = model.CreateHamburgerMenuImageItems();
			foreach (HamburgerMenuImageItem item in items)
			{
				Items.Add(item);
			}
			//Task.Run(async () =>
			//{
			//	var items = await model.CreateHamburgerMenuImageItemsAsync();
			//	foreach (HamburgerMenuImageItem item in items)
			//	{
			//		Items.Add(item);
			//	}
			//});

			VideoListItems = new ObservableSynchronizedCollection<VideoListItem>();
			BindingOperations.EnableCollectionSynchronization(VideoListItems, new object());
			var videoItems = model.CreateVideoListItems("UCD-miitqNY3nyukJ4Fnf4_A");
			foreach (VideoListItem item in videoItems)
			{
				VideoListItems.Add(item);
			}
			//VideoListItems2.Add(AddVideoItems("item1", @"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\zui.jpg"));
			//VideoListItems2.Add(AddVideoItems("item2", @"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\syokaku1.jpg"));
		}

		#region SubscriptionMenu用プロパティ
		#region SubScriptionMenuWidth
		private int _SubScriptionMenuWidth;

		public int SubScriptionMenuWidth
		{
			get
			{ return _SubScriptionMenuWidth; }
			set
			{
				if (_SubScriptionMenuWidth == value)
					return;
				_SubScriptionMenuWidth = value;
				RaisePropertyChanged(nameof(SubScriptionMenuWidth));
			}
		}
		#endregion

		#region SubScriptionMenuHeight
		private int _SubScriptionMenuHeight;

		public int SubScriptionMenuHeight
		{
			get
			{ return _SubScriptionMenuHeight; }
			set
			{
				if (_SubScriptionMenuHeight == value)
					return;
				_SubScriptionMenuHeight = value;
				RaisePropertyChanged(nameof(SubScriptionMenuHeight));
			}
		}
		#endregion

		#region Items
		private ObservableSynchronizedCollection<HamburgerMenuImageItem> _Items;

		public ObservableSynchronizedCollection<HamburgerMenuImageItem> Items
		{
			get
			{ return _Items; }
			set
			{
				if (_Items == value)
					return;
				_Items = value;
				RaisePropertyChanged(nameof(Items));
			}
		}
		#endregion

		#region IsPaneOpen
		private bool _IsPaneOpen = true;

		public bool IsPaneOpen
		{
			get
			{ return _IsPaneOpen; }
			set
			{
				if (_IsPaneOpen == value)
					return;
				_IsPaneOpen = value;
				RaisePropertyChanged(nameof(IsPaneOpen));
			}
		}
		#endregion
		#endregion

		#region VireoList用プロパティ
		#region VideoListPanelWidth
		private int _VideoListPanelWidth;

		public int VideoListPanelWidth
		{
			get
			{ return _VideoListPanelWidth; }
			set
			{
				if (_VideoListPanelWidth == value)
					return;
				_VideoListPanelWidth = value;
				RaisePropertyChanged(nameof(VideoListPanelWidth));
			}
		}
		#endregion

		#region VideoListPanelHeight
		private int _VideoListPanelHeight;

		public int VideoListPanelHeight
		{
			get
			{ return _VideoListPanelHeight; }
			set
			{
				if (_VideoListPanelHeight == value)
					return;
				_VideoListPanelHeight = value;
				RaisePropertyChanged(nameof(VideoListPanelHeight));
			}
		}
		#endregion

		#region VideoListPanelMargin
		private Thickness _VideoListPanelMargin;

		public Thickness VideoListPanelMargin
		{
			get
			{ return _VideoListPanelMargin; }
			set
			{
				if (_VideoListPanelMargin == value)
					return;
				_VideoListPanelMargin = value;
				RaisePropertyChanged(nameof(VideoListPanelMargin));
			}
		}
		#endregion

		#region VideoListItems
		//private ObservableSynchronizedCollection<Tile> _VideoListItems;

		//public ObservableSynchronizedCollection<Tile> VideoListItems
		//{
		//	get
		//	{ return _VideoListItems; }
		//	set
		//	{
		//		if (_VideoListItems == value)
		//			return;
		//		_VideoListItems = value;
		//		RaisePropertyChanged(nameof(VideoListItems));
		//	}
		//}
		#endregion

		#region VideoListItems
		private ObservableSynchronizedCollection<VideoListItem> _VideoListItems;

		public ObservableSynchronizedCollection<VideoListItem> VideoListItems
		{
			get
			{ return _VideoListItems; }
			set
			{
				if (_VideoListItems == value)
					return;
				_VideoListItems = value;
				RaisePropertyChanged(nameof(VideoListItems));
			}
		}
		#endregion
		#endregion

		private VideoListItem AddVideoItems(string inTitle,string inResourceURL)
		{
			var source = new BitmapImage();
			source.BeginInit();
			source.UriSource = new Uri(inResourceURL);
			source.EndInit();

			VideoListItem item = new VideoListItem(inTitle, source);

			return item;
		}
	}
}
