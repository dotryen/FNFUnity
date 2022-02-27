using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Trackable {
    public double time; // in milliseconds
}

[Serializable]
public class Note : Trackable {
    public NoteType type = NoteType.Standard;

    public bool IsSustain => type != NoteType.Standard;

    public virtual void OnHit() {

    }

    public virtual void OnSustain() {

    }
}

[Serializable]
public class Event : Trackable {

}
