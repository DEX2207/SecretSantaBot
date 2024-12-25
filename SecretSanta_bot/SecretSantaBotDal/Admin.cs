using System;
using System.Collections.Generic;
using SecretSatnaBotDal.Enum;

namespace SecretSatnaBotDal;

public partial class Admin
{
    public long AdminId { get; set; }

    public GameStatus Gamestatus { get; set; }

    public double? Maxprice { get; set; }

    public long ChatId { get; set; }

    public int Id { get; set; }
    
    public Admin()
    {
        
    }
    public Admin(long chatId, long adminId)
    {
        ChatId = chatId;
        AdminId = adminId;
        Gamestatus = 0;
        Random random = new Random();
        Id = random.Next(int.MinValue, int.MaxValue);
    }
}
