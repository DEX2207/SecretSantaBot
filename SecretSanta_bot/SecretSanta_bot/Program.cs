using SecretSatnaBotDal;
using SecretSatnaBotDal.Enum;
using SecretSatnaBotDal.Interfaces;
using SecretSatnaBotDal.Storage;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSanta_bot;

internal class Program
{
    private static IBaseStorage<Admin> _adminStorage;
    private static IBaseStorage<Gamer> _gamerStorage;
    public static void Main()
    {
        Host client = new Host("7964980181:AAEdYfSihOmuMAVX56LYuUIcb83C8Lhl0ds");
        client.Start();
        client.OnMessage += OnMessage;
        Console.ReadLine();
    }

    private static async void OnMessage(ITelegramBotClient client, Update update)
    {
        try
        {
                SecretSantaDbContext dbContext = new SecretSantaDbContext();
                _adminStorage = new AdminStorage(dbContext);
                _gamerStorage = new GamerStorage(dbContext);
                
                if (update.Message?.NewChatMembers != null && (bool)update.Message?.NewChatMembers.Any())
                {
                    foreach (var member in update.Message?.NewChatMembers)
                    {
                        if (member.Id == client.BotId)
                        {
                            var addedByUser = update.Message.From; 
                            if (addedByUser != null)
                            {
                                Admin newAdmin = new Admin(update.Message.Chat.Id, addedByUser.Id);
                                await _adminStorage.Add(newAdmin);

                                await client.SendTextMessageAsync(update.Message.Chat.Id,
                                    $"Привет, я бот Тайного Санты! Меня добавил {addedByUser.FirstName}.");
                                break;
                            }
                        }
                        else
                        { 
                            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564,
                                $"Добро пожаловать, {member.FirstName}!");
                            break;
                        }
                    }
                }
                else if (update.Message?.LeftChatMember != null)
                {
                    if (update.Message?.LeftChatMember.Id == client.BotId)
                    { 
                        var oldAdmin = await _adminStorage.Get(update.Message.Chat.Id);
                        _adminStorage.Delete(oldAdmin);
                    }
                    await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, 
                        $"Прощай, {update.Message?.LeftChatMember.FirstName}!");
                }
                
                var admin = await _adminStorage.Get(update.Message?.Chat.Id ?? 5214455564);
                
