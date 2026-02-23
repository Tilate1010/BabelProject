using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Paramètres de Vie")]
    public int maxHealth = 3;
    private int currentHealth;

    //  garder le texte "Vie : 3", sinon tu peux laisser vide
    public TextMeshProUGUI healthDisplay;

    // --- AJOUT : La référence vers la barre de vie segmentée ---
    public HealthBarUI healthBar;

    [Header("Feedback Dégâts")]
    public CameraShake camShake;
    public GameObject shockwavePrefab;
    public float shakeIntensity = 2f;
    public float shakeDuration = 0.2f;

    [Header("Invulnérabilité (Dégâts)")]
    public float iFramesDuration = 1.5f;
    public int numberOfFlashes = 5;
    private SpriteRenderer spriteRend;
    private bool isInvulnerable = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRend = GetComponent<SpriteRenderer>();

        UpdateUI(); // Met à jour le texte

        // --- AJOUT : Initialiser les cœurs/segments au démarrage ---
        if (healthBar != null)
        {
            healthBar.SetupHearts(maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth += damage;

        UpdateUI(); // Met à jour le texte

        // --- AJOUT : Mettre à jour les segments visuels ---
        if (healthBar != null)
        {
            healthBar.UpdateHearts(currentHealth);
        }

        // --- Feedback (Shake + Onde) ---
        if (camShake != null) camShake.Shake(shakeIntensity, shakeDuration);
        if (shockwavePrefab != null) Instantiate(shockwavePrefab, transform.position, Quaternion.identity);

        // --- Mort ou Invincibilité ---
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageInvulnerabilityRoutine());
        }
    }

    // Gère le clignotement ROUGE (Dégâts)
    private IEnumerator DamageInvulnerabilityRoutine()
    {
        isInvulnerable = true;
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        isInvulnerable = false;
    }

    // Gère la transparence (Dash) - Appelé par PlayerMovementPro
    public void TriggerDashInvulnerability(float duration)
    {
        StartCoroutine(DashInvulnerabilityRoutine(duration));
    }

    private IEnumerator DashInvulnerabilityRoutine(float duration)
    {
        isInvulnerable = true;
        Color originalColor = spriteRend.color;
        spriteRend.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        yield return new WaitForSeconds(duration);
        spriteRend.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        isInvulnerable = false;
    }

    void UpdateUI()
    {
        if (healthDisplay != null) healthDisplay.text = "Vie : " + currentHealth;
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}