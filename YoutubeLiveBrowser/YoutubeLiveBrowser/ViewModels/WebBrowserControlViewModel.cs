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

			IsGetCommentEnable = false;
			m_Controller = new YoutubeLiveController(ChannelId, inAPIKey);
			Comments = new ObservableSynchronizedCollection<string>();
			BindingOperations.EnableCollectionSynchronization(Comments, new object());

			Action a = async () =>
			{
				await GetYoutubeLiveStreamInfos();
				GetSubscriptionNames();
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

		#region SelectedChannelId
		private string _SelectedChannelId;

		public string SelectedChannelId
		{
			get
			{ return _SelectedChannelId; }
			set
			{
				if (_SelectedChannelId == value)
					return;
				_SelectedChannelId = value;
				RaisePropertyChanged(nameof(SelectedChannelId));
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

		#region LiveChatIds
		/// <summary>
		/// 放送中YoutubeLiveのチャットID
		/// </summary>
		private ObservableSynchronizedCollection<string> _LiveChatIds;

		public ObservableSynchronizedCollection<string> LiveChatIds
		{
			get
			{ return _LiveChatIds; }
			set
			{
				if (_LiveChatIds == value)
					return;
				_LiveChatIds = value;
				RaisePropertyChanged(nameof(LiveChatIds));
			}
		}
		#endregion

		#region VideoIds
		/// <summary>
		/// 放送中の動画ID
		/// </summary>
		private ObservableSynchronizedCollection<string> _VideoIds;

		public ObservableSynchronizedCollection<string> VideoIds
		{
			get
			{ return _VideoIds; }
			set
			{
				if (_VideoIds == value)
					return;
				_VideoIds = value;
				RaisePropertyChanged(nameof(VideoIds));
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

		//IsGetCommentEnable

		#region IsGetCommentEnable
		/// <summary>
		/// コメント取得ボタンが有効か
		/// </summary>
		private bool _IsGetCommentEnable;
		public bool IsGetCommentEnable
		{
			get
			{ return _IsGetCommentEnable; }
			set
			{
				if (_IsGetCommentEnable == value)
					return;
				_IsGetCommentEnable = value;
				RaisePropertyChanged(nameof(IsGetCommentEnable));
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
					_SubmitChannelId = new ViewModelCommand(()=> { });
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

		#region SubscriptionSelectedChanged
		private ViewModelCommand _SubscriptionSelectedChanged = null;

		public ViewModelCommand SubscriptionSelectedChanged
		{
			get
			{
				if (_SubscriptionSelectedChanged == null)
				{
					_SubscriptionSelectedChanged = new ViewModelCommand(SelectChannelId);
				}
				return _SubscriptionSelectedChanged;
			}
		}
		#endregion

		#region GetLiveStreamAsync
		/// <summary>
		/// GetLiveStreamAsync
		/// </summary>
		private async Task GetYoutubeLiveStreamInfos()
		{
			IsProgressActive = true;
			await m_Controller.GetYoutubeLiveStreamInfos(true);
			IsProgressActive = false;
		}
		#endregion

		#region GetChatCommentAsync
		/// <summary>
		/// GetChatCommentAsync
		/// </summary>
		private async void GetChatCommentAsync()
		{
			//ここでコメントのコレクション変更を受け取って、画面に反映
			try
			{
				await Task.Run(async () =>
				{
					var oldComments = new List<string>();
					while (true)
					{
						var comments = await m_Controller.GetChatCommentAsync(LiveChatId);
						oldComments = comments;
						foreach (var comment in comments)
						{
							Comments.Add(comment);
						}
					}
				});
			}
			catch(Exception e)
			{
				string errorMessage = e.Message;
				string errorSource = e.Source;

				Comments.Add(errorMessage + "\n" + errorSource);
			}
		}

		#endregion

		#region GetSubscriptions
		/// <summary>
		/// 自分が登録しているチャンネルのチャンネルID
		/// </summary>
		private void GetSubscriptionNames()
		{
			var list = m_Controller.GetChannelNames();

			Subscriptions = new ObservableSynchronizedCollection<string>(list);
			SelectedChannelId = list.First();
		}
		#endregion

		//private async Task<List<string>> GetChatIdAsync()
		//{

		//}
		private async void SelectChannelId()
		{
			var infos = await m_Controller.GetYoutubeLiveStreamInfos(false);
			if (infos == null) { return; }

			LiveChatId = infos.Where(x => x.ChannelName == SelectedChannelId)?.Select(x => x.LiveChatId).First();
			VideoId = infos.Where(x => x.ChannelName == SelectedChannelId)?.Select(x => x.VideoId).First();
		}
	}
}
