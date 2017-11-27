using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using System;
using System.Linq;

namespace DamaSalesApp.Models
{
    public class DamaDataContext
    {
        readonly SQLiteAsyncConnection database;

        public DamaDataContext(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);

            //database.DropTableAsync<Favorite>().Wait();
            //database.DropTableAsync<NewsCategory>().Wait();

            database.CreateTableAsync<Category>().Wait();
            //database.CreateTableAsync<ProductType>().Wait();
            //database.CreateTableAsync<Product>().Wait();
            //database.CreateTableAsync<BasketItem>().Wait();
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return database.Table<Category>().ToListAsync();
        }

        public Task<int> SaveCategoryAsync(Category category)
        {
            if (category.Id != 0)
            {
                return database.UpdateAsync(category);
            }
            else
            {
                return database.InsertAsync(category);
            }
        }


        //public Task<List<Favorite>> GetItemsAsync()
        //{
        //    return database.Table<Favorite>().ToListAsync();
        //}

        //public async Task<List<Favorite>> GetItemsAsync(List<NewsCategory> categories)
        //{
        //    var favorites = await database.Table<Favorite>().ToListAsync();

        //    foreach (var favorite in favorites)
        //    {
        //        favorite.Category = categories.Where(w => w.Id == favorite.CategoryId).FirstOrDefault();
        //    }

        //    return favorites;
        //}

        //public Task<List<Favorite>> GetItemsByCategoryAsync(List<NewsCategory> categories)
        //{
        //    if (categories != null && categories.Count > 0)
        //    {
        //        return database.QueryAsync<Favorite>($"SELECT * FROM [Favorite] WHERE [CategoryId] = {categories.FirstOrDefault().Id}");
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}

        //public Task<Favorite> GetItemAsync(int id)
        //{
        //    return database.Table<Favorite>().Where(i => i.Id == id).FirstOrDefaultAsync();
        //}

        //public Task<NewsCategory> GetCategoryAsync(string title)
        //{
        //    return database.Table<NewsCategory>().Where(i => i.Title.Equals(title)).FirstOrDefaultAsync();
        //}

        //public Task<int> SaveCategoriesAsync(List<NewsCategory> categories)
        //{
        //    return database.InsertAllAsync(categories);
        //}

        //public Task<int> SaveCategoryAsync(NewsCategory category)
        //{
        //    if (category.Id != 0)
        //    {
        //        return database.UpdateAsync(category);
        //    }
        //    else
        //    {
        //        return database.InsertAsync(category);
        //    }
        //}

        //public Task<int> SaveItemAsync(Favorite item)
        //{
        //    if (item.Id != 0)
        //    {
        //        return database.UpdateAsync(item);
        //    }
        //    else
        //    {
        //        return database.InsertAsync(item);
        //    }
        //}

        //public Task<int> DeleteItemAsync(Favorite item)
        //{
        //    return database.DeleteAsync(item);
        //}
    }
}
 