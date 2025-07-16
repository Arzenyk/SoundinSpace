using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public AudioSource musicSource; // Fuente de la música
    public Color baseColor = Color.black;
    public Color peakColor = Color.white;
    private float[] spectrum = new float[64];
    public Camera mainCamera;

    void Update()
    {
        UpdateVisualEffectsFromMusic();
    }


    void UpdateVisualEffectsFromMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
            float intensity = 0f;
            for (int i = 0; i < spectrum.Length; i++)
            {
                intensity += spectrum[i];
            }
            intensity = Mathf.Clamp01(intensity * 1f); // Ajusta el multiplicador según tu música

            // Cambia el color de fondo de la cámara según la intensidad
            if (mainCamera != null)
            {
                mainCamera.backgroundColor = Color.Lerp(baseColor, peakColor, intensity);
            }
        }
    }



}
