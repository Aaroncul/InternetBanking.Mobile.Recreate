using SQLite;
using System;

namespace InternetBanking.Database.Entities
{
    public abstract class BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public long Key { get; set; }

        [Indexed]
        public DateTime Created { get; set; }
    }
}
