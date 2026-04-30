using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GoogleSheetLoader : MonoBehaviour
{
    [SerializeField] private string sheetUrl;

    private List<CardData> LoadedCards { get; set; }
    public CardDatabase RuntimeDatabase { get; private set; }

    [ContextMenu("Test Load Cards")]
    public void TestLoadCards()
    {
        StartCoroutine(LoadCards());
    }

    public IEnumerator LoadCards()
    {
        using var request = UnityWebRequest.Get(sheetUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        string csv = request.downloadHandler.text;

        LoadedCards = RuntimeCardParser.Parse(csv);

        RuntimeDatabase = ScriptableObject.CreateInstance<CardDatabase>();
        RuntimeDatabase.cards = LoadedCards;

        DebugCards();
    }

    private void DebugCards()
    {
        Debug.Log($"Total cards loaded: {LoadedCards.Count}");

        foreach (var card in LoadedCards)
        {
            Debug.Log(
                $"ID:{card.id} | Title:{card.title} | Weight:{card.weight}"
            );
        }
    }
}