using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeatDetectionManager : MonoBehaviour
{
    public AudioSource audioSource;
    public float[] spectrumData = new float[512];
    public float threshold = 0.1f;

    void Update()
    {
        DetectBeats();
    }

    void DetectBeats()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Hamming);

        for (int i = 0; i < spectrumData.Length; i++)
        {
            if (spectrumData[i] > threshold)
            {
                OnBeatDetected(i);
            }
        }
    }

    void OnBeatDetected(int index)
    {
        Debug.Log("Beat detected at frequency index: " + index);
        // Add your synchronization logic here
    }
}