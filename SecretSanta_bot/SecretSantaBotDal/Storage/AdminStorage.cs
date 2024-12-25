using Microsoft.EntityFrameworkCore;
using SecretSatnaBotDal.Interfaces;

namespace SecretSatnaBotDal.Storage;

public class AdminStorage(SecretSantaDbContext db):IBaseStorage<Admin>
{
    public async Task Add(Admin item)
    {
        await db.AddAsync(item);
        await db.SaveChangesAsync();
    }

    public async Task Delete(Admin item)
    {
        db.Remove(item);
        await db.SaveChangesAsync();
    }
    
    public async Task<Admin> Get(long id)
    {
        return await db.Admins.FirstOrDefaultAsync(x=>x.ChatId == id);
    }

    public IQueryable<Admin> GetAll()
    {
        return db.Admins.AsQueryable();
    }
    public async Task<Admin> Update(Admin item)
    {
        db.Admins.Update(item);
        await db.SaveChangesAsync();
        return item;
    }
}