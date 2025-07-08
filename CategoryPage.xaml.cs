
using PollAventuras10A.ViewModels;

namespace PollAventuras10A.Views;

public partial class CategoryPage : ContentPage
{
    public CategoryPage(CategoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}