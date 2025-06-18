using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace Arsivim.Platforms.Windows
{
    public class WindowsSpecificService
    {
        public static void ConfigureWindow(Microsoft.UI.Xaml.Window window)
        {
            if (window == null) return;

            try
            {
                // Pencere başlığını ayarla
                window.Title = "Arsivim - Belge Arşiv Sistemi";
                
                // Basit pencere boyutu ayarlama
                if (window.AppWindow != null)
                {
                    window.AppWindow.Resize(new global::Windows.Graphics.SizeInt32(1200, 800));
                }
                
                // Custom title bar (isteğe bağlı)
                SetupCustomTitleBar(window);
                
                // File drop desteği
                SetupFileDrop(window);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Windows pencere konfigürasyonu hatası: {ex.Message}");
            }
        }

        public static void SetupCustomTitleBar(Microsoft.UI.Xaml.Window window)
        {
            try
            {
                // Custom title bar ayarları
                window.ExtendsContentIntoTitleBar = false;
                
                if (window.AppWindow != null)
                {
                    var titleBar = window.AppWindow.TitleBar;
                    titleBar.ExtendsContentIntoTitleBar = false;
                    
                    // Title bar renklerini ayarla
                    titleBar.BackgroundColor = Microsoft.UI.Colors.DarkSlateBlue;
                    titleBar.ForegroundColor = Microsoft.UI.Colors.White;
                    titleBar.InactiveBackgroundColor = Microsoft.UI.Colors.Gray;
                    titleBar.InactiveForegroundColor = Microsoft.UI.Colors.LightGray;
                    
                    // Button colors
                    titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                    titleBar.ButtonForegroundColor = Microsoft.UI.Colors.White;
                    titleBar.ButtonHoverBackgroundColor = Microsoft.UI.Colors.LightBlue;
                    titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.Black;
                    titleBar.ButtonPressedBackgroundColor = Microsoft.UI.Colors.Blue;
                    titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.White;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Title bar konfigürasyonu hatası: {ex.Message}");
            }
        }

        public static void SetupFileDrop(Microsoft.UI.Xaml.Window window)
        {
            try
            {
                if (window.Content is FrameworkElement rootElement)
                {
                    rootElement.AllowDrop = true;
                    
                    rootElement.DragOver += (s, e) =>
                    {
                        e.AcceptedOperation = global::Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
                        e.Handled = true;
                    };

                    rootElement.Drop += async (s, e) =>
                    {
                        try
                        {
                            if (e.DataView.Contains(global::Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
                            {
                                var items = await e.DataView.GetStorageItemsAsync();
                                
                                foreach (var item in items.OfType<global::Windows.Storage.StorageFile>())
                                {
                                    await HandleDroppedFile(item);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Dosya drop hatası: {ex.Message}");
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"File drop konfigürasyonu hatası: {ex.Message}");
            }
        }

        private static async Task HandleDroppedFile(global::Windows.Storage.StorageFile file)
        {
            try
            {
                // Desteklenen dosya türlerini kontrol et
                var supportedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".pptx", ".jpg", ".jpeg", ".png", ".tiff", ".bmp" };
                var extension = Path.GetExtension(file.Name)?.ToLower();
                
                if (extension != null && supportedExtensions.Contains(extension))
                {
                    // BelgeEkle sayfasına git ve dosyayı parametre olarak gönder
                    var filePath = file.Path;
                    await Shell.Current.GoToAsync($"BelgeEkle?dosyaYolu={Uri.EscapeDataString(filePath)}");
                    
                    // Bildirim göster
                    await ShowNotification($"'{file.Name}' dosyası yüklendi!");
                }
                else
                {
                    await ShowNotification($"'{extension}' dosya türü desteklenmiyor.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Dropped dosya işleme hatası: {ex.Message}");
                await ShowNotification("Dosya işlenirken bir hata oluştu.");
            }
        }

        public static async Task ShowNotification(string message)
        {
            try
            {
                var currentApp = Microsoft.Maui.Controls.Application.Current;
                if (currentApp?.MainPage != null)
                {
                    // Toast benzeri bildirim yerine basit alert kullan
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await currentApp.MainPage.DisplayAlert("Bildirim", message, "Tamam");
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Bildirim gösterme hatası: {ex.Message}");
            }
        }

        public static void SetupContextMenus()
        {
            // Context menu ayarları - gelecekte XAML dosyalarında kullanılacak
            // Bu method ViewModeller tarafından çağrılabilir
        }

        public static async Task<bool> ShowConfirmationDialog(string title, string message)
        {
            try
            {
                var currentApp = Microsoft.Maui.Controls.Application.Current;
                if (currentApp?.MainPage != null)
                {
                    return await currentApp.MainPage.DisplayAlert(title, message, "Evet", "Hayır");
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Confirmation dialog hatası: {ex.Message}");
                return false;
            }
        }

        public static async Task ShowErrorDialog(string title, string message)
        {
            try
            {
                var currentApp = Microsoft.Maui.Controls.Application.Current;
                if (currentApp?.MainPage != null)
                {
                    await currentApp.MainPage.DisplayAlert(title, message, "Tamam");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error dialog hatası: {ex.Message}");
            }
        }

        public static void OpenFileInExplorer(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        Arguments = $"/select,\"{filePath}\"",
                        FileName = "explorer.exe"
                    };
                    Process.Start(startInfo);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Explorer açma hatası: {ex.Message}");
            }
        }

        public static void OpenFolderInExplorer(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = folderPath,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Klasör açma hatası: {ex.Message}");
            }
        }
    }
} 