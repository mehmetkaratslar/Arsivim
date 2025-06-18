using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class Istatistikler : ContentPage
{
	public Istatistikler(IstatistiklerVM viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
} 