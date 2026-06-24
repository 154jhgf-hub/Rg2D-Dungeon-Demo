using UnityEngine;
using UnityEngine.UI;

public class HPUI : MonoBehaviour
{
    private GameObject target;
    private Vector3 offset;
    private Slider slider;
    private RectTransform rectTransform;
    private Health health;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        rectTransform = GetComponent<RectTransform>();
        SetupSliderAsDisplayOnly();
    }



    private void OnDisable()
    {
        health.OnDamage -= HpUIUpdate;
        health.OnAdd -= HpUIUpdate;
    }
    private void Start()
    {
       
        health.OnAdd += HpUIUpdate;
    }

    private void LateUpdate()
    {
        FollowTarget();
    }
    public void SetTarget(GameObject gameObject,Vector2 pos)
    {
        target = gameObject;
        offset = pos;
        health = gameObject.GetComponent<Health>();
        health.OnDamage += HpUIUpdate;
        HpUIUpdate(0);
        if (target.CompareTag("Player"))
        {
            PlayerControl temp = target.GetComponent<PlayerControl>();
            temp.OnLevelUpHpUI += HpUIUpdate;
        }
    }

    public void HpUIUpdate(float a)
    {
        slider.value = Mathf.Clamp01((float)health.currentHp / health.maxHp);
    }

    private void SetupSliderAsDisplayOnly()
    {
        if (slider == null)
        {
            return;
        }

        slider.interactable = false;
        Navigation navigation = slider.navigation;
        navigation.mode = Navigation.Mode.None;
        slider.navigation = navigation;

        foreach (Graphic graphic in slider.GetComponentsInChildren<Graphic>())
        {
            graphic.raycastTarget = false;
        }
    }

    private void FollowTarget()
    {
        if (target != null)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint
                (Camera.main,(target.transform.position + offset));
            rectTransform.position = screenPos;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
