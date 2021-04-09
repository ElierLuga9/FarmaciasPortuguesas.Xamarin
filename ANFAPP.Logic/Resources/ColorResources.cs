using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace ANFAPP.Logic.Resources
{
        public static class ColorResources
        {

                // General Colors

                public static readonly Color ANFWhite = Color.FromHex("ffffff");
                public static readonly Color ANFGreen = Color.FromHex("76BD22");
                public static readonly Color ANFYellow = Color.FromHex("E9D600");
                public static readonly Color ANFDarkYellow = Color.FromHex("FDBA12"); //OLD - Color.FromHex("ffbf00"); 
                public static readonly Color ANFOrange = Color.FromHex("F15A24"); //OLD - Color.FromHex("ff8400");
                public static readonly Color ANFRed = Color.FromHex("DC0032");
                public static readonly Color ANFBlue = Color.FromHex("4AB4E6");
                public static readonly Color ANFPaleGrey = Color.FromHex("EEEEEE");
				public static readonly Color ANFDarkerPaleGrey = Color.FromHex("D6D6D6");
                public static readonly Color ANFDarkGrey = Color.FromHex("999999");
				
		        public static readonly Color ProdNotAvailable = Color.FromHex("dcdcdc");

                public static readonly Color ANFLighterBlue = Color.FromHex("ccf1f8");
                public static readonly Color ANFLightBlue = Color.FromHex("00bbda");
                public static readonly Color ANFLightGreen = Color.FromHex("BADE90");
                public static readonly Color ANFDarkBlue = Color.FromHex("2271b6");
                public static readonly Color ANFDarkOrange = Color.FromHex("f37032");
                public static readonly Color ANFLightGrey = Color.FromHex("F0F0F0");
				public static readonly Color ANFPink = Color.FromHex("e52f81");
				public static readonly Color ANFPalePink = Color.FromHex("e74e57");
				public static readonly Color ANFCream = Color.FromHex("f8f8f8");

                public static readonly Color ANFDarkerBlue = Color.FromHex("2C5697");

                public static readonly Color FacebookBlue = Color.FromHex("2a5f9d");

                public static readonly Color ANFCatalogYellow = Color.FromHex("f9bf00");
                public static readonly Color ANFCatalogPurple = Color.FromHex("59187c");
                public static readonly Color ANFCatalogBlue = Color.FromHex("00bbda");

                // Element Colors
                public static readonly Color APPBackgroundLight = Color.White;
                public static readonly Color APPBackgroundDark = Color.FromHex("A6A6A6"); //OLD - Color.FromHex("333333")
                public static readonly Color APPBackgroundGrey = Color.FromHex("EBEBEB");
                public static readonly Color LocatorBGLightGray = Color.FromHex("EFEFEF");

                public static readonly Color SchedulerBackgroundLight = Color.FromHex("f6f6f6");
                public static readonly Color SchedulerSeparatorColor = Color.FromHex("cecece");

                public static readonly Color SchedulerTabUnselectedColor = Color.FromHex("E1E1E1"); //grey


                // Pharmacy Card Colors
                public static readonly Color PharmacyCardPoints = Color.FromHex("2c5697");
                public static readonly Color PharmacyCardExpiringPoints = Color.FromHex("37b1ca");

                public static readonly Color PharmacyCardVoucher = Color.FromHex("ef7624");
                public static readonly Color PharmacyCardExpiringVoucher = Color.FromHex("f9bf00");

                // Text Colors
                public static readonly Color TextColorLight = APPBackgroundLight;
                public static readonly Color TextColorGrey = APPBackgroundDark;
                public static readonly Color TextColorDark = Color.FromHex("3A3A3A");

                public static readonly Color SchedulerTitleTextColor = Color.FromHex("303030");
                public static readonly Color SchedulerPPGreyN3 = Color.FromHex("f0f0f0");
                public static readonly Color SchedulerPPGreyN4 = Color.FromHex("979797");
                public static readonly Color SchedulerPPGreyN5 = Color.FromHex("e1e1e1");

                // Locator colors
                public static readonly Color LocatorBackgroundGrey = Color.FromHex("f2f2f2");
                public static readonly Color LocatorSeparatorColorDark = Color.FromHex("A6A6A6");
                public static readonly Color LocatorSeparatorColorLight = Color.FromHex("E1E1E1");
                public static readonly Color LocatorGrayText = LocatorSeparatorColorDark;
                public static readonly Color LocatorGreenText = Color.FromHex("76bd22");
                public static readonly Color LocatorGreenBackGroundLight = Color.FromHex("aedda8");
                public static readonly Color TestTansparencyColor = Color.FromRgba(216, 224, 227, 0.5);

                // Widget colors
                public static readonly Color WidgetSaudaActiveColor = Color.FromHex("f9bf00");

                //GetPoits Bar Color
                public static readonly Color PointsOrangeBar = ANFDarkOrange;
                public static readonly Color PointsYellowBar = ANFCatalogYellow;
                public static readonly Color PointsGreenBar = ANFGreen; 		

        }
}
