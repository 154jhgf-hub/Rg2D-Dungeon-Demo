using UnityEngine;

public class LevelFollow : MonoBehaviour
{
    private GameObject player;
    public Vector3 offset;
    private RectTransform rectTransform;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (player != null)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint
                (Camera.main, (player.transform.position + offset));
            rectTransform.position = screenPos;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
