using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

// Tokens are git ignored, make your own token dumbass.

public class DiscordController : MonoBehaviour {
    public static DiscordController Instance { get; private set; }
    private Discord.Discord discord;

    private void Awake() {
#if UNITY_EDITOR
        return; // do not do shit in editor
#endif

        if (Instance != null) {
            Destroy(this);
            return;
        }

        try {
            discord = new Discord.Discord(Tokens.DISCORD_TOKEN, (ulong)CreateFlags.NoRequireDiscord);
            Instance = this;
        } catch (System.Exception e) {
            Debug.LogError(e);
            discord = null;
        }

        Activity();
    }

    private void Update() {
        if (discord == null) return;
        discord.RunCallbacks();
    }

    public void Activity() {
        if (discord == null) return;
        var manager = discord.GetActivityManager();
        var activity = new Activity() {
            State = "WIP",
            Assets = new ActivityAssets() {
                LargeImage = "jackie",
                SmallImage = "kadelogo",
                LargeText = "a picture of the opponent",
                SmallText = "the kade engine logo i stole lol"
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
