using SnakeHub.Models.Game;

namespace SnakeHub.Interfaces
{
    public interface IGameMode
    {
        GameObject[][] GameState { get; }
        List<Player> Players { get; }
        Action<Player, Enums.Action> OnPlayerAction { get; }
        void StartGame();
        int GetPlayerPoints(Player player);
    }
}
