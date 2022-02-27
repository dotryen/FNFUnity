using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class Track<T> where T : Trackable {
    public T[] shit;

    public Track() {
        shit = null;
    }

    public Track(IEnumerable<T> stuff) {
        shit = stuff.ToArray();
    }
}

/// <summary>
/// Wrapper for unity serialization
/// </summary>
[Serializable]
public class NoteTrack : Track<Note> {
    public NoteTrack() : base() {

    }

    public NoteTrack(IEnumerable<Note> stuff) : base(stuff) {

    }
}

/// <summary>
/// Wrapper for unity serialization
/// </summary>
[Serializable]
public class EventTrack : Track<Event> {
    public EventTrack() : base() {

    }

    public EventTrack(IEnumerable<Event> stuff) : base(stuff) {

    }
}
