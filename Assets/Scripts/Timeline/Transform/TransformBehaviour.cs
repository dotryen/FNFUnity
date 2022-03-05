using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[System.Serializable]
public class TransformBehaviour : PlayableBehaviour {
    public bool changePosition;
    public Vector3 position;
    [Space]
    public bool changeRotation;
    public Vector3 rotation;
    public bool slerp;
    [Space]
    public bool changeScale;
    public Vector3 scale;
    [Space]
    public bool forceFps;
    public int fps;
}
