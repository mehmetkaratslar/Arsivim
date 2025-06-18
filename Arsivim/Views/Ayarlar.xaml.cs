using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class Ayarlar : ContentPage
{
    public Ayarlar(AyarlarVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 