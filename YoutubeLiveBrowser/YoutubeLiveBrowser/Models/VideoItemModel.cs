using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeLiveBrowser.Models
{
	class VideoItemModel : NotificationObject
	{
		public VideoItemModel()
		{
			Width = 210;
			Height = 320;
			Text = "aaa";
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

		#region Text
		private string _Text;

		public string Text
		{
			get
			{ return _Text; }
			set
			{
				if (_Text == value)
					return;
				_Text = value;
				RaisePropertyChanged(nameof(Text));
			}
		}
		#endregion

	}
}
