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
using YoutubeLiveBrowser.Entity;
using YoutubeLiveBrowser.Models;

using DotNetOpenAuth.OAuth2;

using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Threading;
using Google.Apis.Util.Store;

namespace YoutubeLiveBrowser.ViewModels
{
	class WebBrowserControlViewModel : ControlViewModelBase
	{
		YoutubeLiveController m_Controller;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public WebBrowserControlViewModel()
		{
			Height = 400;
			Width = 860;
		}

		public WebBrowserControlViewModel(string inAPIKey)
		{
			Height = 1080;
			Width = 1900;

			//ChannelId = "UCD-miitqNY3nyukJ4Fnf4_A";//いいんちょ
			ChannelId = "UC6oDys1BGgBsIC3WhG1BovQ";//しずりん
			//ChannelId = "UCv1fFr156jc65EMiLbaLImw";//rikiya
			//ChannelId = "UCsg-YqdqQ-KFF0LNk23BY4A";//でろ
			//ChannelId = "UC48jH1ul-6HOrcSSfoR02fQ";//リリ
			//ChannelId = "UCvmppcdYf4HOv-tFQhHHJMA";//もいもいUCNsidkYpIAQ4QaufptQBPHQ
			//ChannelId = "UCNsidkYpIAQ4QaufptQBPHQ";
			//Comments = new ObservableSynchronizedCollection<string>();
			
			m_Controller = new YoutubeLiveController(ChannelId, inAPIKey);
			Comments = new ObservableSynchronizedCollection<string>();

			IsProgressActive = true;
			BindingOperations.EnableCollectionSynchronization(Comments, new object());
			Action a = async () =>
			{
				var list = await m_Controller.GetSubscriptionNamesAsync();
				Subscriptions = new ObservableSynchronizedCollection<string>(list.Values);
				IsProgressActive = false;
			};
			a();
			
		}
		#endregion

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

		#region IsProgressActive
		private bool _IsProgressActive;

		public bool IsProgressActive
		{
			get
			{ return _IsProgressActive; }
			set
			{
				if (_IsProgressActive == value)
					return;
				_IsProgressActive = value;
				RaisePropertyChanged(nameof(IsProgressActive));
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

		#region Subscriptions
		private ObservableSynchronizedCollection<string> _Subscriptions;

		public ObservableSynchronizedCollection<string> Subscriptions
		{
			get
			{ return _Subscriptions; }
			set
			{
				if (_Subscriptions == value)
					return;
				_Subscriptions = value;
				RaisePropertyChanged(nameof(Subscriptions));
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
					_GetComment = new ViewModelCommand(GetChatCommentAsync);
				}
				return _GetComment;
			}
		}
		#endregion

		#region GetLiveStreamAsync
		/// <summary>
		/// GetLiveStreamAsync
		/// </summary>
		private async void GetLiveStreamAsync()
		{
			VideoId = string.Empty;
			VideoId = await m_Controller.GetStreamAsync();

			LiveChatId = string.Empty;
			LiveChatId = await m_Controller.GetChatIdAsync();
		}

		private async void GetChatCommentAsync()
		{
			//ここでコメントのコレクション変更を受け取って、画面に反映

			await Task.Run(async () => 
			{
				var oldComments = new Dictionary<string, YoutubeLiveComment>();
				while (true)
				{
					var newComments = await m_Controller.GetChatCommentAsync();

					foreach (var comment in newComments)
					{
						if (!oldComments.ContainsKey(comment.Key))
						{
							//Comments.Add(comment.Value.Comment);
							//CommentTimes.Add(comment.Value.PublishedAt.ToLongTimeString());
							Comments.Add(comment.Value.PublishedAt.ToLongTimeString() + ":" + comment.Value.CommentUserName + ":" + comment.Value.Comment);
							oldComments.Add(comment.Key, comment.Value);
						}
					}
				}
			});

		}
		#endregion

		private void GetChatComment()
		{
			m_Controller.GetChatComment();
			//ここでコメントのコレクション変更を受け取って、画面に反映
		}
	}
}
