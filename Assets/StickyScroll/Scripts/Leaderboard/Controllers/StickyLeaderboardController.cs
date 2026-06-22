using UnityEngine;
using VContainer;

namespace Game.Leaderboard.Controllers
{
    using CMS;
    using InfiniteScroll;
    using Data;

    public sealed class StickyLeaderboardController : MonoBehaviour
    {
        [Header("Scroll")]
        [SerializeField] private InfiniteScroll scrollRect;

        [SerializeField] private RectTransform viewport;

        [Header("Sticky")]
        [SerializeField] private RectTransform stickiesSpawnRoot;
        [SerializeField] private StickyLeaderboardView topSticky;

        [SerializeField] private StickyLeaderboardView bottomSticky;

        private LeaderboardEntry userEntry;
        private RectTransform userItem;
        private bool isCurrentDown;

        [Inject]
        private void Construct(IContentManagementSystem cms)
        {
            var topView = cms.LoadPrefab<StickyLeaderboardView>("Top");
            topSticky = Instantiate(topView, stickiesSpawnRoot);
            var bottomView = cms.LoadPrefab<StickyLeaderboardView>("Bottom");
            bottomSticky =  Instantiate(bottomView, stickiesSpawnRoot);
        }

        public void Initialize(LeaderboardEntry userEntry)
        {
            this.userEntry = userEntry;
            userItem = null;

            scrollRect.OnScroll += OnScroll;

            RefreshView();
        }

        public void SetUserItem(RectTransform userItem)
        {
            this.userItem = userItem;
            RefreshView();
        }

        public void UnsetUserItem(bool isDown)
        {
            userItem = null;
            isCurrentDown = isDown;
            RefreshView();
        }

        private void OnDestroy() => scrollRect.OnScroll -= OnScroll;

        private void RefreshView() => OnScroll(0f);

        private void OnScroll(float delta)
        {
            if (userItem == null)
            {
                if (isCurrentDown)
                {
                    bottomSticky.Show(userEntry);
                    topSticky.Hide();
                }
                else
                {
                    topSticky.Show(userEntry);
                    bottomSticky.Hide();
                }
                return;
            }

            var viewportCorners = new Vector3[4];
            var targetCorners = new Vector3[4];

            viewport.GetWorldCorners(viewportCorners);
            userItem.GetWorldCorners(targetCorners);

            var viewportTop = viewportCorners[1].y;
            var viewportBottom = viewportCorners[0].y;

            var targetTop = targetCorners[1].y;
            var targetBottom = targetCorners[0].y;

            var fullyVisible =
                targetTop <= viewportTop &&
                targetBottom >= viewportBottom;

            if (fullyVisible)
            {
                topSticky.Hide();
                bottomSticky.Hide();
                return;
            }

            if (targetTop > viewportTop)
            {
                topSticky.Show(userEntry);
                bottomSticky.Hide();
                return;
            }

            if (targetBottom < viewportBottom)
            {
                bottomSticky.Show(userEntry);
                topSticky.Hide();
            }
        }
    }
}