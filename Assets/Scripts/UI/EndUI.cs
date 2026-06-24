
using TMPro;
using UnityEngine;

public class EndUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI endUI;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetText(string ttt)
    {
        endUI.text = ttt;
    }
    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
