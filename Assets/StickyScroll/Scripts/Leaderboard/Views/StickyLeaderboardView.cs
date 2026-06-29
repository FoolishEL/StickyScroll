using UnityEngine;

namespace Game.Leaderboard.Data
{
    using Interfaces;
    using View;
    
    public sealed class StickyLeaderboardView : MonoBehaviour, IStickyLeaderboardView
    {
        [SerializeField] private LeaderboardItemView view;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Show(LeaderboardEntry entry)
        {
            canvasGroup.Show();
            view.Bind(entry);
        }

        public void Hide() => canvasGroup.Hide();
    }
}