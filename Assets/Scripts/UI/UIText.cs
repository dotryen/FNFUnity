using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIText : UIBehaviour {
    public AlphaCharacter template;
    public RectTransform container;
    [Space]
    public string defaultText;
    public bool bold;

    private List<AlphaCharacter> characters;

    protected override void Awake() {
        base.Awake();
        characters = new List<AlphaCharacter>();

        if (!string.IsNullOrEmpty(defaultText)) {
            CreateText(defaultText);
        }
    }

    public void CreateText(string text) {
        float xPos = 0;
        float width = 0;
        float height = 0;
        AlphaCharacter last = null;

        var lengthDiff = characters.Count - text.Length;
        if (lengthDiff < 0) {
            for (int i = lengthDiff; i != 0; i++) {
                var obj = Instantiate(template, container);
                characters.Add(obj);
            }
        } else if (lengthDiff > 0) {
            for (int i = lengthDiff - 1; i >= 0; i--) {
                var obj = characters[text.Length + i];
                characters.RemoveAt(text.Length + i);
                Destroy(obj.gameObject);
            }
        }

        for (int i = 0; i < text.Length; i++) {
            var character = text[i];
            bool didShit = true;

            if (last != null) {
                xPos = last.rect.anchoredPosition.x + last.rect.sizeDelta.x;
            }

            AlphaCharacter letter = characters[i];
            letter.SetupSprite();
            letter.rect.anchoredPosition = new Vector2(xPos, 0);

            if (char.IsWhiteSpace(character) || character == '-') {
                letter.RectTransform.sizeDelta = new Vector2(40, 1);
                letter.gameObject.SetActive(false);
            } else if (AlphaCharacter.LETTERS.IndexOf(char.ToLower(character)) != -1) {
                if (bold) letter.CreateBold(character);
                else letter.CreateLetter(character);

                letter.gameObject.SetActive(true);
            } else {
                didShit = false;
            }

            if (didShit) {
                width += letter.rect.sizeDelta.x;
                height = Mathf.Max(letter.rect.sizeDelta.y, height);
                last = letter;
            }
        }

        transform.sizeDelta = new Vector2(width, height);
    }
}
