using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Livet;
using Livet.Commands;
using YoutubeLiveBrowser.Models;

namespace YoutubeLiveBrowser.ViewModels
{
	class WebBrowserControlViewModel : ControlViewModelBase
	{
		YoutubeLiveController m_Controller;

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
			//ChannelId = "UCD-miitqNY3nyukJ4Fnf4_A";//いいんちょ
			ChannelId = "UC6oDys1BGgBsIC3WhG1BovQ";//しずりん
			//ChannelId = "UCv1fFr156jc65EMiLbaLImw";//rikiya
			//ChannelId = "UCsg-YqdqQ-KFF0LNk23BY4A";//でろ
			//ChannelId = "UC48jH1ul-6HOrcSSfoR02fQ";//リリ
			//ChannelId = "UCvmppcdYf4HOv-tFQhHHJMA";//もいもい
			Comment = "test";
			Comments = new ObservableSynchronizedCollection<string>();
			BindingOperations.EnableCollectionSynchronization(Comments, new object());

			m_Controller = new YoutubeLiveController(ChannelId);
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

		#region ChannelId
		private string _ChannelId;

		public string ChannelId
		{
			get
			{ return _ChannelId; }
			set
			{
				if (_ChannelId == value)
					return;
				_ChannelId = value;
				RaisePropertyChanged(nameof(ChannelId));
			}
		}
		#endregion

		#region VideoId
		private string _VideoId;

		public string VideoId
		{
			get
			{ return _VideoId; }
			set
			{
				if (_VideoId == value)
					return;
				_VideoId = value;
				RaisePropertyChanged(nameof(VideoId));
			}
		}
		#endregion

		#region LiveChatId
		private string _LiveChatId;

		public string LiveChatId
		{
			get
			{ return _LiveChatId; }
			set
			{
				if (_LiveChatId == value)
					return;
				_LiveChatId = value;
				RaisePropertyChanged(nameof(LiveChatId));
			}
		}
		#endregion

		#region Comment
		private string _Comment;

		public string Comment
		{
			get
			{ return _Comment; }
			set
			{
				if (_Comment == value)
					return;
				_Comment = value;
				RaisePropertyChanged(nameof(Comment));
			}
		}
		#endregion

		#region Comments
		private ObservableSynchronizedCollection<string> _Comments;

		public ObservableSynchronizedCollection<string> Comments
		{
			get
			{ return _Comments; }
			set
			{
				if (_Comments == value)
					return;
				_Comments = value;
				RaisePropertyChanged(nameof(Comments));
			}
		}
		#endregion

		#region SubmitChannelId
		private ViewModelCommand _SubmitChannelId = null;

		public ViewModelCommand SubmitChannelId
		{
			get
			{
				if (_SubmitChannelId == null)
				{
					_SubmitChannelId = new ViewModelCommand(GetLiveStreamAsync);
				}
				return _SubmitChannelId;
			}
		}
		#endregion

		#region GetComment
		private ViewModelCommand _GetComment = null;

		public ViewModelCommand GetComment
		{
			get
			{
				if (_GetComment == null)
				{
					_GetComment = new ViewModelCommand(GetChatComment);
				}
				return _GetComment;
			}
		}
		#endregion

		private async void GetLiveStreamAsync()
		{
			VideoId = string.Empty;
			VideoId = await m_Controller.GetStreamAsync();

			LiveChatId = string.Empty;
			LiveChatId = await m_Controller.GetChatIdAsync();
		}

		private void GetChatComment()
		{
			m_Controller.GetChatComment();
			//ここでコメントのコレクション変更を受け取って、画面に反映
		}

		public async void GetChatCommentTest()
		{
			await Task.Run(() =>
				{
					for (int i = 0; i < 100; i++)
					{
						//System.Threading.Thread.Sleep(500);
						Comments.Add("aaa");
					}
				}
			);
			////別スレッドでコメント取得・更新したい
			//Comments = new ObservableSynchronizedCollection<string>();
			//BindingOperations.EnableCollectionSynchronization(Comments, new object());
			//Action a = async () =>
			//{
			//	await Task.Run(() =>
			//	{
			//		for (int i = 0; i < 10; i++)
			//		{
			//			System.Threading.Thread.Sleep(100);
			//			Comments.Add("aaa");
			//		}
			//	}
			//	);
			//};
			//a();
		}
	}
}
