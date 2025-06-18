using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class BelgeListesi : ContentPage
{
    public BelgeListesi(BelgeListeVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 