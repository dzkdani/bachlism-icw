using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public static class CardImporter
{
    private const string CsvPath = "Assets/Data/cards.csv";
    private const string OutputPath = "Assets/Data/Cards";
    private const string DbPath = "Assets/Data/CardDatabase.asset";

    [MenuItem("Tools/Import Cards")]
    public static void ImportCards()
    {
        if (!Directory.Exists(OutputPath))
            Directory.CreateDirectory(OutputPath);

        var lines = File.ReadAllLines(CsvPath);

        foreach (var line in lines.Skip(1)) // skip header
        {
            var cols = ParseCsvLine(line);

            CardData card = ScriptableObject.CreateInstance<CardData>();

            card.id = cols[0];
            card.weight = int.Parse(cols[1]);
            card.title = cols[2];
            card.desc = cols[3];

            card.left = new Decision
            {
                desc = cols[4],
                effects = ParseEffects(cols[5])
            };

            card.right = new Decision
            {
                desc = cols[6],
                effects = ParseEffects(cols[7])
            };

            string assetPath = $"{OutputPath}/{card.id}.asset";

            var existing = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);

            if (existing == null)
            {
                AssetDatabase.CreateAsset(card, assetPath);
            }
            else
            {
                EditorUtility.CopySerialized(card, existing);
                Object.DestroyImmediate(card);
            }
        }

        CardDatabase database =
        AssetDatabase.LoadAssetAtPath<CardDatabase>(DbPath);

        if (database == null)
        {
            database = ScriptableObject.CreateInstance<CardDatabase>();
            AssetDatabase.CreateAsset(database, DbPath);
        }

        database.cards = AssetDatabase
            .FindAssets("t:CardData", new[] { OutputPath })
            .Select(guid =>
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.LoadAssetAtPath<CardData>(path);
            })
            .ToList();

        EditorUtility.SetDirty(database);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Card import complete.");
    }

    static Effect[] ParseEffects(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return System.Array.Empty<Effect>();

        var effects = new List<Effect>();

        foreach (var entry in raw.Split(';'))
        {
            var pair = entry.Split(':');

            if (pair.Length != 2)
            {
                Debug.LogWarning($"Invalid effect entry: {entry}");
                continue;
            }

            if (!System.Enum.TryParse(pair[0], true, out Stat stat))
            {
                Debug.LogWarning($"Invalid stat: {pair[0]}");
                continue;
            }

            if (!float.TryParse(
                pair[1],
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out float amount))
            {
                Debug.LogWarning($"Invalid amount: {pair[1]}");
                continue;
            }

            effects.Add(new Effect
            {
                stat = stat,
                amount = amount
            });
        }

        return effects.ToArray();
    }

    static string[] ParseCsvLine(string line)
    {
        return line.Split(',');
    }
}