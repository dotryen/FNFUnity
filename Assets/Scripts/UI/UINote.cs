using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FNF.Core {
    using Sprites;

    public class UINote : RawImgSprite {
        public NoteType currentType;

        protected override void Awake() {

        }

        public void Setup() {
            SetupSprite();
        }

        public void PrepareNote(SideColor side, NoteType type) {
            currentType = type;

            if (type == NoteType.Standard) {
                var noteFrame = data.animations.First(x => x.name == System.Enum.GetName(typeof(SideColor), side)).frames[0];
                SetFrame(noteFrame);

                image.color = Color.white;
                rect.sizeDelta = new Vector2(110, 110);
            } else {
                var size = new Vector2(35, 35);
                image.color = new Color(1, 1, 1, 0.6f);

                if (type == NoteType.Sustain) {
                    size.y = (float)Conductor.StepCrochet * GameplayVars.NoteSpeed;
                    var susFrame = data.animations.First(x => x.name == System.Enum.GetName(typeof(SideColor), side) + "HoldPiece").frames[0];
                    SetFrame(susFrame);
                } else {
                    var capFrame = data.animations.First(x => x.name == System.Enum.GetName(typeof(SideColor), side) + "HoldEnd").frames[0];
                    SetFrame(capFrame);
                }

                rect.sizeDelta = size;
            }
        }
    }
}
