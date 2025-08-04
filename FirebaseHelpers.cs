using AppIntegradora10A.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntegradora10A.Helpers
{
    public class FirebaseHelpers
    {
        private readonly FirebaseClient firebaseClient;

        public FirebaseHelpers()
        {
            firebaseClient = new FirebaseClient("https://integradira10a-default-rtdb.firebaseio.com/");
        }

        public async Task<List<Producto>> GetAllproducts()
        {
            var productos = await firebaseClient
                .Child("Productos")
                .OnceAsync<Producto>();

            return productos.Select(item => new Producto
            {
                Id = item.Key,
                Nombre = item.Object.Nombre,
                Descripcion = item.Object.Descripcion,
                Precio = item.Object.Precio
            }).ToList();
        }

        public async Task AddProducto(Producto producto)
        {
            await firebaseClient
                .Child("Productos")
                .PostAsync(producto);
        }

        public async Task UpdateProducto(string key, Producto producto)
        {
            // Corregido: agregar .Child(key) para actualizar el producto específico
            await firebaseClient
                .Child("Productos")
                .Child(key)
                .PutAsync(producto);
        }

        public async Task DeleteProducto(string key)
        {
            await firebaseClient
                .Child("Productos")
                .Child(key)
                .DeleteAsync();
        }
    }
}