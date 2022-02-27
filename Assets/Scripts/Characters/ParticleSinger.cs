using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSinger : MonoBehaviour {
    public ParticleSystem system;

    public int amountToEmit;

    public void OnSing(SideDir dir) {
        Vector3 rot = Vector3.zero;

        if (dir == SideDir.Left) rot.z = 180f;
        if (dir == SideDir.Down) rot.z = -90f;
        if (dir == SideDir.Up) rot.z = 90f;
        if (dir == SideDir.Right) rot.z = 0f;

        transform.localEulerAngles = rot;
        system.Emit(amountToEmit);
    }
}
