using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Windows.Input;
using PollAventuras10A.Models;
using PollAventuras10A.Services;
using PollAventuras10A.Views;

namespace PollAventuras10A.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly FirebaseService _firebaseService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferencesService _preferencesService;

        private ObservableCollection<Article> _featuredArticles;
        private ObservableCollection<Article> _recentArticles;
        private ObservableCollection<Category> _categories;
        private string _userName;

        public MainViewModel(
            FirebaseService firebaseService,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferencesService preferencesService)
        {
            _firebaseService = firebaseService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferencesService = preferencesService;

            FeaturedArticles = new ObservableCollection<Article>();
            RecentArticles = new ObservableCollection<Article>();
            Categories = new ObservableCollection<Category>();

            ArticleSelectedCommand = new Command<Article>(async (article) => await OnArticleSelectedAsync(article));
            CategorySelectedCommand = new Command<Category>(async (category) => await OnCategorySelectedAsync(category));
            LogoutCommand = new Command(async () => await OnLogoutAsync());
            RefreshCommand = new Command(async () => await OnRefreshAsync());

            Title = "Inicio";
            LoadUserData();
        }

        public ObservableCollection<Article> FeaturedArticles
        {
            get => _featuredArticles;
            set => SetProperty(ref _featuredArticles, value);
        }

        public ObservableCollection<Article> RecentArticles
        {
            get => _recentArticles;
            set => SetProperty(ref _recentArticles, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public ICommand ArticleSelectedCommand { get; }
        public ICommand CategorySelectedCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand RefreshCommand { get; }

        // Método que se ejecuta cuando aparece la vista
        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Cargar datos en paralelo para mejor rendimiento
                var featuredTask = _firebaseService.GetFeaturedArticlesAsync();
                var recentTask = _firebaseService.GetArticlesAsync();
                var categoriesTask = _firebaseService.GetCategoriesAsync();

                await Task.WhenAll(featuredTask, recentTask, categoriesTask);

                FeaturedArticles = await featuredTask;
                var allArticles = await recentTask;
                Categories = await categoriesTask;

                // Tomar solo los 10 artículos más recientes
                RecentArticles = new ObservableCollection<Article>(
                    allArticles.OrderByDescending(a => a.CreatedAt).Take(10));
            });
        }

        private async Task OnArticleSelectedAsync(Article article)
        {
            if (article == null) return;

            var parameters = new Dictionary<string, object>
            {
                { "article", article }
            };

            await _navigationService.NavigateToAsync(AppConstants.ArticleDetailRoute, parameters);
        }

        private async Task OnCategorySelectedAsync(Category category)
        {
            if (category == null) return;

            var parameters = new Dictionary<string, object>
            {
                { "category", category }
            };

            await _navigationService.NavigateToAsync(AppConstants.CategoryRoute, parameters);
        }

        private async Task OnLogoutAsync()
        {
            try
            {
                var confirm = await _dialogService.ShowConfirmAsync(
                    "Cerrar Sesión",
                    "¿Estás seguro de que quieres cerrar sesión?",
                    "Sí", "No");

                if (confirm)
                {
                    await _firebaseService.LogoutAsync();

                    // Limpiar preferencias del usuario
                    _preferencesService.Remove(AppConstants.UserEmailKey);
                    _preferencesService.Remove(AppConstants.UserNameKey);

                    // Navegar a la página de login
                    Application.Current.MainPage = new LoginPage(
                        Application.Current.Handler.MauiContext.Services.GetService<LoginViewModel>());
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Error",
                    $"Error al cerrar sesión: {ex.Message}");
            }
        }

        private async Task OnRefreshAsync()
        {
            IsRefreshing = true;

            try
            {
                await LoadDataAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private void LoadUserData()
        {
            // Cargar datos del usuario desde preferencias
            var userEmail = _preferencesService.Get(AppConstants.UserEmailKey);
            var userName = _preferencesService.Get(AppConstants.UserNameKey);

            if (!string.IsNullOrEmpty(userName))
            {
                UserName = userName;
            }
            else if (!string.IsNullOrEmpty(userEmail))
            {
                // Si no hay nombre guardado, usar la parte del email antes del @
                UserName = userEmail.Split('@')[0];
            }
            else
            {
                UserName = "Usuario";
            }
        }

        // Método para manejar la navegación cuando se aparece la vista
        public async Task OnAppearingAsync()
        {
            if (FeaturedArticles?.Count == 0 || RecentArticles?.Count == 0 || Categories?.Count == 0)
            {
                await InitializeAsync();
            }
        }
    }
}