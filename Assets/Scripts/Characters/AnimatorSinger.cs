using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNF.Characters {
    public class AnimatorSinger : BaseSinger {
        [Header("Animation")]
        [Tooltip("Should be the controller provided in the animations folder.")]
        public AnimatorOverrideController controller;
        public Animator animator;
        [Space]
        public bool forceFps;
        public int fps = 12;

        private float threshold;
        private float timer;

        // key is fps, value is frames
        private Dictionary<string, KeyValuePair<float, int>> animationCache;

        private void Awake() {
            animator.runtimeAnimatorController = controller;

            animationCache = new Dictionary<string, KeyValuePair<float, int>>(controller.overridesCount);
            List<KeyValuePair<AnimationClip, AnimationClip>> pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>(controller.overridesCount);

            controller.GetOverrides(pairs);
            foreach (var pair in pairs) {
                if (pair.Value == null) continue;

                float fps = pair.Value.frameRate;
                int frameCount = Mathf.FloorToInt(pair.Value.length / (1f / fps));

                animationCache.Add(pair.Key.name, new KeyValuePair<float, int>(fps, frameCount));
            }
        }

        protected override void Update() {
            base.Update();

            if (forceFps) {
                animator.enabled = false;

                if (threshold <= timer) {
                    animator.Update(threshold);
                    timer -= threshold;
                }

                timer += Time.deltaTime;
            } else {
                animator.enabled = true;
                timer = 0f;
            }
        }

        public override void SetAnimation(string name, int frame, float speed, bool loop = false) {
            var pair = animationCache[name];
            animator.Play(name, -1, (float)frame / (float)pair.Value);
            animator.speed = speed;
            animator.SetBool("Loop", loop);

            threshold = 1f / fps;
            timer = 0f;
        }

        public override void GetAnimLength(string name, out int frameCount, out float fps) {
            var pair = animationCache[name];
            fps = pair.Key;
            frameCount = pair.Value;
        }
    }
}
