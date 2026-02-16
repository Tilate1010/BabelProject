using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    void Start()
    {
        // Le système de particule se détruit tout seul quand il a fini de jouer
        ParticleSystem ps = GetComponent<ParticleSystem>();
        Destroy(gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }
}