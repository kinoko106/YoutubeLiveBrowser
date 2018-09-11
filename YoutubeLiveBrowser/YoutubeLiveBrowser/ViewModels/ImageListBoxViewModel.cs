using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoutubeLiveBrowser.ViewModels;

namespace YoutubeLiveBrowser.ViewModels
{
	class ImageListBoxViewModel : ControlViewModelBase
	{
		public ImageListBoxViewModel()
		{
			Width = 100;
			Height = 200;

			ImageItems = new ObservableSynchronizedCollection<ImageSource>();
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\syokaku1.jpg");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\syokaku2.png");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\syokaku3.png");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\zui.jpg");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\kashima.png");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\suzuya.jpg");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\agano.png");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\syokaku2.png");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\syokaku2.png");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\syokaku2.png");
			AddItem(@"C:\Users\03dai\source\repos\YoutubeLiveBrowser\YoutubeLiveBrowser\YoutubeLiveBrowser\bin\Debug\syokaku2.png");
		}

		#region ImageItems
		private ObservableSynchronizedCollection<ImageSource> _ImageItems;

		public ObservableSynchronizedCollection<ImageSource> ImageItems
		{
			get
			{ return _ImageItems; }
			set
			{
				if (_ImageItems == value)
					return;
				_ImageItems = value;
				RaisePropertyChanged(nameof(ImageItems));
			}
		}
		#endregion

		public void AddItem(string inResourceURL)
		{
			var source = new BitmapImage();
			source.BeginInit();
			source.UriSource = new Uri(inResourceURL);
			source.EndInit();

			ImageItems.Add(source);
		}
	}
}
