using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PollAventuras.Models;
using PollAventuras.Services;

namespace PollAventuras.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IFirebaseService _firebaseService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private User? currentUser;

        [ObservableProperty]
        private List<GameHint> gameHints = new();

        [ObservableProperty]
        private List<SupportTicket> supportTickets = new();

        public MainViewModel(IFirebaseService firebaseService, AuthService authService)
        {
            _firebaseService = firebaseService;
            _authService = authService;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                // Cargar datos del usuario y del juego
                var userId = Preferences.Get("UserId", string.Empty);
                if (!string.IsNullOrEmpty(userId))
                {
                    CurrentUser = await _firebaseService.GetUserAsync(userId);
                    GameHints = await _firebaseService.GetHintsForGame("PollAventuras");
                    SupportTickets = await _firebaseService.GetUserTickets(userId);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToHints()
        {
            await Shell.Current.GoToAsync("//hints");
        }

        [RelayCommand]
        private async Task NavigateToSupport()
        {
            await Shell.Current.GoToAsync("//support");
        }

        [RelayCommand]
        private async Task CreateNewTicket()
        {
            await Shell.Current.GoToAsync(nameof(Views.Support.NewTicketPage));
        }
    }
}