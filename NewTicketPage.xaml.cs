// Views/Support/NewTicketPage.xaml.cs
using PollAventuras.ViewModels.Support;

namespace PollAventuras.Views.Support;

public partial class NewTicketPage : ContentPage
{
    public NewTicketPage(NewTicketViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}