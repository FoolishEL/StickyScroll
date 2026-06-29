using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CMS
{
    public class ContentManagementSystem : IContentManagementSystem
    {
        const string PREFAB_DEFAULT_PATH = "Prefabs/";
        Dictionary<Type, string> typesCache;
        Dictionary<Type, Dictionary<string, string>> typesCacheWithIds;
        public ContentManagementSystem()
        {
            typesCache = new();
            typesCacheWithIds = new();
            AddPrefab<Leaderboard.LeaderboardContentContainer>("LeaderboardContentContainer");
            AddPrefab<Leaderboard.View.LeaderboardItemView>("LeaderboardItemView");
            AddPrefab<Leaderboard.View.LeaderboardExample>("LeaderboardScroll");
            AddPrefab<Leaderboard.Data.StickyLeaderboardView>("LeaderboardItemView", "Bottom");
            AddPrefab<Leaderboard.Data.StickyLeaderboardView>("LeaderboardItemView", "Top");
        }

        public T LoadPrefab<T>() where T : MonoBehaviour
        {
            if (!typesCache.TryGetValue(typeof(T), out var resourcePath))
            {
                throw new KeyNotFoundException($"{typeof(T)} does not exist");
            }
            var prefab = Resources.Load<T>(resourcePath);
            return prefab == null ? throw new($"Failed to load prefab at path {resourcePath}") : prefab;
        }
        public T LoadPrefab<T>(string id) where T : MonoBehaviour
        {
            if(!typesCacheWithIds.TryGetValue(typeof(T), out var ids))
                throw new KeyNotFoundException($"{typeof(T)} does not exist");
            if (!ids.TryGetValue(id, out var path))
            {
                throw new KeyNotFoundException($"id {id} does not exit for type {typeof(T)}");
            }
            var prefab = Resources.Load<T>(path);
            return prefab == null ? throw new($"Failed to load prefab at path {path}") : prefab;
        }

        private void AddPrefab<T>(string path) => typesCache.Add(typeof(T), PREFAB_DEFAULT_PATH + path);
        private void AddPrefab<T>(string path, string id)
        {
            if (!typesCacheWithIds.TryGetValue(typeof(T), out var ids))
            {
                ids = new();
                typesCacheWithIds.Add(typeof(T), ids);
            }
            ids.Add(id, $"{PREFAB_DEFAULT_PATH}{path}_{id}");
        }
    }
}