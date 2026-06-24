
using UnityEngine;

public class BgFollow : MonoBehaviour
{
    public float moveDistance = 2f; // ����ƶ�����
    public float speed = 0.5f;      // �ƶ��ٶ�

    private Vector3 startPos;
    private bool isGame = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!isGame)
        {
            Bg();
        }
    }

    public void StartGame()
    {
        isGame = true;
    }

    public void ShowMenu()
    {
        isGame = false;
    }

    private void Bg()
    {
        float x = Mathf.Sin(Time.unscaledTime * speed) * moveDistance;

        transform.position =
            new Vector3(
                startPos.x + x,
                startPos.y,
                startPos.z
            );
    }
}
