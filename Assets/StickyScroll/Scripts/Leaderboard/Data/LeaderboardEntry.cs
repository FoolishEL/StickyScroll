namespace Game.Leaderboard.Data
{
    public readonly struct LeaderboardEntry
    {
        public readonly string Id;
        public readonly string Name;
        public readonly int Rank;
        public readonly int Score;
        public readonly bool IsCurrentUser;

        public LeaderboardEntry(
            string id,
            string name,
            int rank,
            int score,
            bool isCurrentUser)
        {
            Id = id;
            Name = name;
            Rank = rank;
            Score = score;
            IsCurrentUser = isCurrentUser;
        }
    }
}