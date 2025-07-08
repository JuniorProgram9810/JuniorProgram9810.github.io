using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using PollAventuras10A.Services;

namespace PollAventuras10A.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly FirebaseService _firebaseService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferencesService _preferencesService;

        private string _email;
        private string _password;
        private string _errorMessage;
        private bool _hasError;

        public LoginViewModel(
            FirebaseService firebaseService,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferencesService preferencesService)
        {
            _firebaseService = firebaseService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferencesService = preferencesService;

            LoginCommand = new Command(async () => await OnLoginAsync(), () => CanLogin());
            GoToRegisterCommand = new Command(async () => await OnGoToRegisterAsync());

            Title = "Iniciar Sesión";
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ClearError();
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ClearError();
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        private async Task OnLoginAsync()
        {
            await ExecuteAsync(async () =>
            {
                var success = await _firebaseService.LoginAsync(Email, Password);

                if (success)
                {
                    // Guardar datos del usuario en preferencias
                    _preferencesService.Set(AppConstants.UserEmailKey, Email);

                    // Navegar a la página principal
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    ShowError("Email o contraseña incorrectos. Por favor, verifica tus datos.");
                }
            });
        }

        private async Task OnGoToRegisterAsync()
        {
            await _navigationService.NavigateToAsync(AppConstants.RegisterRoute);
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !IsLoading;
        }

        private void ShowError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        private void ClearError()
        {
            if (HasError)
            {
                ErrorMessage = string.Empty;
                HasError = false;
            }
        }
    }
}