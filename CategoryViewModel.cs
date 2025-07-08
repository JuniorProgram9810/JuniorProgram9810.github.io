using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Windows.Input;
using PollAventuras10A.Models;
using PollAventuras10A.Services;

namespace PollAventuras10A.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        private readonly FirebaseService _firebaseService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<Category> _categories;
        private ObservableCollection<Article> _categoryArticles;
        private Category _selectedCategory;
        private bool _isArticlesVisible;

        public CategoryViewModel(
            FirebaseService firebaseService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _firebaseService = firebaseService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            Categories = new ObservableCollection<Category>();
            CategoryArticles = new ObservableCollection<Article>();

            CategorySelectedCommand = new Command<Category>(async (category) => await OnCategorySelectedAsync(category));
            ArticleSelectedCommand = new Command<Article>(async (article) => await OnArticleSelectedAsync(article));
            RefreshCommand = new Command(async () => await OnRefreshAsync());
            BackCommand = new Command(async () => await OnBackAsync());

            Title = "Categorías";

            _ = LoadCategoriesAsync();
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<Article> CategoryArticles
        {
            get => _categoryArticles;
            set => SetProperty(ref _categoryArticles, value);
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                Title = value?.Name ?? "Categorías";
            }
        }

        public bool IsArticlesVisible
        {
            get => _isArticlesVisible;
            set => SetProperty(ref _isArticlesVisible, value);
        }

        public ICommand CategorySelectedCommand { get; }
        public ICommand ArticleSelectedCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand BackCommand { get; }

        private async Task LoadCategoriesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var categories = await _firebaseService.GetCategoriesAsync();
                Categories.Clear();

                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            });
        }

        private async Task OnCategorySelectedAsync(Category category)
        {
            if (category == null) return;

            await ExecuteAsync(async () =>
            {
                SelectedCategory = category;
                IsArticlesVisible = true;

                var articles = await _firebaseService.GetArticlesByCategoryAsync(category.Id);
                CategoryArticles.Clear();

                foreach (var article in articles)
                {
                    CategoryArticles.Add(article);
                }

                if (CategoryArticles.Count == 0)
                {
                    await _dialogService.ShowAlertAsync("Sin artículos",
                        "No hay artículos disponibles en esta categoría en este momento.");
                }
            });
        }

        private async Task OnArticleSelectedAsync(Article article)
        {
            if (article == null) return;

            var parameters = new Dictionary<string, object>
            {
                { "Article", article }
            };

            await _navigationService.NavigateToAsync(AppConstants.ArticleDetailRoute, parameters);
        }

        private async Task OnRefreshAsync()
        {
            IsRefreshing = true;

            if (SelectedCategory != null)
            {
                await OnCategorySelectedAsync(SelectedCategory);
            }
            else
            {
                await LoadCategoriesAsync();
            }

            IsRefreshing = false;
        }

        private async Task OnBackAsync()
        {
            if (IsArticlesVisible)
            {
                IsArticlesVisible = false;
                SelectedCategory = null;
                Title = "Categorías";
            }
            else
            {
                await _navigationService.GoBackAsync();
            }
        }
    }
}