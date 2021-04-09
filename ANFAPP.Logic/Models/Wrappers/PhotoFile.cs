using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;

namespace ANFAPP.Logic.Models.Wrappers
{
	public class PhotoFile
	{

		#region Properties

		public MediaFile File { get; set; }

		public string Filename
		{
			get
			{
				return (File != null && !string.IsNullOrEmpty(File.Path)) ?
					File.Path.Substring(File.Path.LastIndexOf("/") + 1).Replace(".jpg", string.Empty) :
					null;
			}
		}

		public string MimeType
		{
			get
			{
				if (File != null && !string.IsNullOrEmpty(File.Path)) 
				{
					var type = File.Path.Substring(File.Path.LastIndexOf(".") + 1);
					return "image/" + type;
				}

				return null;
			}
		}

		#endregion

		#region Constructors

		public PhotoFile(MediaFile file)
		{
			File = file;
		}

		#endregion

	}
}
