using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class BelgeEkle : ContentPage
{
    public BelgeEkle(BelgeEkleVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 