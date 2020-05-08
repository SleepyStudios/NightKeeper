using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardEntryText : MonoBehaviour {
    public new TextMeshProUGUI rank, name, score, date;
    public Color mineColour;

    public void AddEntry(LeaderboardEntry entry, bool isMine, int index) {
        rank.text = $"#{index}";
        if (isMine) rank.color = mineColour;

        name.text = $"{entry.name}";
        if (isMine) name.color = mineColour;

        score.text = entry.score.ToString();
        if (isMine) score.color = mineColour;

        date.text = entry.date.ToString("dd MMM yyyy");
        if (isMine) date.color = mineColour;
    }
}
