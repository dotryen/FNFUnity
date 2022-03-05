using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNF.Sprites {
    /// <summary>
    /// Custom sprite behaviour for Sparrow spritesheets
    /// </summary>
    public abstract class BaseSprite : MonoBehaviour {
        public SpriteSheetData data;

        protected virtual void Awake() {
            SetupSprite();
        }

        public abstract Color Color { get; set; }

        public abstract void SetupSprite();
        public abstract void SetFrame(SpriteFrame frame);
    }
}
