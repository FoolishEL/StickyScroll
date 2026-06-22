using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Game.Leaderboard.Controllers
{
    using CMS;
    using InfiniteScroll;
    using Data;
    using View;
    using Interfaces;

    public class LeaderboardPresenter : MonoBehaviour
    {
        [SerializeField] private InfiniteScroll infiniteScroll;
        [SerializeField] private StickyLeaderboardController controller;

        private Dictionary<AbstractContentContainer, LeaderboardContentContainer> containers;
        private ILeaderboardDataProvider dataProvider;
        private int playerContainerIndex;
        private int elementsPerContent;

        private LeaderboardContentContainer leaderboardContentContainer;
        private LeaderboardItemView itemPrefab;

        private void Awake() => Initialize().Forget();

        [Inject]
        private void Construct(IContentManagementSystem cms, ILeaderboardDataProvider leaderboardDataDataProvider, ISettingsProvider settingsProvider)
        {
            itemPrefab = cms.LoadPrefab<LeaderboardItemView>();
            leaderboardContentContainer = cms.LoadPrefab<LeaderboardContentContainer>();
            dataProvider = leaderboardDataDataProvider;
            elementsPerContent = settingsProvider.GetData<int>("elementsPerContent");
        }

        private async UniTaskVoid Initialize()
        {
            var contentsCount = await GetContentCount();
            var playerEntry = await dataProvider.GetPlayerEntry();
            containers = new();
            
            controller.Initialize(playerEntry);
            controller.UnsetUserItem(true);
            playerContainerIndex = GetPlayerContainerIndex(playerEntry);
            var abstractContainers = infiniteScroll.Initialize(leaderboardContentContainer);
            infiniteScroll.SetMaxIndex(contentsCount);
            foreach (var abstractContainer in abstractContainers)
            {
                abstractContainer.OnIndexChange += OnContentIndexChanged;
                containers.Add(abstractContainer, abstractContainer);
                abstractContainer.Initialize(CreateItem, elementsPerContent);
                OnContentIndexChanged(abstractContainer.Index, abstractContainer);
            }
        }

        int GetPlayerContainerIndex(LeaderboardEntry playerEntry)
        {
            var index = playerEntry.Rank / elementsPerContent;
            if (playerEntry.Rank % elementsPerContent != 0)
                index++;
            return index;
        }

        private async UniTask<int> GetContentCount()
        {
            var maxCount = await dataProvider.GetMaxCapacityAsync();
            var remainder = maxCount % elementsPerContent;
            var contentsCount = maxCount / elementsPerContent + 1;
            if (remainder > 0)
                contentsCount++;
            return contentsCount;
        }

        private LeaderboardItemView CreateItem(RectTransform rectTransform) => Instantiate(itemPrefab, rectTransform);

        private void OnDestroy()
        {
            if (containers is null)
            {
                return;
            }
            foreach (var container in containers.Values)
            {
                container.OnIndexChange -= OnContentIndexChanged;
            }
        }

        private void OnContentIndexChanged(int newIndex, AbstractContentContainer container)
        {
            UpdateView(containers[container]).Forget();
        }

        private async UniTaskVoid UpdateView(LeaderboardContentContainer container)
        {
            var index = container.Index - 1;
            bool hasPlayer = playerContainerIndex == index + 1;
            if (index < 0)
            {
                foreach (var leaderboardItemView in container.Views)
                {
                    leaderboardItemView.SetStatus(false);
                }
                return;
            }
            CheckPlayer();
            var entries = await dataProvider.GetEntriesAsync(index * elementsPerContent, elementsPerContent);
            for (var i = 0; i < container.Views.Count; i++)
            {
                var view = container.Views[i];
                if (entries.Count - 1 >= i)
                {
                    if (hasPlayer && entries[i].IsCurrentUser)
                        controller.SetUserItem(view.RectTransform);
                    
                    view.Bind(entries[i]);
                    view.SetStatus(true);
                }
                else
                    view.SetStatus(false);
            }
        }

        void CheckPlayer()
        {
            int maxIndex = int.MinValue;
            foreach (var container in containers.Values)
            {
                var containerIndex = container.Index;
                if (containerIndex == 0)
                    continue;
                bool hasPlayer = playerContainerIndex == containerIndex;
                if (hasPlayer)
                    return;
                if (containerIndex > maxIndex)
                {
                    maxIndex = containerIndex;
                }
            }
            controller.UnsetUserItem(maxIndex < playerContainerIndex);
        }
    }
}