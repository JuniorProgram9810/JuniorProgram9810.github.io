using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using PollAventuras10A.Models;
using PollAventuras10A.Services;

namespace PollAventuras10A.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly FirebaseService _firebaseService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferencesService _preferencesService;

        private User _currentUser;
        private string _userName;
        private string _userEmail;
        private string _newPassword;
        private string _confirmPassword;
        private bool _isEditingProfile;
        private bool _notificationsEnabled;
        private string _appVersion;

        public ProfileViewModel(
            FirebaseService firebaseService,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferencesService preferencesService)
        {
            _firebaseService = firebaseService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferencesService = preferencesService;

            EditProfileCommand = new Command(async () => await OnEditProfileAsync());
            SaveProfileCommand = new Command(async () => await OnSaveProfileAsync(), () => CanSaveProfile());
            CancelEditCommand = new Command(OnCancelEdit);
            ChangePasswordCommand = new Command(async () => await OnChangePasswordAsync());
            ToggleNotificationsCommand = new Command(OnToggleNotifications);
            LogoutCommand = new Command(async () => await OnLogoutAsync());
            DeleteAccountCommand = new Command(async () => await OnDeleteAccountAsync());

            Title = "Perfil";
            AppVersion = AppConstants.AppVersion;

            _ = LoadUserDataAsync();
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, value);
                ((Command)SaveProfileCommand).ChangeCanExecute();
            }
        }

        public string UserEmail
        {
            get => _userEmail;
            set => SetProperty(ref _userEmail, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsEditingProfile
        {
            get => _isEditingProfile;
            set => SetProperty(ref _isEditingProfile, value);
        }

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set => SetProperty(ref _notificationsEnabled, value);
        }

        public string AppVersion
        {
            get => _appVersion;
            set => SetProperty(ref _appVersion, value);
        }

        public ICommand EditProfileCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand ToggleNotificationsCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand DeleteAccountCommand { get; }

        private async Task LoadUserDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var firebaseUser = _firebaseService.GetCurrentUser();
                if (firebaseUser != null)
                {
                    UserEmail = firebaseUser.Email;
                    UserName = _preferencesService.Get(AppConstants.UserNameKey, firebaseUser.DisplayName ?? "Usuario");
                    NotificationsEnabled = _preferencesService.Get(AppConstants.NotificationsEnabledKey, true);
                }
            });
        }

        private async Task OnEditProfileAsync()
        {
            IsEditingProfile = true;
        }

        private async Task OnSaveProfileAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    await _dialogService.ShowAlertAsync("Error", "El nombre no puede estar vacío.");
                    return;
                }

                // Guardar el nombre en las preferencias
                _preferencesService.Set(AppConstants.UserNameKey, UserName);

                // Aquí podrías actualizar el nombre en Firebase si tienes una colección de usuarios
                // await _firebaseService.UpdateUserProfileAsync(UserName);

                await _dialogService.ShowAlertAsync("Éxito", "Perfil actualizado correctamente.");
                IsEditingProfile = false;
            });
        }

        private void OnCancelEdit()
        {
            IsEditingProfile = false;
            // Restaurar valores originales
            UserName = _preferencesService.Get(AppConstants.UserNameKey, "Usuario");
        }

        private async Task OnChangePasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await _dialogService.ShowAlertAsync("Error", "Todos los campos son obligatorios.");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await _dialogService.ShowAlertAsync("Error", "Las contraseñas no coinciden.");
                return;
            }

            if (NewPassword.Length < 6)
            {
                await _dialogService.ShowAlertAsync("Error", "La contraseña debe tener al menos 6 caracteres.");
                return;
            }

            await ExecuteAsync(async () =>
            {
                try
                {
                    var user = _firebaseService.GetCurrentUser();
                    if (user != null)
                    {
                        // Aquí necesitarías implementar el cambio de contraseña en Firebase
                        // await user.UpdatePasswordAsync(NewPassword);

                        await _dialogService.ShowAlertAsync("Éxito", "Contraseña actualizada correctamente.");

                        // Limpiar campos
                        NewPassword = string.Empty;
                        ConfirmPassword = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowAlertAsync("Error", $"Error al cambiar la contraseña: {ex.Message}");
                }
            });
        }

        private void OnToggleNotifications()
        {
            _preferencesService.Set(AppConstants.NotificationsEnabledKey, NotificationsEnabled);
        }

        private async Task OnLogoutAsync()
        {
            var confirm = await _dialogService.ShowConfirmAsync(
                "Cerrar Sesión",
                "¿Estás seguro de que quieres cerrar sesión?",
                "Sí", "No");

            if (confirm)
            {
                await ExecuteAsync(async () =>
                {
                    await _firebaseService.LogoutAsync();
                    Application.Current.MainPage = new Views.LoginPage(
                        Application.Current.Handler.MauiContext.Services.GetService<LoginViewModel>());
                });
            }
        }

        private async Task OnDeleteAccountAsync()
        {
            var confirm = await _dialogService.ShowConfirmAsync(
                "Eliminar Cuenta",
                "¿Estás seguro de que quieres eliminar tu cuenta? Esta acción no se puede deshacer.",
                "Eliminar", "Cancelar");

            if (confirm)
            {
                var finalConfirm = await _dialogService.ShowConfirmAsync(
                    "Confirmación Final",
                    "Esta acción eliminará permanentemente tu cuenta y todos tus datos. ¿Continuar?",
                    "Sí, eliminar", "No");

                if (finalConfirm)
                {
                    await ExecuteAsync(async () =>
                    {
                        try
                        {
                            var user = _firebaseService.GetCurrentUser();
                            if (user != null)
                            {
                                // Eliminar datos del usuario de Firestore
                                // await _firebaseService.DeleteUserDataAsync(user.Uid);

                                // Eliminar cuenta de autenticación
                                // await user.DeleteAsync();

                                // Limpiar preferencias
                                _preferencesService.Clear();

                                await _dialogService.ShowAlertAsync("Cuenta Eliminada",
                                    "Tu cuenta ha sido eliminada exitosamente.");

                                Application.Current.MainPage = new Views.LoginPage(
                                    Application.Current.Handler.MauiContext.Services.GetService<LoginViewModel>());
                            }
                        }
                        catch (Exception ex)
                        {
                            await _dialogService.ShowAlertAsync("Error",
                                $"Error al eliminar la cuenta: {ex.Message}");
                        }
                    });
                }
            }
        }

        private bool CanSaveProfile()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !IsLoading;
        }
    }
}