using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteSheetData : ScriptableObject {
    public Texture2D texture;
    public FrameCollection[] animations;

    public bool ContainsAnimation(string name) {
        return animations.Any(x => x.name == name);
    }

    public bool TryGetAnimation(string name, out FrameCollection animation) {
        try {
            animation = animations.First(x => x.name == name);
            return true;
        } catch {
            animation = default;
            return false;
        }
    }
}
