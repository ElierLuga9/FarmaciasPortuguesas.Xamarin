using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Utils
{
	public interface IMediaWriter
	{
		Task<string> WriteMedia(Stream source, string sourcePath);

		string WriteMedia (string sourcePath);

		string GetMediaRoot();
	}
}

