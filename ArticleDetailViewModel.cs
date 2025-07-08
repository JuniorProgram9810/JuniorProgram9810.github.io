using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using PollAventuras10A.Models;
using PollAventuras10A.Services;

namespace PollAventuras10A.ViewModels
{
    [QueryProperty(nameof(Article), "article")]
    public class ArticleDetailViewModel : BaseViewModel
    {
        private readonly FirebaseService _firebaseService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private Article _article;
        private bool _isFavorite;
        private bool _isBookmarked;

        public ArticleDetailViewModel(
            FirebaseService firebaseService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _firebaseService = firebaseService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            ShareCommand = new Command(async () => await OnShareAsync());
            ToggleFavoriteCommand = new Command(async () => await OnToggleFavoriteAsync());
            ToggleBookmarkCommand = new Command(async () => await OnToggleBookmarkAsync());
            GoBackCommand = new Command(async () => await OnGoBackAsync());

            Title = "Artículo";
        }

        public Article Article
        {
            get => _article;
            set
            {
                SetProperty(ref _article, value);
                if (_article != null)
                {
                    Title = _article.Title;
                    LoadArticleState();
                }
            }
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        public bool IsBookmarked
        {
            get => _isBookmarked;
            set => SetProperty(ref _isBookmarked, value);
        }

        public ICommand ShareCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand ToggleBookmarkCommand { get; }
        public ICommand GoBackCommand { get; }

        public async Task InitializeAsync(Article article)
        {
            Article = article;
            await LoadArticleState();
        }

        private async Task LoadArticleState()
        {
            if (Article == null) return;

            await ExecuteAsync(async () =>
            {
                // Aquí podrías cargar el estado de favoritos y marcadores desde Firebase
                // Por ejemplo, verificar si el artículo está en los favoritos del usuario
                // IsFavorite = await _firebaseService.IsArticleFavoriteAsync(Article.Id);
                // IsBookmarked = await _firebaseService.IsArticleBookmarkedAsync(Article.Id);

                // Por ahora, valores por defecto
                IsFavorite = false;
                IsBookmarked = false;
            }, false);
        }

        private async Task OnShareAsync()
        {
            if (Article == null) return;

            try
            {
                var shareText = $"¡Mira este artículo interesante!\n\n{Article.Title}\n\n{Article.Summary}";

                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = "Compartir artículo"
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Error",
                    "No se pudo compartir el artículo en este momento.");
                System.Diagnostics.Debug.WriteLine($"Share error: {ex.Message}");
            }
        }

        private async Task OnToggleFavoriteAsync()
        {
            if (Article == null) return;

            await ExecuteAsync(async () =>
            {
                // Aquí implementarías la lógica para agregar/quitar de favoritos
                // await _firebaseService.ToggleArticleFavoriteAsync(Article.Id);

                IsFavorite = !IsFavorite;

                var message = IsFavorite ?
                    "Artículo agregado a favoritos" :
                    "Artículo removido de favoritos";

                await _dialogService.ShowAlertAsync("", message);
            }, false);
        }

        private async Task OnToggleBookmarkAsync()
        {
            if (Article == null) return;

            await ExecuteAsync(async () =>
            {
                // Aquí implementarías la lógica para agregar/quitar marcadores
                // await _firebaseService.ToggleArticleBookmarkAsync(Article.Id);

                IsBookmarked = !IsBookmarked;

                var message = IsBookmarked ?
                    "Artículo guardado para leer más tarde" :
                    "Artículo removido de guardados";

                await _dialogService.ShowAlertAsync("", message);
            }, false);
        }

        private async Task OnGoBackAsync()
        {
            await _navigationService.GoBackAsync();
        }

        // Método para formatear la fecha de publicación
        public string GetFormattedDate()
        {
            if (Article?.CreatedAt == null) return string.Empty;

            var timeSpan = DateTime.Now - Article.CreatedAt;

            if (timeSpan.TotalDays >= 1)
            {
                return $"Hace {(int)timeSpan.TotalDays} día(s)";
            }
            else if (timeSpan.TotalHours >= 1)
            {
                return $"Hace {(int)timeSpan.TotalHours} hora(s)";
            }
            else if (timeSpan.TotalMinutes >= 1)
            {
                return $"Hace {(int)timeSpan.TotalMinutes} minuto(s)";
            }
            else
            {
                return "Hace unos momentos";
            }
        }

        // Método para obtener el tiempo estimado de lectura
        public string GetEstimatedReadingTime()
        {
            if (string.IsNullOrEmpty(Article?.Content)) return "1 min";

            var wordCount = Article.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            var readingTime = Math.Ceiling(wordCount / 200.0); // Asumiendo 200 palabras por minuto

            return $"{readingTime} min";
        }
    }
}