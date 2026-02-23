using UnityEngine;
using UnityEngine.UI; // Nécessaire pour manipuler la Slider

public class BossHealth : MonoBehaviour
{
    [Header("Statistiques")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthBar; // Faites glisser votre Slider ici
    public GameObject uiCanvas; // Le conteneur de la barre de vie

    private StaticBossAI bossAI;

    void Start()
    {
        currentHealth = maxHealth;
        bossAI = GetComponent<StaticBossAI>();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        // On vérifie si le boss est vulnérable dans le script StaticBossAI
        if (bossAI != null && bossAI.IsInvulnerable)
        {
            Debug.Log("Le Boss est invincible pour l'instant ! Attendez la phase de vulnérabilité.");
            return;
        }

        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    // Ajoute cette petite fonction dans ton script BossHealth
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    void Die()
    {
        Debug.Log("Boss vaincu !");
        // Ajouter ici des effets d'explosion ou charger une scène de victoire
        Destroy(gameObject);
        uiCanvas.SetActive(false);
    }
}