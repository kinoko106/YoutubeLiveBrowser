using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using YoutubeLiveBrowser.ViewModels;

namespace YoutubeLiveBrowser.ViewModels
{
	class MainWindowViewModel : ControlViewModelBase
	{
		public WebBrowserControlViewModel WebBrowser;

		public MainWindowViewModel()
		{
			WebBrowser = new WebBrowserControlViewModel("https://www.youtube.com/watch?v=quuZ06TLy-E&ab_channel=%E6%9C%88%E3%83%8E%E7%BE%8E%E5%85%8E");

			Height = 600;
			Width = 900;
		}

		//#region WindowWidth
		//private int _WindowWidth;

		//public int WindowWidth
		//{
		//	get
		//	{ return _WindowWidth; }
		//	set
		//	{
		//		if (_WindowWidth == value)
		//			return;
		//		_WindowWidth = value;
		//		RaisePropertyChanged(nameof(WindowWidth));
		//	}
		//}
		//#endregion

		//#region WindowHeight
		//private int _WindowHeight;

		//public int WindowHeight
		//{
		//	get
		//	{ return _WindowHeight; }
		//	set
		//	{
		//		if (_WindowHeight == value)
		//			return;
		//		_WindowHeight = value;
		//		RaisePropertyChanged(nameof(WindowHeight));
		//	}
		//}
		//#endregion
	}
}