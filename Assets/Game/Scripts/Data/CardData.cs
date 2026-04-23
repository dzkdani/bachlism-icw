using UnityEngine;

public enum Stat { environment, economy, trust, corruption }

[CreateAssetMenu(fileName = "CardData", menuName = "Data/Card")]
public class CardData : ScriptableObject
{
    public string id; 
    public int weight;
    public string title;
    [TextArea(3, 10)]
    public string desc;
    public Decision left;
    public Decision right;
}

[System.Serializable]
public struct Decision
{
    [TextArea(3, 10)]
    public string desc;
    public Effect[] effects;
}

[System.Serializable]
public struct Effect
{
    public Stat stat;
    public float amount; 
}

