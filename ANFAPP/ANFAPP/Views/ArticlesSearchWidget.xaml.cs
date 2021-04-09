//reserva para load do ViewModel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ANFAPP.Pages.Articles;

using Xamarin.Forms;

namespace ANFAPP.Views
{
    public partial class ArticlesSearchWidget : ContentView
    {
        public delegate void SearchParamsRefreshedEvent(string searchValue);

		public event SearchParamsRefreshedEvent SearchParamsRefreshed;

        public static readonly BindableProperty SearchValueProperty = BindableProperty.Create<ArticlesSearchWidget, string>(p => p.SearchValue, null);
        public string SearchValue
        {
            get { return (string)GetValue(SearchValueProperty); }
            set { SetValue(SearchValueProperty, value); }
        }
        public ArticlesSearchWidget()
        {
            InitializeComponent();
            MainContainer.BindingContext = this;
        }

        public void OnSearchBoxInputSubmit(object sender, EventArgs args)
        {
            PerformSearch();
        }

        /// <summary>
        /// Perform a search
        /// </summary>
        protected void PerformSearch()
        {
           
            if (SearchParamsRefreshed != null) SearchParamsRefreshed(SearchValue);
        }

        public async void OnCategoryClick(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new ArticlesCategoryPage());
        }

    }
}
