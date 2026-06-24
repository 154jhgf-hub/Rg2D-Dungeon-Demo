using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ToastUI : MonoBehaviour
{
    private static ToastUI instance;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float showDuration = 1.2f;
    [SerializeField] private float fadeDuration = 0.3f;

    private CanvasGroup canvasGroup;
    private Coroutine showRoutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        if (messageText == null)
        {
            messageText = GetComponentInChildren<TextMeshProUGUI>();
        }
        HideImmediate();
    }

    public static void Show(string message)
    {
        if (instance == null)
        {
            Debug.LogWarning("ToastUI is missing in the scene.");
            return;
        }

        instance.ShowInternal(message);
    }

    private void ShowInternal(string message)
    {
        if (messageText == null)
        {
            Debug.LogWarning("ToastUI has no message text assigned.");
            return;
        }

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }

        messageText.text = message;
        showRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        canvasGroup.alpha = 1f;
        yield return new WaitForSecondsRealtime(showDuration);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        HideImmediate();
        showRoutine = null;
    }

    private void HideImmediate()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}
