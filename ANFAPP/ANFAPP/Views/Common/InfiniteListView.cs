using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
	public class InfiniteListView : ListView
	{
		public static readonly BindableProperty LoadMoreCommandProperty = //BindableProperty.Create<InfiniteListView, ICommand>(bp => bp.LoadMoreCommand, default(ICommand));
																			BindableProperty.Create(nameof(InfiniteListView), typeof(ICommand), typeof(InfiniteListView), default(ICommand));		
		public ICommand LoadMoreCommand
		{
			get { return (ICommand) GetValue(LoadMoreCommandProperty); }
			set { SetValue(LoadMoreCommandProperty, value); }
		}

		public InfiniteListView()
		{
			ItemAppearing += InfiniteListView_ItemAppearing;
		}

		void InfiniteListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
		{
			var items = ItemsSource as IList;
			if (items == null || items.Count == 0) return;

			object item = null;
			if (IsGroupingEnabled)
			{
				if (!(items[items.Count - 1] is IList)) return;
				var groupedItems = items[items.Count - 1] as IList;
				if (groupedItems.Count == 0) return;

				// Grouped list
				item = groupedItems[Math.Max(0, groupedItems.Count - 4)];
			}
			else
			{
				// Simple list
				item = items[Math.Max(0, items.Count - 4)];
			}

			if (e.Item == item)
			{
				if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
					LoadMoreCommand.Execute(null);
			} 
		}
	}
}
