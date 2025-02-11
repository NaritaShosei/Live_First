using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    const string dataName = "ScoreData";
    static int Score = 0;
    static List<int> ScoreList = null;
    [SerializeField]
    List<TextMeshProUGUI> _scoreTexts = new();
    [SerializeField]
    TextMeshProUGUI _scoreText;
    void Start()
    {
        if (ScoreList == null)
        {
            Load();
        }
        var list = ScoreList.OrderByDescending(x => x).Take(5).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            _scoreTexts[i].text = $"<size=50>{i + 1}</size>. {list[i].ToString("000000")}";
        }
        _scoreText.text = Score.ToString();
        Save();
    }

    void Save()
    {
        SaveData saveData = new SaveData()
        {
            ScoreDataList = ScoreList
        };
        string data = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(dataName, data);
    }
    void Load()
    {
        string json = PlayerPrefs.GetString(dataName, "");
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        if (string.IsNullOrEmpty(json))
        {
            ScoreList = new();
        }
        else
        {
            ScoreList = saveData.ScoreDataList;
        }
    }

    public void RankingReset()
    {
        ScoreList.Clear();
        for (int i = 0; i < _scoreTexts.Count; i++)
        {
            _scoreTexts[i].text = $"<size=50>{i + 1}</size>. 000000";
        }
        Save();
    }
    public static void AddScore(int score)
    {
        Score = score;
        ScoreList.Add(score);
    }

    [Serializable]
    class SaveData
    {
        public List<int> ScoreDataList;
    }
}
