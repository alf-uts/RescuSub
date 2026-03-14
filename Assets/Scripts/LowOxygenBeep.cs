using UnityEngine;
using UnityEngine.UI;

public class LowOxygenBeep : MonoBehaviour
{
    public Slider slider;
    public AudioSource audioSource;
    public float threshold = 0.25f;

    void Update()
    {
        if (slider.normalizedValue <= threshold)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}