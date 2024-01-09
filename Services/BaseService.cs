using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Infrastructure.Repositories;

namespace AttendanceControlBot.Services;

public class BaseService<T> where T : BaseEntity
{
    private RepositoryBase<T> _repository;

    public BaseService(RepositoryBase<T> repository)
    {
        _repository = repository;
    }

    public async Task<T> CreateAsync(T t)
    {
       return await _repository.AddAsync(t);
    }

    public async Task<T> UpdateAsync(T t)
    {
        return await _repository.UpdateAsync(t);
    }

    public async Task<T?> GetByIdAsync(long id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return  _repository.GetAll().ToList();
    }

    public async Task<T> DeleteAsync(long id)
    {
        return await _repository.RemoveAsync(id);
    }
}