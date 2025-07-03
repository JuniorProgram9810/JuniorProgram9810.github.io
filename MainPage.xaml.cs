using PollAventuras.ViewModels;

namespace PollAventuras.Views
{
    public partial class MainPage : TabbedPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}