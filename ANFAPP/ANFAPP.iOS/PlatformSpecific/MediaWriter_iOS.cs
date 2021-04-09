using System;
using System.IO;
using ANFAPP.Logic.Utils;

using System.Threading.Tasks;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency (typeof(ANFAPP.iOS.PlatformSpecific.MediaWriter_iOS))]
namespace ANFAPP.iOS.PlatformSpecific
{
	public class MediaWriter_iOS : IMediaWriter
	{
		public Task<string> WriteMedia(Stream source, string sourcePath)
		{
			return null;
		}
		public string WriteMedia (string sourcePath)
		{
			string dst = null;
			if (null == sourcePath) return dst;

			string filename = Path.GetFileName(sourcePath);
			dst = Path.Combine(GetMediaRoot(), filename);
			File.Copy (sourcePath, dst);

			return dst;
		}

		public string GetMediaRoot()
		{
			string root = null;

			int SystemVersion = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
			if (SystemVersion >= 8)
			{
				root = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
			}
			else
			{
				root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // iOS 7 and earlier
			}

			return root;
		}
	}
}

