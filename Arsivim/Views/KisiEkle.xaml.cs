using Arsivim.ViewModels;

namespace Arsivim.Views;

public partial class KisiEkle : ContentPage
{
    public KisiEkle(KisiEkleVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 