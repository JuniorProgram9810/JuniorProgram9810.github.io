using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Windows.Input;
using PollAventuras10A.Services;

namespace PollAventuras10A.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly FirebaseService _firebaseService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferencesService _preferencesService;

        private string _name;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _errorMessage;
        private bool _hasError;

        public RegisterViewModel(
            FirebaseService firebaseService,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferencesService preferencesService)
        {
            _firebaseService = firebaseService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferencesService = preferencesService;

            RegisterCommand = new Command(async () => await OnRegisterAsync(), () => CanRegister());
            GoToLoginCommand = new Command(async () => await OnGoToLoginAsync());

            Title = "Registro";
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                ClearError();
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ClearError();
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ClearError();
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                ClearError();
                ((Command)RegisterCommand).ChangeCanExecute();
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

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        private async Task OnRegisterAsync()
        {
            if (!ValidateForm())
                return;

            await ExecuteAsync(async () =>
            {
                var success = await _firebaseService.RegisterAsync(Email, Password, Name);

                if (success)
                {
                    // Guardar datos del usuario en preferencias
                    _preferencesService.Set(AppConstants.UserEmailKey, Email);
                    _preferencesService.Set(AppConstants.UserNameKey, Name);

                    await _dialogService.ShowAlertAsync("¡Éxito!",
                        "Tu cuenta ha sido creada exitosamente. Ahora puedes iniciar sesión.");

                    // Navegar a login
                    await _navigationService.NavigateToAsync(AppConstants.LoginRoute);
                }
                else
                {
                    ShowError("Error al crear la cuenta. Por favor, intenta nuevamente.");
                }
            });
        }

        private async Task OnGoToLoginAsync()
        {
            await _navigationService.NavigateToAsync(AppConstants.LoginRoute);
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   !IsLoading;
        }

        private bool ValidateForm()
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(Name) || Name.Length < 2)
            {
                ShowError("El nombre debe tener al menos 2 caracteres.");
                return false;
            }

            // Validar email
            if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
            {
                ShowError("Por favor, ingresa un email válido.");
                return false;
            }

            // Validar contraseña
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                ShowError("La contraseña debe tener al menos 6 caracteres.");
                return false;
            }

            // Validar confirmación de contraseña
            if (Password != ConfirmPassword)
            {
                ShowError("Las contraseñas no coinciden.");
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
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