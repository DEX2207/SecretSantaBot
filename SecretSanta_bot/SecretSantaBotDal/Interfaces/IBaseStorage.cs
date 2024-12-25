namespace SecretSatnaBotDal.Interfaces;

public interface IBaseStorage<T> where T : class
{
    Task Add(T entity);
    Task Delete(T entity);
    Task<T> Get(long id);
    Task<T> Update(T entity);
    
    IQueryable<T> GetAll();
}