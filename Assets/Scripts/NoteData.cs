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
    }
    [Serializable]
    public class Notes
    {
        public int num;
    }
    public int[] ScoreNum { get => _scoreNum; }
    [SerializeField] int[] _scoreNum;
    private void Awake()
    {
        MusicReading();
    }

    void MusicReading()
    {
        string inputString = Resources.Load<TextAsset>("Unite In The Sky (short)").ToString();
        InputJson inputJson = JsonUtility.FromJson<InputJson>(inputString);

        _scoreNum = new int[inputJson.notes.Length];

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
