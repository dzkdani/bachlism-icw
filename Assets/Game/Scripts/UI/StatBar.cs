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
        if (bar == null)
        {
            Debug.LogError("StatBar: Bar Image component is not assigned.");
        }
    }
    
    public void UpdateStat(float newValue)
    {
        Debug.Log($"Updating {stat} bar: new value = {newValue}");
        finalValue = newValue / maxValue;
        bar.DOFillAmount(Mathf.Clamp01(finalValue), 0.5f).SetEase(Ease.InOutQuad);
    }

}