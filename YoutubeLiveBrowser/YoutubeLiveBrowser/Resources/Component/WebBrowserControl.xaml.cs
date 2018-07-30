using System;
using System.Collections.Generic;
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

			//var axIWebBrowser2 = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
			//var comObj = axIWebBrowser2.GetValue(browser, null);

			//// JavaScriptのエラー表示を抑止する
			//comObj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, comObj, new object[] { true });
		}
	}
}
