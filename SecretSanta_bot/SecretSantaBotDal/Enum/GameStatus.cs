using System.ComponentModel;

namespace SecretSatnaBotDal.Enum;

public enum GameStatus
{
    [Description("Игра не запущена")]
    GameNotStarted = 0,
    [Description("Игра идет")]
    GameStarted = 1
}