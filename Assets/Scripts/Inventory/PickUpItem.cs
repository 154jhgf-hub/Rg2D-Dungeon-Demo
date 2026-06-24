using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public Item item;
    private bool isPickUp;

    private void Update()
    {
        if (isPickUp && Input.GetKeyDown(KeyCode.K))
        {
            PickUp();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.CompareTag("Player"))
        {
            isPickUp = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPickUp = false;
        }
    }

    private void PickUp()
    {
        bool success = InventoryManager.Instance.AddItem(item);
        if (success)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.pickupSounds[0]);
            ToastUI.Show($"拾取{item.itemName}");
            Destroy(gameObject);

        }
        else
        {
            ToastUI.Show("背包已满");
        }
    }
}
