using Livet.Commands;
using Livet.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeLiveBrowser.Models;
using YoutubeLiveBrowser.ViewModels;

namespace YoutubeLiveBrowser.ViewModels
{
	class VideoItemViewModel : ControlViewModelBase
	{
		VideoItemModel _Model; 

		public VideoItemViewModel()
		{
			_Model = new VideoItemModel();

			//よくわからない modelに通知できていない
			var modelListener = new PropertyChangedEventListener(_Model,(sender ,e)=> RaisePropertyChanged(()=> this.Text));
			base.CompositeDisposable.Add(modelListener);
		}

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

		public ViewModelCommand _ChangeText;
		public ViewModelCommand ChangeText
		{
			get
			{
				if (_ChangeText == null)
				{
					_ChangeText = new ViewModelCommand(() => { _Model.Text = "bbbbb";});
				}
				return _ChangeText;
			}
		}
	}
}
