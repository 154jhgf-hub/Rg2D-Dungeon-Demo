using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    private CanvasGroup canvasGroup;
    public CardUI[] cardUIs=new CardUI[20];
    private bool isOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        for(int i = 0; i < cardUIs.Length; i++)
        {
            cardUIs[i].SetCardIndex(i);
        }
        InventoryManager.Instance.OnInventoryChange += RefreshUI;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
    private void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void RefreshUI()
    {
        var inventory = InventoryManager.Instance.GetInventory();
        for (int i = 0; i < cardUIs.Length; i++)
        {
            cardUIs[i].UpdateCard(inventory.cards[i].bindItem);
        }
    }
}
