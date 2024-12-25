using Microsoft.EntityFrameworkCore;
using SecretSatnaBotDal.Interfaces;

namespace SecretSatnaBotDal.Storage;

public class GamerStorage(SecretSantaDbContext db):IBaseStorage<Gamer>
{
    private static SecretSantaDbContext dbContext = new SecretSantaDbContext();
    private static IBaseStorage<Gamer> _gamerStorage=new GamerStorage(dbContext);
    
    public async Task Add(Gamer item)
    {
        var existingGamer = db.Gamers.FirstOrDefault(x => x.UserId == item.UserId || x.Username == item.Username || x.GiftTo == item.GiftTo);
        if (existingGamer == null)
        {
            await db.Gamers.AddAsync(item);
            Console.WriteLine($"Добавляем игрока: UserId={item.UserId}, Username={item.Username}, GiftTo={item.GiftTo}");
            await db.SaveChangesAsync();
        }
        else
        {
            if (item.GiftTo == null && item.Username != existingGamer.GiftTo)
            {
                existingGamer.GiftTo = item.Username;
                await _gamerStorage.Update(existingGamer);
                await db.SaveChangesAsync();
                await db.Gamers.AddAsync(item);
                Console.WriteLine($"Добавляем игрока: UserId={item.UserId}, Username={item.Username}, GiftTo={item.GiftTo}");
                await db.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Игрок с таким UserId уже существует.");
            }
        }
    }

    public async Task Delete(Gamer item)
    {
        db.Gamers.Remove(item); 
        await db.SaveChangesAsync();
    }
    
    public async Task<Gamer> Get(long id)
    {
        return await db.Gamers.FirstOrDefaultAsync(x=>x.UserId == id);
    }

    public IQueryable<Gamer> GetAll()
    {
        return db.Gamers.AsQueryable();
    }
    public async Task<Gamer> Update(Gamer item)
    {
        db.Gamers.Update(item);
        await db.SaveChangesAsync();
        return item;
    }
}