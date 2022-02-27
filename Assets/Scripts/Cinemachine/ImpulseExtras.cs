using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ImpulseExtras : MonoBehaviour {
    private CinemachineImpulseSource source;

    public Vector3 minRand;
    public Vector3 maxRand;

    private void Start() {
        source = GetComponent<CinemachineImpulseSource>();
    }

    public void ImpulseWithRandomTimlineCompatible() {
        ImpulseWithRandom(minRand, maxRand);
    }

    public void ImpulseWithRandom(Vector3 minimum, Vector3 maximum) {
        var min = Abs(minimum);
        var max = Abs(maximum);

        var force = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        
        if (Mathf.RoundToInt(Random.value) == 1) force.x *= -1f;
        if (Mathf.RoundToInt(Random.value) == 1) force.y *= -1f;
        if (Mathf.RoundToInt(Random.value) == 1) force.z *= -1f;

        source.GenerateImpulseWithVelocity(force);
    }

    public Vector3 Abs(Vector3 input) {
        return new Vector3(Mathf.Abs(input.x), Mathf.Abs(input.y), Mathf.Abs(input.z));
    }
}
