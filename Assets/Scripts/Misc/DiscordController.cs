using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using FNF.Core;

// Tokens are git ignored, make your own token dumbass.

public class DiscordController : MonoBehaviour {
    public static DiscordController Instance { get; private set; }
    private Discord.Discord discord;

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        } else {
            Instance = this;
        }

#if UNITY_EDITOR
        return; // do not do shit in editor
#endif

        try {
            // Token excluded on git
            discord = new Discord.Discord(Tokens.DISCORD_TOKEN, (ulong)CreateFlags.NoRequireDiscord);
        } catch (System.Exception e) {
            Debug.LogError(e);
            discord = null;
        }

        IdleActivity();
    }

    private void Update() {
        if (discord == null) return;
        discord.RunCallbacks();
    }

    public void IdleActivity() {
        if (discord == null) return;
        var manager = discord.GetActivityManager();

        var activity = new Activity() {
            State = "Idle",
            Assets = new ActivityAssets() {
                LargeImage = "kadelogo",
            }
        };

        manager.UpdateActivity(activity, DiscordCallback);
    }

    public void PlayingActivity() {
        if (discord == null) return;
        var manager = discord.GetActivityManager();

        var activity = new Activity() {
            Details = $"Playing: {GameController.Instance.SongAssets.Metadata.songName}",
            State = $"Score: {GameplayVars.Score} | {Ratings.GetCurrentRatingString()}",
            Assets = new ActivityAssets() {
                LargeImage = "mad",
                LargeText = "Leave he is going to kill yoy!!!!",
                SmallImage = "kadelogo"
            }
        };

        manager.UpdateActivity(activity, DiscordCallback);
    }

    private void DiscordCallback(Result result) {
        if (result != Result.Ok) {
            Debug.LogError("WTF!!!?!?!? discord brokey");
        }
    }
}
