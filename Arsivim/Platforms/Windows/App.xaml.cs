using Microsoft.UI.Xaml;
using WinUIEx;
using Arsivim.Platforms.Windows;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Arsivim.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		this.InitializeComponent();
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);
		
		        var currentApp = Microsoft.Maui.Controls.Application.Current;
        if (currentApp?.Windows?.FirstOrDefault()?.Handler?.PlatformView is Microsoft.UI.Xaml.Window window)
        {
            // Windows özel ayarları
            window.Title = "Arsivim - Belge Arşiv Sistemi";
            
            // Pencere yönetimi için WinUIEx kullan
            WindowsSpecificService.ConfigureWindow(window);
			
			// Window state'i ayarla
			window.SetWindowPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Default);
			
			// System tray icon (gelecekte eklenebilir)
			// SetupSystemTrayIcon();
		}
		
		// Klavye kısayolları için event handler
		SetupKeyboardAccelerators();
	}

	private void SetupKeyboardAccelerators()
	{
		try
		        {
            var currentApp = Microsoft.Maui.Controls.Application.Current;
            if (currentApp?.Windows?.FirstOrDefault()?.Handler?.PlatformView is Microsoft.UI.Xaml.Window window &&
                window.Content is FrameworkElement rootElement)
			{
				// Ctrl+N - Yeni belge
				var newDocAccelerator = new Microsoft.UI.Xaml.Input.KeyboardAccelerator
				{
					Key = Windows.System.VirtualKey.N,
					Modifiers = Windows.System.VirtualKeyModifiers.Control
				};
				newDocAccelerator.Invoked += async (s, e) =>
				{
					await Shell.Current.GoToAsync("//BelgeEkle");
				};
				rootElement.KeyboardAccelerators.Add(newDocAccelerator);

				// Ctrl+F - Arama
				var searchAccelerator = new Microsoft.UI.Xaml.Input.KeyboardAccelerator
				{
					Key = Windows.System.VirtualKey.F,
					Modifiers = Windows.System.VirtualKeyModifiers.Control
				};
				searchAccelerator.Invoked += async (s, e) =>
				{
					await Shell.Current.GoToAsync("//BelgeListesi");
				};
				rootElement.KeyboardAccelerators.Add(searchAccelerator);

				// Ctrl+S - Ayarlar
				var settingsAccelerator = new Microsoft.UI.Xaml.Input.KeyboardAccelerator
				{
					Key = Windows.System.VirtualKey.S,
					Modifiers = Windows.System.VirtualKeyModifiers.Control | Windows.System.VirtualKeyModifiers.Shift
				};
				settingsAccelerator.Invoked += async (s, e) =>
				{
					await Shell.Current.GoToAsync("//Ayarlar");
				};
				rootElement.KeyboardAccelerators.Add(settingsAccelerator);

				// F1 - Yardım (gelecekte eklenebilir)
				var helpAccelerator = new Microsoft.UI.Xaml.Input.KeyboardAccelerator
				{
					Key = Windows.System.VirtualKey.F1
				};
				helpAccelerator.Invoked += (s, e) =>
				{
					ShowHelpDialog();
				};
				rootElement.KeyboardAccelerators.Add(helpAccelerator);
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Klavye kısayolları ayarlanırken hata: {ex.Message}");
		}
	}

	private async void ShowHelpDialog()
	{
		        try
        {
            var currentApp = Microsoft.Maui.Controls.Application.Current;
            if (currentApp?.MainPage != null)
            {
                await currentApp.MainPage.DisplayAlert(
					"Klavye Kısayolları",
					"Ctrl+N - Yeni Belge Ekle\n" +
					"Ctrl+F - Belge Arama\n" +
					"Ctrl+Shift+S - Ayarlar\n" +
					"F1 - Bu yardım penceresi",
					"Tamam");
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Yardım dialog hatası: {ex.Message}");
		}
	}
}

