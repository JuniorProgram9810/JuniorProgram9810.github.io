using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Services/FirebaseService.cs
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using PollAventuras10A.Models;
using System.Collections.ObjectModel;

namespace PollAventuras10A.Services
{
    public class FirebaseService
    {
        private readonly IFirestore _firestore;
        private readonly IFirebaseAuth _auth;

        public FirebaseService()
        {
            _firestore = CrossCloudFirestore.Current.Instance;
            _auth = CrossFirebaseAuth.Current.Instance;
        }

        // Autenticación
        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var authResult = await _auth.SignInWithEmailAndPasswordAsync(email, password);
                return authResult.User != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string email, string password, string name)
        {
            try
            {
                var authResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);

                if (authResult.User != null)
                {
                    // Crear perfil de usuario en Firestore
                    var user = new User
                    {
                        Id = authResult.User.Uid,
                        Email = email,
                        Name = name,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _firestore.Collection("users").Document(user.Id).SetAsync(user);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                await _auth.SignOutAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logout error: {ex.Message}");
                return false;
            }
        }

        // Obtener artículos
        public async Task<ObservableCollection<Article>> GetArticlesAsync()
        {
            try
            {
                var query = await _firestore.Collection("articles")
                    .OrderBy("created_at", true)
                    .GetAsync();

                var articles = new ObservableCollection<Article>();
                foreach (var doc in query.Documents)
                {
                    var article = doc.ToObject<Article>();
                    article.Id = doc.Id;
                    articles.Add(article);
                }
                return articles;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get articles error: {ex.Message}");
                return new ObservableCollection<Article>();
            }
        }

        // Obtener artículos destacados
        public async Task<ObservableCollection<Article>> GetFeaturedArticlesAsync()
        {
            try
            {
                var query = await _firestore.Collection("articles")
                    .WhereEqualsTo("is_featured", true)
                    .OrderBy("created_at", true)
                    .GetAsync();

                var articles = new ObservableCollection<Article>();
                foreach (var doc in query.Documents)
                {
                    var article = doc.ToObject<Article>();
                    article.Id = doc.Id;
                    articles.Add(article);
                }
                return articles;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get featured articles error: {ex.Message}");
                return new ObservableCollection<Article>();
            }
        }

        // Obtener categorías
        public async Task<ObservableCollection<Category>> GetCategoriesAsync()
        {
            try
            {
                var query = await _firestore.Collection("categories").GetAsync();
                var categories = new ObservableCollection<Category>();

                foreach (var doc in query.Documents)
                {
                    var category = doc.ToObject<Category>();
                    category.Id = doc.Id;
                    categories.Add(category);
                }
                return categories;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get categories error: {ex.Message}");
                return new ObservableCollection<Category>();
            }
        }

        // Obtener artículos por categoría
        public async Task<ObservableCollection<Article>> GetArticlesByCategoryAsync(string categoryId)
        {
            try
            {
                var query = await _firestore.Collection("articles")
                    .WhereEqualsTo("category", categoryId)
                    .OrderBy("created_at", true)
                    .GetAsync();

                var articles = new ObservableCollection<Article>();
                foreach (var doc in query.Documents)
                {
                    var article = doc.ToObject<Article>();
                    article.Id = doc.Id;
                    articles.Add(article);
                }
                return articles;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get articles by category error: {ex.Message}");
                return new ObservableCollection<Article>();
            }
        }

        // Verificar si el usuario está autenticado
        public bool IsUserAuthenticated()
        {
            return _auth.CurrentUser != null;
        }

        // Obtener usuario actual
        public IFirebaseUser GetCurrentUser()
        {
            return _auth.CurrentUser;
        }
    }
}