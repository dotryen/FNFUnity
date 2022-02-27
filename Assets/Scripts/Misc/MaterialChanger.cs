using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour {
    [System.Serializable]
    public class Scheme {
        public Material material;
        public string property = "_Color";
        public Color[] colors;
    }

    public bool resetSchemesOnStart;
    public Scheme[] schemes;

    private void Start() {
        if (resetSchemesOnStart) {
            SetScheme(0);
        }
    }

    public void SetScheme(int number) {
        foreach (var scheme in schemes) {
            scheme.material.SetColor(scheme.property, scheme.colors[number]);
        }
    }
}
