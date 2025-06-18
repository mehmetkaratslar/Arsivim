using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class KullaniciYonetimi : ContentPage
{
	public KullaniciYonetimi(KullaniciYonetimVM viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
} 