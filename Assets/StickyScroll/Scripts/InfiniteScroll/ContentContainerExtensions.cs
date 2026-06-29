using UnityEngine;

namespace Game.InfiniteScroll.Extensions
{
    public static class ContentContainerExtensions
    {
        public static Vector2 GetAnchoredPosition(this AbstractContentContainer container)
        {
            if (!container)
                return Vector2.zero;
            return container.RectTransform.anchoredPosition;
        }

        public static void SetAnchoredPosition(this AbstractContentContainer container, Vector2 anchoredPosition)
        {
            if (!container)
                return;
            container.RectTransform.anchoredPosition = anchoredPosition;
        }
    }
}