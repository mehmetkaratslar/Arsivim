using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Arsivim.ViewModels
{
    /// <summary>
    /// Tüm ViewModel'ler için temel sınıf
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy = false;
        private string _title = string.Empty;

        /// <summary>
        /// Yükleme durumu
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set 
            { 
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(IsNotBusy));
                    OnPropertyChanged(nameof(Yuklenme));
                }
            }
        }

        /// <summary>
        /// Yükleme durumu değil
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Yükleme durumu (Türkçe property adı)
        /// </summary>
        public bool Yuklenme => IsBusy;

        /// <summary>
        /// Sayfa başlığı
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Property değişiklik bildirimi
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Property değeri ayarlar ve değişiklik bildirimini gönderir
        /// </summary>
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Property değişiklik bildirimi gönderir
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Birden fazla property için değişiklik bildirimi
        /// </summary>
        protected void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Async işlemler için yardımcı metod
        /// </summary>
        protected async Task ExecuteAsync(Func<Task> operation)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await operation();
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Async işlemler için yardımcı metod (return değeri ile)
        /// </summary>
        protected async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            if (IsBusy)
                return default(T)!;

            try
            {
                IsBusy = true;
                return await operation();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
} 