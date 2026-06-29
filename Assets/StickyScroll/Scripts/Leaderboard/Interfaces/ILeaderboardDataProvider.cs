using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Leaderboard.Interfaces
{
    using Data;
    public interface ILeaderboardDataProvider
    {
        UniTask<IReadOnlyList<LeaderboardEntry>> GetAllEntriesAsync();
        UniTask<IReadOnlyList<LeaderboardEntry>> GetEntriesAsync(int start, int count);
        UniTask<int> GetMaxCapacityAsync();
        UniTask<LeaderboardEntry> GetPlayerEntry();
    }
}