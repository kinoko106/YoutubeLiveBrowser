using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YoutubeLiveBrowser.Resources.Component
{
	/// <summary>
	/// WebBrowserControl.xaml の相互作用ロジック
	/// </summary>
	public partial class WebBrowserControl : UserControl
	{
		public WebBrowserControl()
		{
			this.InitializeComponent();
		}

		//private void ListBox_TargetUpdated(object sender, DataTransferEventArgs e)
		//{
		//	(CommentList.ItemsSource as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(ListBox_CollectionChanged);
		//}
		//void ListBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		//{
		//	try
		//	{
		//		var comment = CommentList.Items[CommentList.Items.Count - 1];
		//		CommentList.ScrollIntoView(comment);
		//	}
		//	catch
		//	{

		//	}
		//}
	}
}
