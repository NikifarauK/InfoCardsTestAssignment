using System.Collections.Generic;

namespace Server.Data
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        bool Insert(T card);
        bool Update(T card);
        bool Delete(int id);
        void SaveChanges();
    }
}
