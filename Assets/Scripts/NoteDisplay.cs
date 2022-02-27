using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class NoteDisplay : MonoBehaviour {
    public RectTransform strumLine;
    public GameObject templateNote;
    public SpriteAnimation[] staticArrows;
    [Space]
    public int poolCount = 50;
    public RectTransform standardPool;
    public RectTransform sustainPool;

    private ConcurrentBag<UINote> notePool;
    private List<int> noteState;
    private int index;

    private RectTransform[] staticCache;

    public bool Initialized { get; private set; }

    private void Start() {
        notePool = new ConcurrentBag<UINote>();
        noteState = new List<int>();
        index = 0;

        templateNote.GetComponent<UINote>().Setup();
        templateNote.SetActive(false);

        for (int i = 0; i < poolCount; i++) {
            var go = Instantiate(templateNote, standardPool);
            notePool.Add(go.GetComponent<UINote>());
        }

        staticCache = new RectTransform[] { 
            ((RawImgSprite)staticArrows[0].sprite).RectTransform,
            ((RawImgSprite)staticArrows[1].sprite).RectTransform,
            ((RawImgSprite)staticArrows[2].sprite).RectTransform,
            ((RawImgSprite)staticArrows[3].sprite).RectTransform
        };

        Initialized = true;
    }

    public int StartNote(SideColor side, NoteType type, float time) {
        if (!notePool.TryTake(out var note)) {
            Debug.LogError("Failed to get note from pool!");
            return -1;
        }

        var noteId = index;
        index++;
        noteState.Add(noteId);

        var trans = staticCache[(int)side];
        var pos = trans.anchoredPosition;
        pos.y = -720;

        note.rect.anchoredPosition = pos;
        note.PrepareNote(side, type);

        if (type != NoteType.Standard) note.transform.SetParent(sustainPool);
        else note.transform.SetParent(standardPool);
        note.gameObject.SetActive(true);

        StartCoroutine(NoteBehavior(noteId, time, note));
        return noteId;
    }

    public void StopNote(int id) {
        noteState.Remove(id);
    }

    public void StopAllNotes() {
        noteState.Clear();
    }

    public void StopNoteDelayed(int id, float delay) {
        StartCoroutine(DelayStop(id, delay));
    }

    IEnumerator DelayStop(int id, float time) {
        yield return new WaitForSeconds(time);
        StopNote(id);
    }

    IEnumerator NoteBehavior(int id, float time, UINote note) {
        while (true) {
            if (!noteState.Contains(id)) break;

            var pos = note.rect.anchoredPosition;

            pos.y = ((float)GameplayVars.CurrentTimeMS - time) * GameplayVars.NoteSpeed;

            note.rect.anchoredPosition = pos;

            yield return null;
        }

        note.gameObject.SetActive(false);
        notePool.Add(note);
    }
}
