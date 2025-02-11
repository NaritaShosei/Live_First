using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    const string dataName = "ScoreData";
    static int Score = 0;
    static int Combo = 0;
    static List<int> ScoreList = null;
    [SerializeField]
    List<TextMeshProUGUI> _scoreTexts = new();
    [SerializeField]
    TextMeshProUGUI _scoreText;
    [SerializeField]
    List<Sprite> _spriteList;
    [SerializeField]
    Image _yuko;
    void Start()
    {
        if (ScoreList == null)
        {
            Load();
        }
        //スコアの表示処理
        var list = ScoreList.OrderByDescending(x => x).Take(5).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            _scoreTexts[i].text = $"<size=50>{i + 1}</size>. {list[i].ToString("000000")}";
        }
        _scoreText.text = Score.ToString();

        ChangeSprite();

        Save();
    }
    void ChangeSprite()
    {
        Sprite sprite = null;
        if (Score == 556491 && Combo == 114)//最高得点獲得時
        {
            sprite = _spriteList[2];
        }
        else if (Score != 556491 && Combo == 114)
        {
            sprite = _spriteList[1];
        }
        else if (Score > 200000 && Combo != 114)
        {
            sprite = _spriteList[0];
        }
        else
        {
            var random = UnityEngine.Random.Range(3, 6);
            sprite = _spriteList[random];
        }
        _yuko.sprite = sprite;
    }
    static void Save()
    {
        SaveData saveData = new SaveData()
        {
            ScoreDataList = ScoreList
        };
        string data = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(dataName, data);
    }
    static void Load()
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
    public static void AddScore(int score, int combo)
    {
        if (ScoreList == null)
        {
            Load();
        }
        Combo = combo;
        Score = score;
        ScoreList.Add(score);
    }

    [Serializable]
    class SaveData
    {
        public List<int> ScoreDataList;
    }
}
