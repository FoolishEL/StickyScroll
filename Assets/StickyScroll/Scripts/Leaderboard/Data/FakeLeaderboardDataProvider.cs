using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace Game.Leaderboard.Data
{
    using CMS;
    using Interfaces;
    
    public sealed class FakeLeaderboardDataProvider : ILeaderboardDataProvider
    {
        private const int MAX_SCORE = 100_000;
        private const int PLAYER_INDEX = 25;

        private readonly int entriesCount;
        private readonly int playerIndex;
        private Random random;
        private int currentScore = MAX_SCORE;
        private string[] prefixes = { "Dark", "Pro", "Crazy", "Ghost", "Shadow", "Neo", "Fire", "Ice", "Storm", "Lucky", };
        private string[] suffixes = { "Slayer", "King", "God", "Ninja", "Legend", "Wolf", "Phoenix", "Dragon", "Byte", "Void", };

        private List<LeaderboardEntry> cachedEntries;
        private readonly object cacheLock = new();

        public FakeLeaderboardDataProvider(ISettingsProvider settingsProvider, int playerIndex = PLAYER_INDEX)
        {
            entriesCount = settingsProvider.GetData<int>("elementsPerContent") * 3;
            entriesCount = Mathf.Max(entriesCount, 1);
            this.playerIndex = Mathf.Clamp(playerIndex, 0, entriesCount);
            cachedEntries = new();
            random = new();
        }

        public FakeLeaderboardDataProvider(int entriesCount = 50, int playerIndex = PLAYER_INDEX)
        {
            this.entriesCount = Mathf.Max(entriesCount, 1);
            this.playerIndex = Mathf.Clamp(playerIndex, 0, this.entriesCount);
            cachedEntries = new();
            random = new();
        }

        public UniTask<int> GetMaxCapacityAsync() => UniTask.FromResult(entriesCount);
        public UniTask<LeaderboardEntry> GetPlayerEntry()
        {
            lock (cacheLock)
            {
                EnsurePlayerExists();
                return UniTask.FromResult(cachedEntries[playerIndex]);
            }
        }

        public UniTask<IReadOnlyList<LeaderboardEntry>> GetAllEntriesAsync()
        {
            lock (cacheLock)
            {
                if (cachedEntries == null)
                {
                    GenerateEntries();
                }

                return UniTask.FromResult<IReadOnlyList<LeaderboardEntry>>(cachedEntries);
            }
        }

        public UniTask<IReadOnlyList<LeaderboardEntry>> GetEntriesAsync(int start, int count)
        {
            if (start < 0)
                start = 0;

            if (count <= 0)
                return UniTask.FromResult<IReadOnlyList<LeaderboardEntry>>(new List<LeaderboardEntry>());

            lock (cacheLock)
            {
                if (cachedEntries == null)
                {
                    GenerateEntries();
                }

                var totalCount = cachedEntries!.Count;

                if (start >= totalCount)
                {
                    return UniTask.FromResult<IReadOnlyList<LeaderboardEntry>>(new List<LeaderboardEntry>());
                }

                var actualCount = Mathf.Min(count, totalCount - start);

                var result = new List<LeaderboardEntry>(actualCount);

                for (var i = 0; i < actualCount; i++)
                {
                    result.Add(cachedEntries[start + i]);
                }

                return UniTask.FromResult<IReadOnlyList<LeaderboardEntry>>(result);
            }
        }

        private void EnsurePlayerExists()
        {
            if (cachedEntries != null && playerIndex < cachedEntries!.Count)
                return;

            GenerateEntriesWithPlayer();
        }

        private void GenerateEntriesWithPlayer()
        {
            int requiredCount = Mathf.Max(entriesCount, playerIndex + 1);
            
            currentScore = MAX_SCORE;

            for (int i = requiredCount; i > 0; i--)
            {
                int rank = requiredCount - i + 1;
                bool isPlayer = rank == playerIndex + 1;

                cachedEntries.Add(new(
                    $"player_{rank}",
                    GenerateFancyName(),
                    rank,
                    GetRandomScore(),
                    isPlayer));
            }
        }

        private void GenerateEntries()
        {
            var result = new List<LeaderboardEntry>(entriesCount);

            currentScore = MAX_SCORE;

            for (var i = entriesCount; i > 0; i--)
            {
                var index = entriesCount - i + 1;
                result.Add(new(
                    $"player_{index}",
                    GenerateFancyName(),
                    index,
                    GetRandomScore(),
                    index == playerIndex));
            }

            cachedEntries = result;
        }

        private int GetRandomScore()
        {
            var dropFactor = random.Next(92, 100) / 100f;
            if (random.Next(0, 100) < 25)
                dropFactor = random.Next(75, 92) / 100f;

            currentScore = Mathf.Max(0, Mathf.FloorToInt(currentScore * dropFactor));

            if (random.Next(0, 100) < 40)
                currentScore += random.Next(-15, 16);

            currentScore = Mathf.Clamp(currentScore, 0, MAX_SCORE);
            return currentScore;
        }

        private string GenerateFancyName()
        {
            if (random.Next(0, 100) < 15)
                return $"Player_{random.Next(1000, 9999)}";

            if (random.Next(0, 100) < 25)
                return $"xX_{prefixes[random.Next(0, prefixes.Length)]}_{suffixes[random.Next(0, suffixes.Length)]}_Xx";

            return $"{prefixes[random.Next(0, prefixes.Length)]}{suffixes[random.Next(0, suffixes.Length)]}{random.Next(1, 100)}";
        }
    }
}