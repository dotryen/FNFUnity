using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

public class Girlfriend : BeatBehaviour {
    public new SpriteAnimation animation;

    [Space]
    [Min(0)]
    public float xRandom = 0.5f;
    [Min(0)]
    public float yRandomMin = 0.5f;
    [Min(0)]
    public float yRandomMax = 0.1f;
    [Space]
    public int poolCount = 20;
    public SpriteAnimation templateSprite;
    public Transform container;

    private ConcurrentBag<SpriteAnimation> bag = new ConcurrentBag<SpriteAnimation>();
    private bool danced = false;
    private bool sad = false;

    public bool Danced => danced;
    public bool IsSad => sad;

    private void Awake() {
        templateSprite.gameObject.SetActive(false);
        for (int i = 0; i < poolCount; i++) {
            bag.Add(Instantiate(templateSprite, container));
        }
    }

    public void Dance() {
        animation.update = true;
        animation.SetAnimation("Dance", true, danced ? 15 : 0);

        var length = 15 * animation.Threshold;
        animation.speed = length / (float)Conductor.SecondsPerBeat;

        danced = !danced;
        sad = false;
    }

    public void Sad() {
        animation.SetAnimation("Sad");
        sad = true;
    }

    public void ShowRating(Rating rating) {
        if (bag.TryTake(out SpriteAnimation animation)) {
            animation.gameObject.SetActive(true);
            animation.transform.position = container.position;
            animation.SetAnimation(rating.ToString(), false);

            StartCoroutine(Tween(animation));
        }
    }

    // FlxVelocity
    private float CalcVelocity(float velocity, float accel, float max, float elapsed) {
        if (accel != 0) {
            velocity += accel * elapsed;
        }
        if (velocity != 0 && max != 0) {
            velocity = Mathf.Clamp(velocity, -max, max);
        }
        return velocity;
    }

    private IEnumerator Tween(SpriteAnimation animation) {
        // yield return new WaitForSeconds(Conductor.Crochet * 0.001f);

        Vector2 velocity = new Vector2(-Random.Range(0, 10), Random.Range(140, 174));
        Vector2 accel = new Vector2(0, -550);
        var life = 0.2f;

        while (life > 0) {
            Vector2 deltaPos = Vector2.zero;

            // update x
            {
                var delta = 0.5f * (CalcVelocity(velocity.x, accel.x, 10000, Time.deltaTime) - velocity.x);
                velocity.x += delta;
                delta = velocity.x * Time.deltaTime;
                velocity.x += delta;
                deltaPos.x += delta;
            }

            // update y
            {
                var delta = 0.5f * (CalcVelocity(velocity.y, accel.y, 10000, Time.deltaTime) - velocity.y);
                velocity.y += delta;
                delta = velocity.y * Time.deltaTime;
                velocity.y += delta;
                deltaPos.y += delta;
            }

            deltaPos *= Globals.PIXEL_SIZE;
            animation.transform.position += (Vector3)deltaPos;

            life -= Time.deltaTime;

            yield return null;
        }

        animation.gameObject.SetActive(false);
        bag.Add(animation);
    }
}
