using InternetBanking.Database.Entities;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InternetBanking.Database
{
    public class AppDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public AppDatabase(string path)
        {
            _database = new SQLiteAsyncConnection(path);

            _database.CreateTableAsync<MessageThread>().Wait();
        }

        public Task<List<MessageThread>> GetMessageThreadsAsync()
        {
            return _database
                .Table<MessageThread>()
                .ToListAsync();
        }

        public Task<MessageThread> GetMessageThreadAsync(long id)
        {
            return _database
                .Table<MessageThread>()
                .FirstOrDefaultAsync(x => x.Key == id);
        }

        public Task<int> SaveMessageThreadAsync(MessageThread entity)
        {
            return entity.Key == 0 ? _database.InsertAsync(entity) : _database.UpdateAsync(entity);
        }

        public Task<int> DeleteMessageThreadAsync(MessageThread entity)
        {
            return _database.DeleteAsync(entity);
        }
    }
}
