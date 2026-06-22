using VContainer;
using VContainer.Unity;

namespace Game.Gameplay
{
    using CMS;
    using Leaderboard.Data;
    using Leaderboard.Interfaces;

    public class GameplayScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register(_ => new ContentManagementSystem(), Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register(_ => new DummySettingsProvider(), Lifetime.Scoped).As<ISettingsProvider>();
            builder.Register(resolver => new FakeLeaderboardDataProvider(resolver.Resolve<ISettingsProvider>()), Lifetime.Scoped).As<ILeaderboardDataProvider>();
        }
    }
}