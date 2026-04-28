using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public static class CardImporter
{
    private const string CsvPath = "Assets/Data/cards.csv";
    private const string OutputPath = "Assets/Data/Cards";
    private const string DbPath = "Assets/Data/CardDatabase.asset";
    private const int ExpectedColumnCount = 10;

    [MenuItem("Tools/Import Cards")]
    public static void ImportCards()
    {
        if (!Directory.Exists(OutputPath))
            Directory.CreateDirectory(OutputPath);

        var lines = File.ReadAllLines(CsvPath);
        int rowNumber = 0;
        int successCount = 0;
        int errorCount = 0;

        foreach (var line in lines.Skip(1)) // skip header
        {
            rowNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                Debug.LogWarning($"Row {rowNumber}: Empty line, skipping.");
                errorCount++;
                continue;
            }

            var cols = ParseCsvLine(line);

            if (cols.Length != ExpectedColumnCount)
            {
                Debug.LogError($"Row {rowNumber}: Expected {ExpectedColumnCount} columns, got {cols.Length}. Skipping.");
                errorCount++;
                continue;
            }

            if (!ValidateAndParseRow(cols, rowNumber, out CardData card))
            {
                errorCount++;
                continue;
            }

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

            successCount++;
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

        Debug.Log($"Card import complete. Imported: {successCount}, Errors: {errorCount}");
    }

    static bool ValidateAndParseRow(string[] cols, int rowNumber, out CardData card)
    {
        card = null;

        // cols[0]=id, cols[1]=weight, cols[2]=isCritical, cols[3]=category, 
        // cols[4]=title, cols[5]=desc, cols[6]=leftDesc, cols[7]=leftEffects, 
        // cols[8]=rightDesc, cols[9]=rightEffects

        if (string.IsNullOrWhiteSpace(cols[0]))
        {
            Debug.LogError($"Row {rowNumber}: Card ID is empty. Skipping.");
            return false;
        }

        if (!int.TryParse(cols[1], out int weight))
        {
            Debug.LogError($"Row {rowNumber} (ID: {cols[0]}): Invalid weight '{cols[1]}'. Skipping.");
            return false;
        }

        if (!bool.TryParse(cols[2], out bool isCritical))
        {
            Debug.LogWarning($"Row {rowNumber} (ID: {cols[0]}): Invalid isCritical '{cols[2]}'. Defaulting to false.");
            isCritical = false;
        }

        if (string.IsNullOrWhiteSpace(cols[3]))
        {
            Debug.LogError($"Row {rowNumber} (ID: {cols[0]}): Category is empty. Skipping.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(cols[4]))
        {
            Debug.LogError($"Row {rowNumber} (ID: {cols[0]}): Title is empty. Skipping.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(cols[5]))
        {
            Debug.LogError($"Row {rowNumber} (ID: {cols[0]}): Description is empty. Skipping.");
            return false;
        }

        card = ScriptableObject.CreateInstance<CardData>();
        card.id = cols[0].Trim();
        card.weight = weight;
        card.isCritical = isCritical;
        card.category = cols[3].Trim();
        card.title = cols[4].Trim();
        card.desc = cols[5].Trim();

        card.left = new Decision
        {
            desc = cols[6].Trim(),
            effects = ParseEffects(cols[7], cols[0], rowNumber, "left")
        };

        card.right = new Decision
        {
            desc = cols[8].Trim(),
            effects = ParseEffects(cols[9], cols[0], rowNumber, "right")
        };

        return true;
    }

    static Effect[] ParseEffects(string raw, string cardId, int rowNumber, string side)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return System.Array.Empty<Effect>();

        var effects = new List<Effect>();

        foreach (var entry in raw.Split(';'))
        {
            if (string.IsNullOrWhiteSpace(entry))
                continue;

            var pair = entry.Split(':');

            if (pair.Length != 2)
            {
                Debug.LogWarning($"Row {rowNumber} (ID: {cardId}, {side}): Invalid effect entry '{entry}'.");
                continue;
            }

            if (!System.Enum.TryParse(pair[0].Trim(), true, out Stat stat))
            {
                Debug.LogWarning($"Row {rowNumber} (ID: {cardId}, {side}): Invalid stat '{pair[0]}'.");
                continue;
            }

            if (!float.TryParse(
                pair[1].Trim(),
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out float amount))
            {
                Debug.LogWarning($"Row {rowNumber} (ID: {cardId}, {side}): Invalid amount '{pair[1]}'.");
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
        var columns = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                columns.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        columns.Add(current.ToString());
        return columns.ToArray();
    }
}