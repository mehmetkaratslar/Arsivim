using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class Gecmis : ContentPage
{
	public Gecmis(GecmisVM viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
} 