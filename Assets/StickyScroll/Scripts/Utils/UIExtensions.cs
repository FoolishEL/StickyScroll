using UnityEngine;

public static class UIExtensions
{
    public static void Show(this CanvasGroup canvasGroup)
    {
        if(!canvasGroup)
            return;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public static void Hide(this CanvasGroup canvasGroup)
    {
        if(!canvasGroup)
            return;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}