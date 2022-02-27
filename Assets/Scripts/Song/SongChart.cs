using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Song Chart", fileName = "SongChart")]
public class SongChart : ScriptableObject {
    [Min(0.1f)]
    public float speed;
    [Min(0)]
    public int order;
    public int totalPlayerNoteCount;

    public NoteTrack[] playerTracks;
    public NoteTrack[] opponentTracks;
}
