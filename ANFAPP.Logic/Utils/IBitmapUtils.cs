using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Utils
{
	public interface IBitmapUtils
	{

		byte[] ResizeImage(byte[] image, double width, double height);

	}
}
