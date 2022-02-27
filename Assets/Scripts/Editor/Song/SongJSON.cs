using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongJSON {
    [System.Serializable]
    public struct Root {
        public Song song;
    }

    [System.Serializable]
    public struct Song {
        public string song;
        public Section[] notes;
        public float bpm;
        public bool needsVoices;
        public float speed;

        public string player1;
        public string player2;
        public bool validScore;
    }

    [System.Serializable]
    public struct Section {
        public double[][] sectionNotes;
        public int lengthInSteps;
        public int typeOfSection;
        public bool mustHitSection;
        public float bpm;
        public bool changeBPM;
        public bool altAnim;
    }
}
