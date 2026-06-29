using System;
using UnityEngine;

namespace Game.InfiniteScroll
{
    public abstract class AbstractContentContainer : MonoBehaviour
    {
        [field: SerializeField]
        public RectTransform RectTransform { get; private set; }

        public int Index { get; private set; }
        public event Action<int, AbstractContentContainer> OnIndexChange = delegate {};

        public virtual void SetIndex(int newIndex)
        {
            Index = newIndex;
            OnIndexChange?.Invoke(Index, this);
        }
    }
}