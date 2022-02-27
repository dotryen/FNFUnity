using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key {
    private readonly KeyCode[] codes;

    public bool Pressed { get; private set; }
    public bool HeldDown { get; private set; }
    public bool WasDown { get; private set; }

    public Key(params KeyCode[] keys) {
        codes = keys;
    }

    public void Update() {
        foreach (var code in codes) {
            Pressed = Input.GetKeyDown(code);
            WasDown = Pressed;
            if (Pressed) break;
        }

        foreach (var code in codes) {
            HeldDown = Input.GetKey(code);
            if (HeldDown) break;
        }
    }
}