                switch (update.Message?.Text.Split(' ')[0])
                {
                    case "/start":
                        if (update.Message.From.Id == admin.AdminId)
                        {
                            if (update.Message.Text.Split(' ').Length > 1 && double.TryParse(update.Message.Text.Split(' ')[1], out double maxPrice))
                            {
                                admin.Maxprice = maxPrice;
                                admin.Gamestatus = GameStatus.GameStarted;
                                await _adminStorage.Update(admin);
                                await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, $"Игра началась! Успейте зарегистрироваться!\n Максимальная цена подарка: {maxPrice}");
                            }
                            else
                            {
                                await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, "Для начала игры введите максимальную сумму подарка (пример: /start 1000)");
                            }
                        }
                        else
                        {
                            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, "Нет прав");
                        }
                        break;
                    case "/stop":
                        if (update.Message.From.Id == admin.AdminId)
                        {
                            var gamers = _gamerStorage.GetAll().Where(x=>x.ChatId == admin.ChatId).ToList();
                            admin.Maxprice = null;
                            admin.Gamestatus = GameStatus.GameNotStarted;
                            await _adminStorage.Update(admin);
                            foreach (var gamer in gamers)
                            {
                                if (gamer.GiftTo != null)
                                {
                                    for (int i = 0; i < gamers.Count; i++)
                                    {
                                        if (gamers[i].Username == gamer.GiftTo)
                                        {
                                            await client.SendTextMessageAsync(gamers[i].UserId, $"Игрок @{gamer.Username} подарил вам {gamer.Gift} игровой валюты");
                                        }
                                    }
                                }
                                await _gamerStorage.Delete(gamer);
                            }
                            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, "Игра остановлена! Спасибо всем, кто участвовал!");
                        }
                        else
                        {
                            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, "Нет прав");
                        }
                        break;
                    case "/reset":
                        if (update.Message.From.Id == admin.AdminId)
                        {
                            var gamers = _gamerStorage.GetAll().Where(x=>x.ChatId == admin.ChatId).ToList();
                            foreach (var gamer in gamers)
                            {
                                for (int i = 0; i < gamers.Count; i++)
                                {
                                    if (gamers[i].Username == gamer.GiftTo)
                                    {
                                        await client.SendTextMessageAsync(gamers[i].UserId, $"Игрок @{gamer.Username} подарил вам {gamer.Gift} игровой валюты");
                                    }
                                }
                                await _gamerStorage.Delete(gamer);
                            }
                            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, $"Игра сброшена! Максимальная цена подарка также {admin.Maxprice}.\nУспейте заригестрироваться снова!");
                        }
                        else
                        {
                            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, "Нет прав");
                        }
                        break;
                    case "/join":
                        if (admin.Gamestatus == GameStatus.GameStarted)
                        {
                            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, $"Спасибо, что участвуете в нашей игре,{update.Message.From.FirstName}!");
                            var newgamer = new Gamer(update.Message.Chat.Id,update.Message.From.Id,update.Message.From.Username);
                            await _gamerStorage.Add(newgamer);
                            var gamers = _gamerStorage.GetAll().Where(x=>x.ChatId == admin.ChatId).ToList();
                            if (gamers.Count>=2)
                            {
                                Random random = new Random();
                                int num=random.Next(0,gamers.Count);
                                if (gamers[num] == null || gamers[num].Username == null)
                                {
                                    await client.SendTextMessageAsync(update.Message.From.Id, "Ошибка: У случайного участника нет имени пользователя. Попробуйте позже.");
                                    break;
                                }
checkgoto:
                                if (gamers[num].Username == newgamer.Username || gamers[num].Username == null)
                                {
                                    random = new Random();
                                    num=random.Next(0,gamers.Count);
                                    goto checkgoto;
                                }

                                for (int i = 0; i < gamers.Count; i++)
                                {
                                    if (gamers[num].Username == gamers[i].GiftTo || gamers[num].Username == null )
                                    {
                                        num=random.Next(0,gamers.Count);
                                        goto checkgoto;
                                    }
                                }
                                await client.SendTextMessageAsync(update.Message.From.Id, $"Вам предстоит подарить подарок игроку @{gamers[num].Username}\nВведите сумму подарка:");
                                newgamer.GiftTo = gamers[num].Username;
                                await _gamerStorage.Update(newgamer);
                            }
                            else
                            {
                                await client.SendTextMessageAsync(update.Message.From.Id, $"Упс, похоже пока никто кроме вас не зарегистрирован в игре! Давайте подождем новых участников");
                            }
                        }
                        else
                        {
                            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564, "Игра не начата");
                        }
                        break;
                    default:
                        if (update.Message?.Type == MessageType.Text)
                        {
                            if (update.Message.Chat.Id == update.Message.From.Id)
                            {
                                if (double.TryParse(update.Message.Text, out double gift))
                                {
                                    var newgamer= await _gamerStorage.Get(update.Message.From.Id);
                                    if (newgamer.Gift != null)
                                    {
                                        await client.SendTextMessageAsync(update.Message.From.Id,"Подарок уже был отправлен");
                                    }
                                    else if (newgamer != null)
                                    {
                                        admin= await _adminStorage.Get(newgamer.ChatId);
                                        if (gift > admin.Maxprice)
                                        { 
                                            await client.SendTextMessageAsync(update.Message.From.Id, "Сумма подарка больше той, что указана администратором");
                                        }
                                        if(gift <= admin.Maxprice && gift != 0 && gift >= 1)
                                        { 
                                            newgamer.Gift= gift; 
                                            await _gamerStorage.Update(newgamer); 
                                            await client.SendTextMessageAsync(update.Message?.From.Id ?? 5214455564, "Ваш подарок отправлен!");
                                        }
                                    }
                                    else
                                    {
                                        await client.SendTextMessageAsync(update.Message.From.Id, "Вы не зарегистрированы в игре!");
                                    }
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(update.Message.From.Id, "Неверный формат ввода, пожалуйста введите число");
                                }
                            }
                            else
                            {
                                await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 5214455564,
                                    "Неизвестная команда");
                            }
                        }
                        break;
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}