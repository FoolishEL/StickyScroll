using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.InfiniteScroll
{
    using Extensions;
    
    public class InfiniteScroll : MonoBehaviour,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler
    {
        [SerializeField] private RectTransform spawnRoot;

        [Header("Scroll Settings")]
        [SerializeField] private float deceleration = 8f;

        [SerializeField] private float dragSensitivity = 1f;

        [Header("Index Limits")]
        [SerializeField] private int maxIndex = -1;

        private float velocity;
        private bool dragging;

        private AbstractContentContainer[] contents;

        public event Action<float> OnScroll = delegate {};

        private float contentHeight;

        private bool isInitialized;

        public T[] Initialize<T>(T containerPrefab) where T : AbstractContentContainer
        {
            if (isInitialized)
                return Array.Empty<T>();

            isInitialized = true;
            var contentsTemp = new T[3];
            contents = new AbstractContentContainer[3];

            contentHeight = containerPrefab.RectTransform.sizeDelta.y;

            for (int i = 0; i < 3; i++)
            {
                contentsTemp[i] = Instantiate(containerPrefab, spawnRoot);
                contents[i] = contentsTemp[i];

                contents[i].RectTransform.anchoredPosition = new(0, (1 - i) * contentHeight);

                contents[i].SetIndex(i);
            }
            return contentsTemp;
        }
        
        public void SetMaxIndex(int newMaxIndex)
        {
            maxIndex = newMaxIndex;
        }

        private void Update()
        {
            if (dragging)
                return;

            if (Mathf.Abs(velocity) > 0.01f)
            {
                float delta = velocity * Time.unscaledDeltaTime;
                MoveContent(delta);

                velocity = Mathf.Lerp(velocity, 0, deceleration * Time.unscaledDeltaTime);
            }
            else
            {
                velocity = 0;
            }
        }

        #region Drag handling

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
            velocity = 0;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            float delta = eventData.delta.y * dragSensitivity;
            MoveContent(delta);
            velocity = delta / Time.unscaledDeltaTime;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }

        #endregion

        private void MoveContent(float delta)
        {
            CheckDelta(ref delta);

            for (int i = 0; i < contents.Length; i++)
            {
                Vector2 pos = contents[i].GetAnchoredPosition();
                pos.y += delta;
                contents[i].SetAnchoredPosition(pos);
            }
            OnScroll?.Invoke(delta);
            RecycleIfNeeded();
        }
        
        private void CheckDelta(ref float delta)
        {
            var minIndex = GetMinAssignedIndex();
            float desiredY;
            if (minIndex == 0)
            {
                var content = GetContentWithIndex(1);
                if (content != null)
                {
                    desiredY = content.GetAnchoredPosition().y + delta;
                    if (desiredY < 0)
                        delta = -content.GetAnchoredPosition().y;
                }
            }

            if (maxIndex < 0 || GetMaxAssignedIndex() != maxIndex)
                return;
            
            var bottomContent = GetContentWithIndex(maxIndex);
            
            if (bottomContent == null)
                return;
            
            desiredY = bottomContent.GetAnchoredPosition().y + delta;
            
            if (desiredY > -contentHeight)
                delta = -contentHeight - bottomContent.GetAnchoredPosition().y;
        }

        private AbstractContentContainer GetContentWithIndex(int index)
        {
            for (int i = 0; i < contents.Length; i++)
            {
                if (contents[i].Index == index)
                    return contents[i];
            }

            return null;
        }

        private void RecycleIfNeeded()
        {
            while (true)
            {
                AbstractContentContainer topAbstractContent = GetTopContent();

                if (topAbstractContent.GetAnchoredPosition().y > contentHeight * 2f)
                {
                    if (maxIndex >= 0 && GetMaxAssignedIndex() >= maxIndex)
                        break;

                    AbstractContentContainer bottomAbstractContent = GetBottomContent();

                    topAbstractContent.SetAnchoredPosition(new(
                        topAbstractContent.GetAnchoredPosition().x,
                        bottomAbstractContent.GetAnchoredPosition().y - contentHeight));

                    int newIndex = GetMaxAssignedIndex() + 1;
                    topAbstractContent.SetIndex(newIndex);
                }
                else
                {
                    break;
                }
            }
            
            while (true)
            {
                AbstractContentContainer bottomAbstractContent = GetBottomContent();

                if (bottomAbstractContent.GetAnchoredPosition().y < -contentHeight * 2f)
                {
                    int newIndex = GetMinAssignedIndex() - 1;

                    if (newIndex < 0)
                        break;

                    AbstractContentContainer topAbstractContent = GetTopContent();

                    bottomAbstractContent.SetAnchoredPosition(new(
                        bottomAbstractContent.GetAnchoredPosition().x,
                        topAbstractContent.GetAnchoredPosition().y + contentHeight));

                    bottomAbstractContent.SetIndex(newIndex);
                }
                else
                {
                    break;
                }
            }
        }

        private AbstractContentContainer GetTopContent()
        {
            AbstractContentContainer result = contents[0];
            for (var index = 1; index < contents.Length; index++)
            {
                var content = contents[index];
                if (content.Index < result.Index)
                    result = content;
            }
            return result;
        }

        private AbstractContentContainer GetBottomContent()
        {
            AbstractContentContainer result = contents[0];
            for (var index = 1; index < contents.Length; index++)
            {
                var content = contents[index];
                if (content.Index > result.Index)
                    result = content;
            }
            return result;
        }

        private int GetMinAssignedIndex()
        {
            int min = contents[0].Index;
            for (int i = 1; i < contents.Length; i++)
            {
                min = Mathf.Min(min, contents[i].Index);
            }
            return min;
        }

        private int GetMaxAssignedIndex()
        {
            int max = contents[0].Index;
            for (int i = 1; i < contents.Length; i++)
            {
                max = Mathf.Max(max, contents[i].Index);
            }
            return max;
        }
    }
}