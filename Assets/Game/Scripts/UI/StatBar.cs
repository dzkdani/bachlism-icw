using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatBar : MonoBehaviour
{
    public Stat stat;
    public Image bar;
    private float finalValue;
    [SerializeField] private const float maxValue = 100f;

    void Awake()
    {
        if (GetComponent<Image>() == null)
        {
            Debug.LogError("StatBar requires an Image component.");
            gameObject.AddComponent<Image>();
        }
        else
        {
            bar = GetComponent<Image>();
        }
    }
    
    public void UpdateStat(float newValue)
    {
        finalValue = newValue / maxValue;
        bar.fillAmount = Mathf.Clamp01(finalValue);
    }

}