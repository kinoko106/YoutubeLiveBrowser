using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Livet;

namespace YoutubeLiveBrowser.ViewModels
{
	class WebBrowserControlViewModel : ControlViewModelBase
	{
		public WebBrowserControlViewModel()
		{
			Height = 400;
			Width = 860;
		}

		public WebBrowserControlViewModel(string inSourceURL)
		{
			Height = 1080;
			Width = 1900;

			SourceURL = inSourceURL;
		}

		#region SourceURL
		private string _SourceURL;

		public string SourceURL
		{
			get
			{ return _SourceURL; }
			set
			{
				if (_SourceURL == value)
					return;
				_SourceURL = value;
				RaisePropertyChanged(nameof(SourceURL));
			}
		}
		#endregion

	}
}
