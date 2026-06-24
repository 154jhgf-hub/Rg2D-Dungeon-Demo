
using UnityEngine;
using UnityEngine.UI;

public class AudioUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;
    private CanvasGroup canvasGroup;
    public StartMenu startMenu;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        LoadVolume();
    }
    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        AudioManager.Instance.SetMasterVolume(volume);
    }

    public void SetBgmVolume()
    {
        float volume = musicSlider.value;
        AudioManager.Instance.SetBgmVolume(volume);
    }

    public void SetSoundVolume()
    {
        float volume = soundSlider.value;
        AudioManager.Instance.SetSoundVolume(volume);
    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume", 0.8f);
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.6f);
        soundSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.8f);
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        startMenu.Show();
    }

    public void Show()
    {
        startMenu.Hide();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
