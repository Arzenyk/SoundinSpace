using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Necesario para el Bloom

public class EffectManager : MonoBehaviour
{
    public AudioSource musicSource; // Fuente de la música
    public Color baseColor = Color.black;
    public Color peakColor = Color.red;
    private float[] spectrum = new float[64];
    public Camera mainCamera;
    public Light sceneLight;
    public float minLightIntensity = 1f;
    public float maxLightIntensity = 2f;
    public Material reactiveMaterial;
    public Color materialBaseColor = Color.white;
    public Color materialPeakColor = Color.yellow;
    public ParticleSystem particles;
    public Color particleBaseColor = Color.white;
    public Color particlePeakColor = Color.cyan;
    public float minParticleSpeed = 2f;
    public float maxParticleSpeed = 100f;
    public float minParticleSize = 0.1f;
    public float maxParticleSize = 50f;

    // === POST-PROCESADO ===
    public Volume postProcessVolume; // Volume asignado en el Inspector
    private Bloom bloom;             // Componente de Bloom

    // === Suavizado opcional ===
    private float currentIntensity = 0f;

    void Start()
    {
        // Buscar el componente Bloom dentro del perfil del Volume
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out bloom))
        {
            // Bloom encontrado y listo para usarse
        }
        else
        {
            Debug.LogWarning("Bloom no encontrado en el Volume asignado.");
        }
    }

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
            for (int i = 0; i < 10; i++) // Solo graves (frecuencias bajas)
            {
                intensity += spectrum[i];
            }

            intensity = Mathf.Clamp01(intensity * 0.2f); // Menos sensible
            float smoothed = Mathf.Pow(intensity, 1.5f); // Curva suave
            currentIntensity = Mathf.Lerp(currentIntensity, smoothed, Time.deltaTime * 2f); // Suavizado

            // Cambia el color de fondo de la cámara según la intensidad
            if (mainCamera != null)
            {
                mainCamera.backgroundColor = Color.Lerp(baseColor, peakColor, currentIntensity);
            }

            if (sceneLight != null)
            {
                if (particles != null)
                {
                    var main = particles.main;
                    main.startSpeed = Mathf.Lerp(minParticleSpeed, maxParticleSpeed, currentIntensity);
                    main.startSize = Mathf.Lerp(minParticleSize, maxParticleSize, currentIntensity);
                    main.startColor = Color.Lerp(particleBaseColor, particlePeakColor, currentIntensity);

                    var shape = particles.shape;
                    shape.arcMode = currentIntensity < 0.5f ?
                        ParticleSystemShapeMultiModeValue.Loop :
                        ParticleSystemShapeMultiModeValue.Random;
                }

                sceneLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, currentIntensity);

                if (reactiveMaterial != null)
                {
                    reactiveMaterial.color = Color.Lerp(materialBaseColor, materialPeakColor, currentIntensity);
                }
            }

            // === BLOOM dinámico ===
            if (bloom != null)
            {
                bloom.intensity.value = Mathf.Lerp(0.2f, 3f, currentIntensity);
                bloom.threshold.value = Mathf.Lerp(1.2f, 5f, currentIntensity); // Opcional: más bloom con más intensidad
            }
        }
    }
}
