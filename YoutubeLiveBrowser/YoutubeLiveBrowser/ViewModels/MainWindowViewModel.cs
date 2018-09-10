using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using YoutubeLiveBrowser.Viewmodels;
using YoutubeLiveBrowser.ViewModels;

namespace YoutubeLiveBrowser.ViewModels
{
	class MainWindowViewModel : ControlViewModelBase
	{
		public MainWindowViewModel()
		{
			string apikey = "";
			WebBrowser = new WebBrowserControlViewModel(apikey);

			Height = 600;
			Width = 900;

			SubScriptionList = new ImageListBoxViewModel();

			var key = System.Configuration.ConfigurationManager.AppSettings.AllKeys;
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

		#region SubScriptionList
		public ImageListBoxViewModel _SubScriptionList;

		public ImageListBoxViewModel SubScriptionList
		{
			get
			{ return _SubScriptionList; }
			set
			{
				if (_SubScriptionList == value)
					return;
				_SubScriptionList = value;
				RaisePropertyChanged(nameof(SubScriptionList));
			}
		}
		#endregion
	}
}