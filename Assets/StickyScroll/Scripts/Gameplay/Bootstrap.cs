using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay
{
    using Leaderboard.View;
    using CMS;

    public class Bootstrap : MonoBehaviour
    {
        [SerializeField]
        LifetimeScope lifetimeScope;

        void Start()
        {
            lifetimeScope.Build();
            CreateStage(lifetimeScope.Container);
        }

        void CreateStage(IObjectResolver resolver)
        {
            var cms = resolver.Resolve<IContentManagementSystem>();
            var viewPrefab = cms.LoadPrefab<LeaderboardExample>();
            resolver.Instantiate(viewPrefab);
        }
    }
}