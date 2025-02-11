using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    static List<int> ScoreList = new();
    [SerializeField]
    List<TextMeshProUGUI> _texts = new();
    void Start()
    {
        var list = ScoreList.OrderByDescending(x => x).Take(5).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            _texts[i].text = $"<size=100>{i + 1}</size>. {list[i].ToString("000000")}";
        }
    }
    public static void AddScore(int score)
    {
        ScoreList.Add(score);
    }
}
