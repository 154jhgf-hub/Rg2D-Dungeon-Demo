
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public StartMenu startMenu;
    public EndUI endUI;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        AudioManager.Instance.PlayBgm(AudioManager.Instance.bgmClips[0],1f);
        EnemyManager.Instance.OnAllEnemiesDefeated += GameVictory;
    }

    public void ResetGame()
    {
        Time.timeScale = 1;
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach(GameObject gameObject in items)
        {
            DestroyImmediate(gameObject);
        }
    }

    public void ExitGame()
    {
      #if UNITY_EDITOR
        // 在Unity编辑器中停止运行
        UnityEditor.EditorApplication.isPlaying = false;
      #else
            // 在打包后的游戏中退出
            Application.Quit();
      #endif
    }
    private void GameVictory()
    {
        endUI.SetText("游戏胜利");
        endUI.Show();
        Time.timeScale = 0;
    }
}
