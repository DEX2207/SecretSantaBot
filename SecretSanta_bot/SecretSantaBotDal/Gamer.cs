using System;
using System.Collections.Generic;

namespace SecretSatnaBotDal;

public partial class Gamer
{
    public long ChatId { get; set; }

    public long UserId { get; set; }

    public double? Gift { get; set; }

    public string Username { get; set; } = null!;

    public string? GiftTo { get; set; }

    public int Id { get; set; }
    
    public Gamer()
    {
    }

    public Gamer(long chatId, long userId, string username)
    {
        ChatId = chatId;
        UserId = userId;
        Username = username;
        Random random = new Random();
        Id = random.Next(int.MinValue, int.MaxValue);
    }
    public Gamer(long chatId, long userId, double gift, string username, string giftTo)
    {
        ChatId = chatId;
        UserId = userId;
        Gift = gift;
        Username = username;
        GiftTo = giftTo;
    }
}
