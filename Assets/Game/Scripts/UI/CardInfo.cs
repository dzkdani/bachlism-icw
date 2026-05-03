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

        /// <summary>
        /// Aggregates the card's title, description, and decisions into a single string for TTS.
        /// </summary>
        public string GetAccessibilityText()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            if (titleText != null && !string.IsNullOrEmpty(titleText.text))
                sb.Append("Title: ").Append(titleText.text).Append(". ");
                
            if (descText != null && !string.IsNullOrEmpty(descText.text))
                sb.Append("Description: ").Append(descText.text).Append(". ");
                
            // Decision is likely a struct, so we only check if the text is not empty
            if (!string.IsNullOrEmpty(leftDecision.desc))
                sb.Append("Left Option: ").Append(leftDecision.desc).Append(". ");
                
            if (!string.IsNullOrEmpty(rightDecision.desc))
                sb.Append("Right Option: ").Append(rightDecision.desc).Append(". ");
                
            return sb.ToString();
        }
    }