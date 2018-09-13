using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using YoutubeLiveBrowser.Models;

namespace YoutubeLiveBrowser.ViewModels
{
	class MainWindowViewModel : ControlViewModelBase
	{
		DataBaseAccess dataBaseAccess;
		YoutubeApiService youtubeApiService;

		public MainWindowViewModel()
		{
			//WebBrowser = new WebBrowserControlViewModel(apikey);
			Height = 600;
			Width = 900;

			InitializeCommonModels();

			var key = System.Configuration.ConfigurationManager.AppSettings.AllKeys;
		}

		/// <summary>
		/// アプリケーション内で共通のオブジェクトを初期化
		/// </summary>
		private void InitializeCommonModels()
		{
			//DB接続用オブジェクト
			dataBaseAccess = new DataBaseAccess();

			//YoutubeApi用オブジェクト
			string ApiKey = ConfigurationManager.AppSettings["APIKey"];
			youtubeApiService = new YoutubeApiService(ApiKey);

			//コントロール用ViewModel
			SubscriptionHamburgerMenu = new SubscriptionHamburgerMenuViewModel(Height, Width, dataBaseAccess, youtubeApiService);
		}

		#region WebBrowser
		public WebBrowserControlViewModel _WebBrowser;

		public WebBrowserControlViewModel WebBrowser
		{
			get
			{ return _WebBrowser; }
			set
			{
				if (_WebBrowser == value)
					return;
				_WebBrowser = value;
				RaisePropertyChanged(nameof(WebBrowser));
			}
		}
		#endregion

		#region SubscriptionHamburgerMenu
		public SubscriptionHamburgerMenuViewModel _SubscriptionHamburgerMenu;

		public SubscriptionHamburgerMenuViewModel SubscriptionHamburgerMenu
		{
			get
			{ return _SubscriptionHamburgerMenu; }
			set
			{
				if (_SubscriptionHamburgerMenu == value)
					return;
				_SubscriptionHamburgerMenu = value;
				RaisePropertyChanged(nameof(SubscriptionHamburgerMenu));
			}
		}
		#endregion
	}
}