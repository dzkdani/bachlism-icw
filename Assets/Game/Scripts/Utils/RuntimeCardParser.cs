using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public static class RuntimeCardParser
{
    private const int ExpectedColumnCount = 10;

    public static List<CardData> Parse(string csvText)
    {
        var cards = new List<CardData>();
        var lines = csvText.Split('\n');

        int rowNumber = 0;

        foreach (var line in lines.Skip(1))
        {
            rowNumber++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var cols = ParseCsvLine(line);

            if (cols.Length != ExpectedColumnCount)
            {
                Debug.LogError($"Row {rowNumber}: invalid column count");
                continue;
            }

            if (ValidateAndParseRow(cols, rowNumber, out CardData card))
            {
                cards.Add(card);
            }
        }

        return cards;
    }

    static bool ValidateAndParseRow(string[] cols, int rowNumber, out CardData card)
    {
        card = ScriptableObject.CreateInstance<CardData>();

        if (!int.TryParse(cols[1], out int weight))
            return false;

        bool.TryParse(cols[2], out bool isCritical);

        card.id = cols[0].Trim();
        card.weight = weight;
        card.isCritical = isCritical;
        card.category = cols[3].Trim();
        card.title = cols[4].Trim();
        card.desc = cols[5].Trim();

        card.left = new Decision
        {
            desc = cols[6].Trim(),
            effects = ParseEffects(cols[7])
        };

        card.right = new Decision
        {
            desc = cols[8].Trim(),
            effects = ParseEffects(cols[9])
        };

        return true;
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
                continue;

            if (!System.Enum.TryParse(pair[0], true, out Stat stat))
                continue;

            if (!float.TryParse(
                pair[1],
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out float amount))
                continue;

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
                columns.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        columns.Add(current.ToString().Trim());

        return columns.ToArray();
    }
}