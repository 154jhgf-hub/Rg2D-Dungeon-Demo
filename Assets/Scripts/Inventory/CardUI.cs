using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IDragHandler, IBeginDragHandler, IEndDragHandler,IPointerClickHandler
{
    [SerializeField]
    public TextMeshProUGUI amount;
    [SerializeField]
    public Image icon;
    [SerializeField]
    public TextMeshProUGUI itemName;
    private bool isHovered;
    public int cardIndex;
    private CanvasGroup canvasGroup;
    public Vector3 offset;

    private void Start()
    {
        amount = transform.Find("Item/number").GetComponent<TextMeshProUGUI>();
        itemName = transform.Find("Item/name").GetComponent<TextMeshProUGUI>();
        icon = transform.Find("Item/icon").GetComponent<Image>();
        canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    public void SetCardIndex(int index)
    {
        cardIndex = index;
    }

    public void UpdateCard(Item item)
    {
        if (item == null)
        {
            amount.text = "";
            icon.sprite = null;
            itemName.text = "";
        }
        else
        {
            amount.text = item.currentAmount != 0 ? item.currentAmount.ToString() : "";
            icon.sprite = Resources.Load<Sprite>(item.iconpath);
            itemName.text = item.itemName;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Card card = InventoryManager.Instance.GetInventory().cards[cardIndex];
        if (!card.HasItem())
        {
            return;
        }
        DragManager.Instance.StartDrag(card.bindItem, this);
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragManager.Instance.Drag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragManager.Instance.EndDrag();
        canvasGroup.alpha = 1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (DragManager.Instance.IsDrag)
        {
            DragManager.Instance.SetEndCard(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (DragManager.Instance.IsDrag)
        {
            DragManager.Instance.SetEndCard(null);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ¼ì²âÓÒ¼üµã»÷
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (InventoryManager.Instance.inventoryData.cards[cardIndex].bindItem == null)
            {
                return;
            }

            Vector2 screenPos = GetComponent<RectTransform>().position+offset;
            UseMenu.Instance.Show(
                InventoryManager.Instance.inventoryData.cards[cardIndex].bindItem, cardIndex,screenPos
               );
        }
        
    }
}
