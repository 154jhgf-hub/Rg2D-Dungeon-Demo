
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UseMenu : MonoBehaviour
{
    public static UseMenu Instance { private set; get; }
    public GameObject useMenu;
    public Button useBtn;
    public Item currentItem;
    public int cardIndex;
    public RectTransform canvasRect;
    public TextMeshProUGUI itemText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        useBtn = GetComponentInChildren<Button>();
        itemText = transform.Find("ItemText").GetComponent<TextMeshProUGUI>();
        useBtn.onClick.AddListener(UseItem);
        gameObject.SetActive(false);
        

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (gameObject.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                gameObject.GetComponent<RectTransform>(),
                Input.mousePosition))
            {
                Hide();
            }
        }
    }

    public void Show(Item item,int cardindex,Vector2 pos)
    {
        currentItem = item;
        cardIndex = cardindex;
        itemText.text = item.itemName;
        GetComponent<RectTransform>().position =pos;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        currentItem = null;
        gameObject.SetActive(false);
    }

    private void UseItem()
    {
        if (currentItem != null)
        {
            UseItemManager.Instance.UseItem(currentItem, cardIndex);
        }
        Hide();
    }
}
