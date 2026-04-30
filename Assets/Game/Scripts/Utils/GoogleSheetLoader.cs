using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GoogleSheetLoader : MonoBehaviour
{
    [SerializeField] private string sheetUrl;

    public List<CardData> LoadedCards { get; private set; }

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

        Debug.Log($"Loaded {LoadedCards.Count} cards");
    }
}