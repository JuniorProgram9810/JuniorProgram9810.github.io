// ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MainViewModel : ObservableObject
{
    private readonly FirebaseService _firebaseService;
    private readonly AuthService _authService;

    [ObservableProperty]
    private User currentUser;

    [ObservableProperty]
    private List<GameHint> gameHints;

    [ObservableProperty]
    private List<SupportTicket> supportTickets;

    public MainViewModel(FirebaseService firebaseService, AuthService authService)
    {
        _firebaseService = firebaseService;
        _authService = authService;

        LoadData();
    }

    private async void LoadData()
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

    [RelayCommand]
    private async Task NavigateToHints()
    {
        await Shell.Current.GoToAsync("//hints");
    }

    [RelayCommand]
    private async Task CreateNewTicket()
    {
        await Shell.Current.GoToAsync("newticket");
    }
}