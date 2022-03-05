using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FNF.Sprites {
    [DisallowMultipleComponent]
    public class SpriteAnimation : MonoBehaviour {
        public BaseSprite sprite;
        public Offsets offsets;
        [Space]
        public bool update = true;
        public bool playOnAwake = true;
        public string defaultAnimation;
        [Space]
        public bool loop = true;
        public int frameRate = 24;
        public float speed = 1f;

        private float threshold;
        private float time;
        private int frame;
        private FrameCollection currentAnimation;
        private bool animEndedOrLooped;
        private Coroutine returnRoutine;

        private Vector2 currentOffset;

        public bool AnimationEndOrLoop => animEndedOrLooped;
        public float Threshold => threshold;
        public int CurrentFrame => frame;
        public float CurrentTime => time;
        public FrameCollection CurrentAnimation => currentAnimation;
        public float AnimationTime => currentAnimation.FrameCount * Threshold;

        protected void Start() {
            if (playOnAwake) SetAnimation(defaultAnimation);
        }

        protected virtual void Update() {
            if (!update) return;
            if (!loop && animEndedOrLooped) return;

            animEndedOrLooped = false;
            time += Time.deltaTime * speed;
            if (time < threshold) return;

            sprite.SetFrame(currentAnimation.frames[frame]);

            if (frame == currentAnimation.FrameCount - 1) {
                if (loop) frame = 0;
                animEndedOrLooped = true;
            } else {
                frame++;
            }

            time -= threshold;
        }

        public void PlayOneShot(string name, bool loop = true) {
            if (returnRoutine != null) StopCoroutine(returnRoutine);
            var oldLoop = loop;

            SetAnimation(name, loop);
            returnRoutine = StartCoroutine(DelayedAnim(defaultAnimation, oldLoop));
        }

        public void StateMachine(params string[] states) {
            StateMachine(true, states);
        }

        public void StateMachine(bool loop, params string[] states) {
            StartCoroutine(StateMachineRoutine(states, loop));
        }

        public virtual void SetAnimation(string name, bool loop = true, int frame = 0) {
            this.frame = frame;
            time = 0;
            threshold = 1f / frameRate;
            update = true;
            this.loop = loop;
            animEndedOrLooped = false;

            returnRoutine = null;

            try {
                currentAnimation = sprite.data.animations.First(x => x.name == name);
                sprite.SetFrame(currentAnimation.frames[0]);

                // stop causing null reference exceptions you cun t!!!!!!
                if (offsets) {
                    if (offsets.offsets.Any(x => x.name == name)) {
                        var offset = offsets.GetOffset(name);
                        transform.localPosition = (transform.localPosition - (Vector3)currentOffset) + (Vector3)offset.value;
                        currentOffset = offset.value;
                    }
                }
            } catch {
                SetEmptyAnim();
                Debug.LogWarning($"Could not play animation \"{name}\"");
            }
        }

        private void SetEmptyAnim() {
            currentAnimation = default;
            currentAnimation.frames = new SpriteFrame[1];
            currentAnimation.name = "empty";

            currentAnimation.frames[0] = default;
        }

        IEnumerator StateMachineRoutine(string[] states, bool loop) {
            for (int i = 0; i < states.Length - 1; i++) {
                SetAnimation(states[i], false);
                yield return new WaitUntil(() => animEndedOrLooped);
            }
            SetAnimation(states[states.Length - 1], loop);
        }

        IEnumerator DelayedAnim(string name, bool loop) {
            yield return new WaitUntil(() => animEndedOrLooped);
            SetAnimation(name, loop);
        }
    }
}
