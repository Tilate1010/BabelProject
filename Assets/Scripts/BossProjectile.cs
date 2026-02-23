using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float lifeTime = 5f; // Détruit le projectile après 5s s'il ne touche rien

    void Start()
    {
        // Sécurité : détruit l'objet après un délai pour libérer la mémoire
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. On cherche si l'objet touché a le script HealthSystem
        Health playerHealth = collision.GetComponent<Health>();

        if (playerHealth != null)
        {
            
            playerHealth.TakeDamage(-1);
            Destroy(gameObject);
        }

        // Détruire le projectile s'il touche un mur (Layer "Obstacle" ou "Wall")
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}