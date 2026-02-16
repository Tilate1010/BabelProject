using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    [Header("Réglages Onde")]
    public float expansionSpeed = 10f;
    public float fadeSpeed = 5f;

    [Header("Réglages Particules")]
    public GameObject particlePrefab; // Glisse ton prefab "ShockwaveDust" ici
    public float spawnThreshold = 0.3f; // A quel niveau de transparence on déclenche les particules (0 à 1)
    private SpriteRenderer rend;
    private Color objColor;
    private bool hasSpawnedParticles = false;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        objColor = rend.color;

        transform.localScale = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        // 1. Agrandir
        transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;

        // 2. Devenir transparent
        objColor.a -= fadeSpeed * Time.deltaTime;
        rend.color = objColor;

        // 3. Logique de transformation en particules
        if (objColor.a <= spawnThreshold && !hasSpawnedParticles)
        {
            SpawnDebris();
        }

        // 4. Destruction finale
        if (objColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    void SpawnDebris()
    {
        hasSpawnedParticles = true;

        if (particlePrefab != null)
        {
            GameObject dust = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = dust.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                var shape = ps.shape;
                // On ajuste le rayon des particules à la taille actuelle de l'onde
                shape.radius = transform.localScale.x / 2f;

                // On lance l'émission (au cas où "Play on Awake" est décoché)
                ps.Play();

                // OPTIONNEL : Détruire le prefab de particules automatiquement après sa durée de vie
                Destroy(dust, ps.main.duration + ps.main.startLifetime.constantMax);
            }

            // Désactive le sprite de l'onde pour laisser place aux particules
            rend.enabled = false;
        }
    }
}