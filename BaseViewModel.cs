using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PollAventuras10A.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isLoading;
        private bool _isRefreshing;
        private string _title;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual async Task ExecuteAsync(Func<Task> task, bool showLoading = true)
        {
            try
            {
                if (showLoading)
                    IsLoading = true;

                await task();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
            }
            finally
            {
                if (showLoading)
                    IsLoading = false;
            }
        }

        protected virtual async Task<T> ExecuteAsync<T>(Func<Task<T>> task, bool showLoading = true)
        {
            try
            {
                if (showLoading)
                    IsLoading = true;

                return await task();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                return default(T);
            }
            finally
            {
                if (showLoading)
                    IsLoading = false;
            }
        }

        protected virtual async Task HandleExceptionAsync(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");

            // Mostrar mensaje de error al usuario
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Ha ocurrido un error inesperado. Por favor, intenta de nuevo.",
                    "OK");
            }
        }
    }
}