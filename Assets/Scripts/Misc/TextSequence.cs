using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSequence : MonoBehaviour {
    [System.Serializable]
    public struct Entry {
        public float time; 
        public string text;
    }

    public TextMeshProUGUI text;
    public Entry[] sequence;
    public bool playing = false;

    public void StartSequence() {
        if (!playing) {
            StartCoroutine(DoSequence());
        }
    }

    private IEnumerator DoSequence() {
        float time = 0;
        float max = sequence[sequence.Length - 1].time;
        int index = 0;

        while (max > time) {
            if (sequence[index].time <= time) {
                text.text = sequence[index].text;
                index++;
            }

            time += Time.deltaTime;
            playing = true;
            yield return null;
        }

        text.text = sequence[sequence.Length - 1].text;
        playing = false;
    }
}
