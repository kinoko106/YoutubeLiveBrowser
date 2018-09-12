using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using YoutubeLiveBrowser.Models;

namespace YoutubeLiveBrowser.ViewModels
{
	class ControlViewModelBase : ViewModel
	{
		//DB接続とYoutubeAPIの使用は全部のViewModelから実行できるようにする
		protected DataBaseAccess	m_DataBaseAccess;
		protected YoutubeApiService m_YoutubeApiService;

		public ControlViewModelBase()
		{
		}

		public ControlViewModelBase(DataBaseAccess inDataBaseAccess, YoutubeApiService inYoutubeApiService)
		{
			m_DataBaseAccess	= inDataBaseAccess;
			m_YoutubeApiService = inYoutubeApiService;
		}

		#region Width
		private int _Width;

		public int Width
		{
			get
			{ return _Width; }
			set
			{
				if (_Width == value)
					return;
				_Width = value;
				RaisePropertyChanged(nameof(Width));
			}
		}
		#endregion

		#region Height
		private int _Height;

		public int Height
		{
			get
			{ return _Height; }
			set
			{
				if (_Height == value)
					return;
				_Height = value;
				RaisePropertyChanged(nameof(Height));
			}
		}
		#endregion
		
	}
}
