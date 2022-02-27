using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Song Metadata", fileName = "SongMeta")]
public class SongMetadata : ScriptableObject {
    public string songName;
    public float length;
    public int bpm;
    public string[] difficulties;
}
