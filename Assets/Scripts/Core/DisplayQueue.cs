using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNF.Core {
    public class DisplayQueue {
        private Queue<Note>[] notes;
        private Queue<int>[] ids;

        public DisplayQueue() {
            notes = new Queue<Note>[4];
            ids = new Queue<int>[4];

            for (int i = 0; i < 4; i++) {
                notes[i] = new Queue<Note>();
                ids[i] = new Queue<int>();
            }
        }

        public void Enqueue(int track, Note note, int id) {
            notes[track].Enqueue(note);
            ids[track].Enqueue(id);
        }

        public void Dequeue(int track, out Note note, out int id) {
            note = null;
            id = -1;

            if (Count(track) == 0) return;

            note = notes[track].Dequeue();
            id = ids[track].Dequeue();
        }

        public void Peek(int track, out Note note, out int id) {
            note = null;
            id = -1;

            if (Count(track) == 0) return;

            note = notes[track].Peek();
            id = ids[track].Peek();
        }

        public int Count(int track) {
            return notes[track].Count;
        }
    }
}
