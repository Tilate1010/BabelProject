using UnityEngine;
using Unity.Cinemachine; // Note : "Unity.Cinemachine" obligatoire ici
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera virtualCam; // Nom en version 3
    private CinemachineBasicMultiChannelPerlin noiseComponent;

    void Awake()
    {
        virtualCam = GetComponent<CinemachineCamera>();

        // En v3, on cherche le composant de bruit directement
        noiseComponent = GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (noiseComponent == null) noiseComponent = GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float intensity, float duration)
    {
        if (noiseComponent != null)
        {
            StopAllCoroutines();
            StartCoroutine(PerformShake(intensity, duration));
        }
    }

    private IEnumerator PerformShake(float intensity, float duration)
    {
        noiseComponent.AmplitudeGain = intensity; // Note : Pas de "m_"
        yield return new WaitForSeconds(duration);
        noiseComponent.AmplitudeGain = 0f;
    }
}