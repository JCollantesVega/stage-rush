using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PaceNoteData", menuName = "Scriptable Objects/PaceNoteData")]
public class PaceNoteData : ScriptableObject
{
    public NoteEntry[] entries;

    [System.Serializable]
    public struct NoteEntry
    {
        public Direction direction;
        public int severity;
        public Sprite icon;
    }

    public Sprite GetSprite(Direction direction, int severity)
    {
        foreach(var entry in entries)
        {
            if(entry.direction == direction && entry.severity == severity)
            {
                return entry.icon;
            }
        }
        return null;
    }
}
