using Livet;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoutubeLiveBrowser.Models;

namespace YoutubeLiveBrowser.ViewModels
{
	class SubscriptionHamburgerMenuViewModel : ControlViewModelBase
	{
		SubscriptionMenuModel model;

		public SubscriptionHamburgerMenuViewModel()
		{
			Height = 580;//windowタイトルを除いた高さ
			Width  = 280;
		}

		public SubscriptionHamburgerMenuViewModel(int MainwindowHeight,
												  int MainwinndoWidth,
												  DataBaseAccess	inDataBaseAccess,
												  YoutubeApiService inYoutubeApiService)
			: base(inDataBaseAccess, inYoutubeApiService)
		{
			model = new SubscriptionMenuModel(inDataBaseAccess, inYoutubeApiService); ;

			//初期化
			Height = MainwindowHeight - 20;
			Width  = 280;
			IsPaneOpen = false;

			//MenuItemの生成
			Items = new ObservableSynchronizedCollection<HamburgerMenuImageItem>();
			BindingOperations.EnableCollectionSynchronization(Items, new object());

			//var items = model.CreateHamburgerMenuImageItems();
			//foreach (HamburgerMenuImageItem item in items)
			//{
			//	Items.Add(item);
			//}

			//Task.Run(async () =>
			//{
			//	var items = await model.CreateHamburgerMenuImageItemsAsync();
			//	foreach (HamburgerMenuImageItem item in items)
			//	{
			//		Items.Add(item);
			//	}
			//});
		}

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
	}
}
