using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNF.Characters {
    using Sprites;

    [RequireComponent(typeof(SpriteAnimation))]
    public class SpriteSinger : BaseSinger {
        [Header("Sprite fields")]
        public new SpriteAnimation animation;

        private void Awake() {
            animation = GetComponent<SpriteAnimation>();
        }

        public override void GetAnimLength(string name, out int frameCount, out float fps) {
            var has = animation.sprite.data.TryGetAnimation(name, out var anim);

            if (has) {
                frameCount = anim.FrameCount;
                fps = animation.frameRate;
            } else {
                frameCount = 0;
                fps = 0;
            }
        }

        public override void SetAnimation(string name, int frame, float speed, bool loop = false) {
            animation.speed = speed;
            animation.SetAnimation(name, loop, frame);
            animation.update = true;
        }
    }
}
