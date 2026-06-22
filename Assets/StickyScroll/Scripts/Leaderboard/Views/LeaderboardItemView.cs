using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Leaderboard.View
{
    using Data;
    using Interfaces;
    
    public sealed class LeaderboardItemView : MonoBehaviour, ILeaderboardItemView
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Image backgroundImage;
        
        [SerializeField] private Color playerColor;
        [SerializeField] private Color normalColor;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [SerializeField] private GameObject currentUserMark;

        public void Bind(LeaderboardEntry entry)
        {
            rankText.text = entry.Rank.ToString();
            nameText.text = entry.Name;
            scoreText.text = entry.Score.ToString();

            currentUserMark.SetActive(entry.IsCurrentUser);
            backgroundImage.color = entry.IsCurrentUser ? playerColor : normalColor;
        }

        public void SetStatus(bool isActive)
        {
            canvasGroup.alpha = isActive ? 1 : 0;
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;
        }
    }   
}