using UnityEngine;

public class MiddleUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool isShow;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isShow = !isShow;
            if (isShow)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

       
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        Time.timeScale = 1;
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Time.timeScale = 0;
    }
}
