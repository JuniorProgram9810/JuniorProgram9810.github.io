using PollAventuras10A.ViewModels;

namespace PollAventuras10A.Views;

public partial class ArticleDetailPage : ContentPage
{
    public ArticleDetailPage(ArticleDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}