﻿using Livet;
using Livet.Commands;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
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
		//VideoInfoViewModel VideoInfo;

		/// <summary>
		/// デザイナ用引数なしコンストラクタ
		/// </summary>
		public SubscriptionHamburgerMenuViewModel()
		{
			model = new SubscriptionMenuModel();
			//コントロールの初期化
			Height = 580;//windowタイトルを除いた高さ
			Width = 900;
			SubScriptionMenuHeight = Height;
			SubScriptionMenuWidth = 280;
			VideoListPanelHeight = Height;
			VideoListPanelWidth = 700;
			VideoListPanelMargin = new Thickness(48, 0, 0, 0);

			IsPaneOpen = false;

			Items = new ObservableSynchronizedCollection<HamburgerMenuImageItem>();

			VideoListItems = new ObservableSynchronizedCollection<VideoListItem>();

			VideoListItems.Add(AddVideoItems("item1", @"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\zui.jpg"));
			VideoListItems.Add(AddVideoItems("item1", @"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\zui.jpg"));


			VideoInfo = new VideoInfoViewModel();
			//for (int i = 0; i < 20;i++)
			//{
			//	VideoListItems2.Add(AddVideoItems());
			//	//VideoListItems.Add(AddTile());
			//}
		}

		public SubscriptionHamburgerMenuViewModel(int MainwindowHeight,
												  int MainwinndowWidth,
												  DataBaseAccess inDataBaseAccess,
												  YoutubeApiService inYoutubeApiService)
			: base(inDataBaseAccess, inYoutubeApiService)
		{
			model = new SubscriptionMenuModel(inDataBaseAccess, inYoutubeApiService); ;

			//コントロールの初期化
			Height = MainwindowHeight - 20;
			Width = MainwinndowWidth;
			SubScriptionMenuHeight = Height;
			SubScriptionMenuWidth = 280;
			VideoListPanelHeight = Height;
			VideoListPanelWidth = 700;
			VideoListPanelMargin = new Thickness(48, 0, 0, 0);
			IsPaneOpen = false;
			IsVideoListProgressActive = false;
			//MenuItemの生成
			Items = new ObservableSynchronizedCollection<HamburgerMenuImageItem>();
			BindingOperations.EnableCollectionSynchronization(Items, new object());

			VideoListItems = new ObservableSynchronizedCollection<VideoListItem>();
			BindingOperations.EnableCollectionSynchronization(VideoListItems, new object());

			//動画情報のウィンドウ
			VideoInfo = new VideoInfoViewModel();
			InitializeMenuItem();

			VideoItem = new VideoItemViewModel();
		}

		#region 汎用メソッド

		#region selectItem メニュー選択時メソッド
		/// <summary>
		/// メニュー選択時メソッド
		/// </summary>
		/// <param name="sender"></param>
		private void selectItem(object sender)
		{
			HamburgerMenu menu = sender as HamburgerMenu;

			HamburgerMenuImageItem imageItem = (HamburgerMenuImageItem)menu.Items[menu.SelectedIndex];
			string selectedChannelId = model.FindChannelId(imageItem.Label);

			VideoListItems.Clear();


			Task.Run(async () =>
			{
				IsVideoListProgressActive = true;
				await CreateVideoListAsync(selectedChannelId);
				IsVideoListProgressActive = false;
			});
		}
		#endregion

		#region InitializeMenuItem 最初画面を表示する際に使用
		/// <summary>
		/// 最初画面を表示する際に使用
		/// </summary>
		public async void InitializeMenuItem()
		{
			IsVideoListProgressActive = true;
			await CreateSubscriptionMenuItemAsync();
			CreateVideoListByNameAsync(Items.First().Label);
			IsVideoListProgressActive = false;
		}
		#endregion
		#endregion

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

		#region SubScriptionMenuItemClicked メニューアイテム選択時
		public ListenerCommand<HamburgerMenu> _SubScriptionMenuItemClicked;

		public ListenerCommand<HamburgerMenu> SubScriptionMenuItemClicked
		{
			get
			{
				if (_SubScriptionMenuItemClicked == null)
				{
					_SubScriptionMenuItemClicked = new ListenerCommand<HamburgerMenu>(selectItem);
				}
				return _SubScriptionMenuItemClicked;
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

		#region CreateSubscriptionMenuItemAsync メニューアイテム作成(非同期)
		/// <summary>
		/// メニューアイテム作成(非同期)
		/// </summary>
		public async Task CreateSubscriptionMenuItemAsync()
		{
			var listMaterial = await model.CreateHamburgerMenuImageItemsAsync();
			foreach (HamburgerMenuImageItem item in listMaterial)
			{
				Items.Add(item);
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

		#region IsVideoListProgressActive
		private bool _IsVideoListProgressActive;

		public bool IsVideoListProgressActive
		{
			get
			{ return _IsVideoListProgressActive; }
			set
			{
				if (_IsVideoListProgressActive == value)
					return;
				_IsVideoListProgressActive = value;
				RaisePropertyChanged(nameof(IsVideoListProgressActive));
			}
		}
		#endregion

		#region CreateVideoList 動画リストの要素を作成
		private void CreateVideoList(string inChannelName)
		{
			VideoListItems.Clear();

			var videoItems = model.CreateVideoListItems(inChannelName);
			foreach (VideoListItem item in videoItems)
			{
				VideoListItems.Add(item);
			}
		}
		#endregion

		#region CreateVideoListAsync 動画リストの要素を作成(非同期)
		/// <summary>
		/// 
		/// </summary>
		public async Task CreateVideoListAsync(string selectedChannelId)
		{
			// UI作成用の情報をAPIを使用して集める
			var videoItems = await model.CreateVideoListItemAsync(selectedChannelId);

			// UI作成はまとめてUIスレッドに投げる
			App.Current.Dispatcher.Invoke(() =>
			{
				foreach (VideoListItem item in videoItems)
				{

					VideoListItems.Add(item);
				}
			});
		}
		#endregion

		#region CreateVideoListByNameAsync 動画リストの要素をチャンネル名から作成(非同期)
		/// <summary>
		/// 
		/// </summary>
		public async void CreateVideoListByNameAsync(string inChannelId)
		{
			var videoItems = await model.CreateVideoListItemByNameAsync(inChannelId);

			foreach (VideoListItem item in videoItems)
			{
				VideoListItems.Add(item);
			}
		}
		#endregion
		#endregion

		#region VideoInfo用プロパティ

		#region VideoInfo
		private VideoInfoViewModel _VideoInfo;

		public VideoInfoViewModel VideoInfo
		{
			get
			{ return _VideoInfo; }
			set
			{
				if (_VideoInfo == value)
					return;
				_VideoInfo = value;
				RaisePropertyChanged(nameof(VideoInfo));
			}
		}
		#endregion

		#endregion

		#region AddVideoItems 動画リスト追加(デザイナ用)
		/// <summary>
		/// デザイナ用
		/// </summary>
		/// <param name="inTitle"></param>
		/// <param name="inResourceURL"></param>
		/// <returns></returns>
		private VideoListItem AddVideoItems(string inTitle, string inResourceURL)
		{
			var source = new BitmapImage();
			source.BeginInit();
			source.UriSource = new Uri(inResourceURL);
			source.EndInit();

			VideoListItem item = new VideoListItem(inTitle, source);

			return item;
		}
		#endregion

		public ViewModelCommand _OpenFlyOut;
		public ViewModelCommand OpenFlyOut
		{
			get
			{
				if(_OpenFlyOut == null)
				{
					_OpenFlyOut = new ViewModelCommand(() => { VideoInfo.IsOpen = true; });
				}
				return _OpenFlyOut;
			}
		}


		#region VideoItem
		private VideoItemViewModel _VideoItem;

		public VideoItemViewModel VideoItem
		{
			get
			{ return _VideoItem; }
			set
			{
				if (_VideoItem == value)
					return;
				_VideoItem = value;
				RaisePropertyChanged(nameof(VideoItem));
			}
		}
		#endregion
	}
}
