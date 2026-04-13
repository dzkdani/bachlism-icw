using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Data/Card")]
public class CardData : ScriptableObject
{
    public string id;
    public string desc;

    public Choice leftChoice;
    public Choice rightChoice;

}

[System.Serializable]
public class Choice
{
    public string desc;
    public Effect[] effect;
}

[System.Serializable]
public class Effect
{
    public StatType stat;
    public float amount;
}
