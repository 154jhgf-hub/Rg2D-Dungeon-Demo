using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance { private set; get; }
    private CardUI startCard;
    public CardUI endCard;
    private Item dragItem;
    private bool isDrag;
    [SerializeField]
    private Image dragIcon;

    public bool IsDrag => isDrag;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        dragIcon.enabled = false;
    }
    public void SetEndCard(CardUI cardUI)
    {
        endCard = cardUI;
    }
    public void StartDrag(Item item,CardUI cardUI)
    {
        dragIcon.enabled = true;
        dragIcon.color = Color.white;
        isDrag = true;
        startCard = cardUI;
        dragItem = item;
        dragIcon.sprite = startCard.icon.sprite;
    }

    public void Drag(Vector2 pos)
    {
        if (isDrag)
        {
            dragIcon.rectTransform.position= pos;
        }
    }

    public void EndDrag()
    {
        if (!isDrag)
        {
            return;
        }
        HandleDrag();
        ResetDrag();
    }

    private void ResetDrag()
    {
        isDrag = false;
        startCard = null;
        endCard = null;
        dragItem = null;
        dragIcon.color = Color.clear;
        dragIcon.sprite = null;
        dragIcon.enabled = false;
    }

    private void HandleDrag()
    {
        if (endCard == null)
        {
            return;
        }
        Item targetItem = InventoryManager.Instance.GetInventory().
            cards[endCard.cardIndex].bindItem;
        
        if (targetItem != null && targetItem.id == dragItem.id&&
            targetItem.currentAmount<targetItem.maxStack)
        {
            int canAdd = targetItem.maxStack - targetItem.currentAmount;
            if (canAdd >= dragItem.currentAmount)
            {
                targetItem.currentAmount += dragItem.currentAmount;
                InventoryManager.Instance.RemoveItem(startCard.cardIndex, dragItem.currentAmount);
            }
            else if(canAdd>0)
            {
                targetItem.currentAmount = targetItem.maxStack;
                dragItem.currentAmount -= canAdd;
                InventoryManager.Instance.RemoveItem(startCard.cardIndex, canAdd);
            }
        }
        else if(targetItem!=null&&targetItem.id!=dragItem.id)
        {
            InventoryManager.Instance.SwapItem(startCard.cardIndex, endCard.cardIndex);
        }
        else
        {
            InventoryManager.Instance.SwapItem(startCard.cardIndex, endCard.cardIndex);
        }
    }
}
