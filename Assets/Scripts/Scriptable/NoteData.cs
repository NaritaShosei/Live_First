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
    public float[] NotesTime { get => _notesTime; }
    [SerializeField] float[] _notesTime;
    [SerializeField] int _scoreLPB;
    [SerializeField] int _scoreBPM;
    [SerializeField] float _beatTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void MusicReading()
    {
        string inputString = Resources.Load<TextAsset>("Unite In The Sky (short)").ToString();
        InputJson inputJson = JsonUtility.FromJson<InputJson>(inputString);

        _notesTime = new float[inputJson.notes.Length];
        _scoreBPM = inputJson.BPM;
        _scoreLPB = inputJson.notes[0].LPB;
        _beatTime = 60f / _scoreBPM / _scoreLPB;
        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            _notesTime[i] = inputJson.notes[i].num * _beatTime;
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
