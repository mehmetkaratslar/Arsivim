using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class BelgeDetay : ContentPage
{
    public BelgeDetay(BelgeDetayVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 