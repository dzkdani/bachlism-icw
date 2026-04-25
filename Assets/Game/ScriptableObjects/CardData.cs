using UnityEngine;

public enum Stat { environment, economy, trust, corruption }
public enum Category { coup, war, disaster, scandal, reform, diplomacy, culture, science, other }

[CreateAssetMenu(fileName = "CardData", menuName = "Data/Card")]
public class CardData : ScriptableObject
{
    public string id; 
    public int weight;
    public bool isCritical;
    public Category category;
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

