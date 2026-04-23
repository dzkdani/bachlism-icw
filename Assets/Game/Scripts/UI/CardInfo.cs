using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    public Text titleText;
    public Text descText;
    
    private CardData cardData;
    public CardData CardData => cardData;
    
    private string id;
    private int weight;

    [SerializeField] private Decision leftDecision;
    public Decision Left => leftDecision;
    [SerializeField] private Decision rightDecision;
    public Decision Right => rightDecision;

    public void Initialize(CardData data)
    {
        cardData = data;
        id = cardData.id;
        weight = cardData.weight;
        titleText.text = cardData.title;
        descText.text = cardData.desc;
        leftDecision = cardData.left;
        rightDecision = cardData.right;
    }
}
