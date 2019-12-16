using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICommonService<T>
    {
        Task<bool> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Remove(int Id);
        Task<T> GetById(int Id);
        Task<List<T>> GetAllById(int Id);
        Task<List<T>> GetAll();
        Task<PagedList<T>> GetAllPaging(string keyword,int page, int pageSize);

    }
}
