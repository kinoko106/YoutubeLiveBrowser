using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace YoutubeLiveBrowser.ViewModels
{
	class VideoInfoViewModel : ControlViewModelBase
	{
		public VideoInfoViewModel()
		{
			Width  = 900;
			Height = 580;
			Header = "Video Title";
			IsOpen = false;
		}

		#region Header
		private string _Header;

		public string Header
		{
			get
			{ return _Header; }
			set
			{
				if (_Header == value)
					return;
				_Header = value;
				RaisePropertyChanged(nameof(Header));
			}
		}
		#endregion

		#region IsOpen
		private bool _IsOpen;

		public bool IsOpen
		{
			get
			{ return _IsOpen; }
			set
			{
				if (_IsOpen == value)
					return;
				_IsOpen = value;
				RaisePropertyChanged(nameof(IsOpen));
			}
		}
		#endregion

		#region Thumbnail
		private BitmapImage _Thumbnail;

		public BitmapImage Thumbnail
		{
			get
			{ return _Thumbnail; }
			set
			{
				if (_Thumbnail == value)
					return;
				_Thumbnail = value;
				RaisePropertyChanged(nameof(Thumbnail));
			}
		}
		#endregion

		#region CloseButtonClicked
		private ViewModelCommand _CloseButtonClicked;

		public ViewModelCommand CloseButtonClicked
		{
			get
			{
				if (_CloseButtonClicked == null)
				{
					_CloseButtonClicked = new ViewModelCommand(ChangeOpenState);
				}
				return _CloseButtonClicked;
			}
		}
		#endregion

		private void ChangeOpenState()
		{
			//バインドできていない
			IsOpen = false;
		}
	}
}
