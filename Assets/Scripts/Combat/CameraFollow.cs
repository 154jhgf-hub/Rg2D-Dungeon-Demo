using UnityEngine;



public class CameraFollow : MonoBehaviour
{

    public enum CameraState
    {
        Menu,
        FollowPlayer
    }
    public GameObject target;
    public Vector3 offset;
    public RectTransform rectTransform;
    public float moveDistance = 2f; // 最大移动距离
    public float speed = 0.5f;      // 移动速度
    private Vector3 startPos;
    public CameraState cameraState=CameraState.Menu;
    private void Start()
    {
        target = GameObject.FindWithTag("Player");
        rectTransform = GetComponent<RectTransform>();
        startPos = transform.position;
    }

    private void Update()
    {
        switch (cameraState)
        {
            case CameraState.Menu:
                BgFollow();
                break;
            case CameraState.FollowPlayer:
                FollowTarget();
                break;
        }
    }
    private void FollowTarget()
    {
        if (target != null)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint
                (Camera.main, (target.transform.position + offset));
            rectTransform.position = screenPos;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BgFollow()
    {
        float x = Mathf.Sin(Time.time * speed) * moveDistance;

        transform.position =
            new Vector3(
                startPos.x + x,
                startPos.y,
                startPos.z
            );
    }

    public void StartGame()
    {
        cameraState = CameraState.FollowPlayer;
    }
}
