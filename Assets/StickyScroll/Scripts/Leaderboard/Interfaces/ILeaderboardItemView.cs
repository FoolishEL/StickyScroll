
namespace Game.Leaderboard.Interfaces
{
    using Data;
    public interface ILeaderboardItemView
    {
        void Bind(LeaderboardEntry entry);
    }
}