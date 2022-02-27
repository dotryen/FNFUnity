using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Animation Offsets", fileName = "Offsets")]
public class Offsets : ScriptableObject {
    public Offset[] offsets;

    public Offset GetOffset(string name) {
        return offsets.First(x => x.name == name);
    }
}

[System.Serializable]
public struct Offset {
    public string name;
    public Vector2 value;
}
