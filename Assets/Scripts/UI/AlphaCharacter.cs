using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlphaCharacter : RawImgSprite {
    public const string LETTERS = "abcdefghijklmnopqrstuvwxyz";
    public const string NUMBERS = "123456790";
    public const string SYMBOLS = "|~#$%()*+-:;<=>@[]^_.,'!?";

    public new SpriteAnimation animation;
    public int row;

    public void CreateBold(char letter) {
        animation.SetAnimation(char.ToUpper(letter) + " bold");
    }

    public void CreateLetter(char letter) {
        var kind = char.IsLower(letter) ? "lowercase" : "capital";
        animation.SetAnimation($"{letter} {kind}");

        var pos = new Vector2(rect.anchoredPosition.x, rect.sizeDelta.y - 110);
        pos.y -= row * 60;
        rect.anchoredPosition = pos;
    }

    public void CreateNumber(char number) {
        animation.SetAnimation(number.ToString());
    }

    public void CreateSymbol(char symbol) {
        switch (symbol) {
            case '.':
                animation.SetAnimation("period");
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 60);
                break;
            case '\'':
                animation.SetAnimation("apostraphie");
                break;
            case '?':
                animation.SetAnimation("question marl");
                break;
            case '!':
                animation.SetAnimation("exclamtion point");
                break;
            case '<':
                animation.SetAnimation("lChevron");
                break;
        }
    }
}
