using ANFAPP.Logic.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(ANFAPP.WinPhone.PlatformSpecific.MediaWriter_WP))]
namespace ANFAPP.WinPhone.PlatformSpecific
{
	public class MediaWriter_WP : IMediaWriter
	{
		public string WriteMedia(string sourcePath)
		{
			return null;
		}

		public async Task<string> WriteMedia(Stream source, string sourcePath)
		{
			string filename = Path.GetFileName(sourcePath);
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			StorageFile storageFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

			using (Stream outputStream = await storageFile.OpenStreamForWriteAsync())
			{
				await source.CopyToAsync(outputStream);
				outputStream.Close();
				source.Close();
			}

			return storageFile.Path;
		}

		public string GetMediaRoot()
		{
			return ApplicationData.Current.LocalFolder.Path;			
		}
	}
}
