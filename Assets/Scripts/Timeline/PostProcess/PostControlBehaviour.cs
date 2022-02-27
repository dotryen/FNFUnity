using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class PostControlBehaviour : PlayableBehaviour {
    public float bloomIntensity;
    [Range(0, 1)]
    public float chromaticAbberation;
    [Range(0, 360)]
    public float hueShift;
    [Range(-100, 100)]
    public float saturation;
}
