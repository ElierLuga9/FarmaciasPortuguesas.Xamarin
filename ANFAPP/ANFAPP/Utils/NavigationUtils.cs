using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Utils
{
    public static class NavigationUtils
    {

		/// <summary>
		/// Returns the page on top of the stack
		/// </summary>
		/// <param name="navigation"></param>
		/// <returns></returns>
		public static Page GetPageOnTop(INavigation navigation)
		{
			if (navigation == null || navigation.NavigationStack == null || navigation.NavigationStack.Count == 0) return null;
			return navigation.NavigationStack[navigation.NavigationStack.Count - 1];

		}

        /// <summary>
        /// Check if the navigation stack contains a page of the referenced type.
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="navStack"></param>
        /// <returns></returns>
        public static bool ContainsPageOfType(Type pageType, IReadOnlyList<Page> navStack)
        {
            if (navStack == null || pageType == null) return false;

            // Search the stack
            foreach (var page in navStack)
            {
                if (page.GetType() == pageType) return true;
            }

            return false;
        }

        /// <summary>
        /// Pop the navigation stack to the referenced page.
        /// </summary>
        /// <param name="page"></param>
        public static void PopToPageType(Type page, INavigation navigation)
        {
            if (page == null || navigation == null) return;

            var navStack = navigation.NavigationStack.ToList();
            if (navStack == null || navStack.Count <= 1) return;

			if (navStack.Count > 1) 
			{ 
				// Current position to pop.
				int currPos = navStack.Count - 2;            
				while (currPos > 0 && navStack[currPos].GetType() != page) {
					navigation.RemovePage(navStack[currPos]);
					currPos--;
				}
			}

			navigation.PopAsync();
        }

        /// <summary>
        /// Pop the navigation stack to the referenced page.
        /// </summary>
        /// <param name="page"></param>
        public static async Task PopToPage(Page page, INavigation navigation)
        {
            if (page == null || navigation == null) return;

            var navStack = navigation.NavigationStack.ToList();
            if (navStack == null || navStack.Count <= 1) return;

			if (navStack.Count > 1)
			{
				// Current position to pop.
				int currPos = navStack.Count - 2;
				while (currPos > 0 && !navStack[currPos].Equals(page))
				{
					navigation.RemovePage(navStack[currPos]);
					currPos--;
				}
			}

			await navigation.PopAsync();
        }

        //testes
        public static async Task PushPageKeepHistory(Page page, INavigation navigation)
        {
            if (page == null || navigation == null) return;
            //navigation.InsertPageBefore(page, navigation.NavigationStack[navigation.NavigationStack.Count ]);
            await navigation.PushAsync(page);
        }

		/// <summary>
		/// Push the page without keeping history of the previous page.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="navigation"></param>
		public static async Task PushPageWithNoHistory(Page page, INavigation navigation)
		{
			if (page == null || navigation == null) return;

			navigation.InsertPageBefore(page, navigation.NavigationStack[navigation.NavigationStack.Count - 1]);
			await navigation.PopAsync();

			// XXX: Implemenetation crashing in Windows Phone.
			// get the last page
			//var lastPage = navigation.NavigationStack[navigation.NavigationStack.Count - 1];

			//await navigation.PushAsync(page);
			//if (lastPage != null) navigation.RemovePage(lastPage);
		}

		/// <summary>
		/// Push a page and clear the navigation history
		/// </summary>
		/// <param name="page"></param>
		/// <param name="navigation"></param>
		/// <returns></returns>
		public static async Task PushPageAndClearHistory(Page page, INavigation navigation)
		{
			navigation.InsertPageBefore(page, navigation.NavigationStack[0]);
			await PopToPage(page, navigation);
		}

		/// <summary>
		/// Push a page and clear the navigation history if the page is not on top of the navigation stack.
		/// </summary>
		/// <param name="page">Page.</param>
		/// <param name="navigation">Navigation.</param>
		public static void PushPageAndClearIfNotActive(Page page, INavigation navigation)
		{
			if (page == null || navigation == null) return;

			// get the last page
			var lastPage = navigation.NavigationStack[navigation.NavigationStack.Count - 1];

			if (!lastPage.GetType ().Equals (page.GetType ())) {
				PushPageAndClearHistory (page, navigation);
			}
		}

		/// <summary>
		/// Returns true if the top page is of the referenced type
		/// </summary>
		/// <param name="type"></param>
		/// <param name="navigation"></param>
		/// <returns></returns>
		public static bool TopPageOfType(Type type, INavigation navigation)
		{
			var count = navigation.NavigationStack.Count;
			return count > 0 && navigation.NavigationStack[count - 1].GetType() == type;
		}

    }
}
