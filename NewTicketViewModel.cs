using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PollAventuras.Models;
using PollAventuras.Services;
using System.Threading.Tasks;

namespace PollAventuras.ViewModels.Support;

public partial class NewTicketViewModel : ObservableObject
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string description;

    private readonly iFirebaseService _firebaseService;

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

        var userId = Preferences.Get("UserId", string.Empty);

        var ticket = new SupportTicket
        {
            UserId = userId,
            Title = Title,
            Description = Description
        };

        await _firebaseService.AddSupportTicket(ticket);
        await Shell.Current.DisplayAlert("Éxito", "Ticket creado correctamente", "OK");
        await Shell.Current.GoToAsync("..");
    }
}