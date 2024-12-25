using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace SecretSanta_bot;

public class Host
{
    public Action<ITelegramBotClient, Update>? OnMessage;
    private TelegramBotClient client;

    public Host(string token)
    {
        client = new TelegramBotClient(token);
    }

    public void Start()
    {
        client.StartReceiving(UpdateHandler,ErrorHandler);
        Console.WriteLine("Бот запущен");
    }

    private async Task ErrorHandler(ITelegramBotClient client, Exception ex, HandleErrorSource arg3, CancellationToken arg4)
    {
        Console.WriteLine("Ошибка "+ ex.Message);
        await Task.CompletedTask;
    }

    private async Task UpdateHandler(ITelegramBotClient arg1, Update update, CancellationToken arg3)
    {
        Console.WriteLine($"Пришло сообщение: {update.Message?.Text ?? "Не текст"}");
        OnMessage.Invoke(client, update);
        await Task.CompletedTask;
    }
}