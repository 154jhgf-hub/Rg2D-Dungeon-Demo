using UnityEngine;

public class StartMenu : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private BgFollow bgFollow;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        bgFollow = FindObjectOfType<BgFollow>();
    }

    public void Hide()
    {
        if (bgFollow != null)
        {
            bgFollow.StartGame();
        }
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        if (bgFollow != null)
        {
            bgFollow.ShowMenu();
        }
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
