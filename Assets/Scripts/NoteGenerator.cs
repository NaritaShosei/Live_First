using System;
using UnityEngine;

public class NoteGenerator : MonoBehaviour
{
    [Serializable]
    public class InputJson
    {
        public Notes[] notes;
        public int BPM;
    }
    [Serializable]
    public class Notes
    {
        public int num;
        public int block;
        public int LPB;
    }

    int[] _scoreNum;
    int[] _scoreBlock;
    int   BPM;
    int   LPB;
    private void Awake()
    {
        MusicReading();
    }

    void MusicReading()
    {
        string inputString = Resources.Load<TextAsset>("Unite In The Sky (short)").ToString();
        InputJson inputJson = JsonUtility.FromJson<InputJson>(inputString);

        _scoreNum = new int[inputJson.notes.Length];
        _scoreBlock = new int[inputJson.notes.Length];
        BPM = inputJson.BPM;
        LPB = inputJson.notes[0].LPB;

        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            _scoreNum[i] = inputJson.notes[i].num;

            _scoreBlock[i] = inputJson.notes[i] .block;
        }
    }
}
