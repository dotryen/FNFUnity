using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextColorChanger : MonoBehaviour {
    public Color[] colors;

    private TMP_Text text;

    private void Start () {
        text = GetComponent<TMP_Text>();
    }

    public void SetColor(int index) {
        text.color = colors[index];
    }
}
