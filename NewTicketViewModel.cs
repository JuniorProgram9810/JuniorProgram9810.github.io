using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PollAventuras.Models;
using PollAventuras.Services;

namespace PollAventuras.ViewModels.Support
{
    public partial class NewTicketViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        private readonly IFirebaseService _firebaseService;

        public NewTicketViewModel(IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [RelayCommand]
        private async Task SubmitTicket()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Description))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor completa todos los campos", "OK");
                return;
            }

            try
            {
                var userId = Preferences.Get("UserId", string.Empty);
                if (string.IsNullOrEmpty(userId))
                {
                    await Shell.Current.DisplayAlert("Error", "Usuario no autenticado", "OK");
                    return;
                }

                var ticket = new SupportTicket
                {
                    UserId = userId,
                    Title = Title,
                    Description = Description
                };

                await _firebaseService.AddSupportTicket(ticket);
                await Shell.Current.DisplayAlert("Éxito", "Ticket creado correctamente", "OK");

                // Limpiar campos
                Title = string.Empty;
                Description = string.Empty;

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al crear ticket: {ex.Message}", "OK");
            }
        }
    }
}