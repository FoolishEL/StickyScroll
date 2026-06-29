using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Leaderboard
{
    using InfiniteScroll;
    using View;

    public class LeaderboardContentContainer : AbstractContentContainer
    {
        [SerializeField] private RectTransform spawnRoot;

        private LeaderboardItemView[] views;
        public IReadOnlyList<LeaderboardItemView> Views => views;

        public void Initialize(Func<RectTransform, LeaderboardItemView> createFunc, int capacity)
        {
            views = new LeaderboardItemView[capacity];
            for (int i = 0; i < capacity; i++)
            {
                views[i] = createFunc(spawnRoot);
            }
        }
    }
}