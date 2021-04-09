using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using UIKit;
using ANFAPP.iOS.Renderer;
using ANFAPP.Views.Common;

[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]
namespace ANFAPP.iOS.Renderer
{
	public class CustomViewCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell,tv);
			if (cell != null) {
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			}

			return cell;
		}
	}
}

