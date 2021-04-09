using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Logic.Resources
{
    public class DimenResources
    {

        public static readonly double ApplicationBarHeight = 50;
        public static readonly double AppFooterTextSize = Device.OnPlatform(11, 9, 9);
        public static readonly double ApplicationBarHeightByDevice = Device.OnPlatform(70, 50, 50);
		public static readonly GridLength ApplicationBarDummy= Device.OnPlatform(70, 50, 50);

        public static readonly double MenuBarHeight = 80;
        public static readonly double MenuBarHeightByDevice = Device.OnPlatform(100, 80, 80);
        public static readonly Thickness MenuBarPadding = Device.OnPlatform(new Thickness(15, 30, 15, 10), new Thickness(15, 10), new Thickness(15, 10));

		public static readonly double BannerHeight = Device.Idiom == TargetIdiom.Phone ? (Device.OS == TargetPlatform.WinPhone ? 210 : 150) : 270;
		public static readonly double VideoHeight = Device.Idiom == TargetIdiom.Phone ? 190 : 300;

		public static readonly GridLength BannerHeightLength = new GridLength(DimenResources.BannerHeight);

		public static readonly double SaudaWidgetHeight = (Device.OS == TargetPlatform.WinPhone ? 110 : 75);

        public static readonly double PharmacyWidgetHeight = 89;

		public static readonly double TabletSinglePageWidth = 480;
    }
}
