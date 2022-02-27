using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PostControlAsset : PlayableAsset {
    public PostControlBehaviour template;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        return ScriptPlayable<PostControlBehaviour>.Create(graph, template);
    }
}
