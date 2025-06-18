using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class KisiListesi : ContentPage
{
    public KisiListesi(KisiListeVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 