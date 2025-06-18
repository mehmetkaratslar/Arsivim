using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class AnaSayfa : ContentPage
{
    public AnaSayfa(AnaSayfaVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 