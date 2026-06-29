namespace Game.Leaderboard.Interfaces
{
    using Data;
    public interface IStickyLeaderboardView
    {
        void Show(LeaderboardEntry entry);
        void Hide();
    }
}