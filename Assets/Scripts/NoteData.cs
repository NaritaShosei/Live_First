using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "GameData/NoteData", fileName = "NoteData")]
public class NoteData : ScriptableObject
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
        public int LPB;
    }
    public int[] ScoreNum { get => _scoreNum; }
    [SerializeField] int[] _scoreNum;
    public int BPM { get => bpm; }
    [SerializeField] int bpm;
    public int LPB { get => lpb; }
    [SerializeField] int lpb;
    private void Awake()
    {
        MusicReading();
    }

    void MusicReading()
    {
        string inputString = Resources.Load<TextAsset>("Unite In The Sky (short)").ToString();
        InputJson inputJson = JsonUtility.FromJson<InputJson>(inputString);

        _scoreNum = new int[inputJson.notes.Length];
        bpm = inputJson.BPM;
        lpb = inputJson.notes[0].LPB;

        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            _scoreNum[i] = inputJson.notes[i].num;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(NoteData))]
    public class NoteDataLoad : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(10);
            NoteData noteData = (NoteData)target;

            if (GUILayout.Button("Note Data Loading"))
            {
                noteData.MusicReading();
            }
        }
    }
#endif
}
