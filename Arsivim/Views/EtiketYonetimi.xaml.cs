using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class EtiketYonetimi : ContentPage
{
	public EtiketYonetimi(EtiketYonetimVM viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
} 