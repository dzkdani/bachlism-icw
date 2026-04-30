using System;
using UnityEngine;
using TMPro;

public class CardInfo : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;

    private CardData cardData;

    private string defaultDesc;

    [SerializeField] private Decision leftDecision;
    public Decision Left => leftDecision;

    [SerializeField] private Decision rightDecision;
    public Decision Right => rightDecision;

    public void Initialize(CardData data)
    {
        cardData = data;

        // titleText.text = cardData.title;
        titleText.PlayTypewriter(cardData.title, 30f);

        defaultDesc = cardData.desc;
        // descText.PlayTypewriter(defaultDesc, 30f);
        descText.PlayTypewriterWithMark(defaultDesc, 30f);

        leftDecision = cardData.left;
        rightDecision = cardData.right;
    }

    public void ShowDefaultText()
    {
        // titleText.text = cardData.title;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.PlayTypewriter(cardData.title, 30f);
    }

    public void ShowLeftDecision()
    {
        // titleText.text = leftDecision.desc;
        titleText.alignment = TextAlignmentOptions.Right;
        titleText.PlayTypewriter(leftDecision.desc, 30f);
    }

    public void ShowRightDecision()
    {
        // titleText.text = rightDecision.desc;
        titleText.alignment = TextAlignmentOptions.Left;
        titleText.PlayTypewriter(rightDecision.desc, 30f);
    }
}