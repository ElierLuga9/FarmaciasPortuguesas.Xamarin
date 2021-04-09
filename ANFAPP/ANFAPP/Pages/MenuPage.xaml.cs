
using ANFAPP.Views;
namespace ANFAPP.Pages
{

	public partial class MenuPage : ANFPage
	{

		#region Page Initialization

		public MenuPage() : base() { }

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			MenuView.RenderView();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			MenuView.ReloadMenu();
		}

		#endregion

        

	}
}
