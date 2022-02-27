using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour {
    private const int MAX_BANDS = 1024;

    public AudioSource input;
    [Range(1, MAX_BANDS)]
    public int numberOfBands = 32;
    [Min(1)]
    public float minimumSize = 1f;
    [Min(1)]
    public float maximumSize = 10f;
    [Space]
    public float[] bandHeight;

    private float[] samples = new float[MAX_BANDS];
    private ComputeShader compute;
    private ComputeBuffer buffer;

    private void Update() {
        input.GetSpectrumData(samples, 0, FFTWindow.Blackman);

        bandHeight = new float[numberOfBands];
        int band = 0;
        for (int i = 0; i < numberOfBands; i++) {
            float average = 0;

            // As you increment on Frequency bands to set, get number of samples looking to get average of next based on for loop progress percentage
            int sampleCount = (int)Mathf.Lerp(2f, numberOfBands - 1, i / ((float)numberOfBands - 1));

            // always start the j index at the current value of band here!  if you always start from 0, band++ will increment out of _samples bounds
            for (int j = band; j < sampleCount; j++) {
                average += samples[band] * (band + 1);
                band++;
            }

            average /= sampleCount;
            bandHeight[i] = average;
        }
    }
}
