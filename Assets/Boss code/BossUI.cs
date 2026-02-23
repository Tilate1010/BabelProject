using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public Slider healthBar;
    public BossHealth bossHealth; // Référence au script de vie du Boss

    void Start()
    {
        // On initialise la barre avec les PV max du boss
        healthBar.maxValue = bossHealth.maxHealth;
        healthBar.value = bossHealth.maxHealth;
    }

    void Update()
    {
        // La barre suit les PV actuels
        // On utilise une interpolation (Lerp) pour que ce soit plus fluide
        healthBar.value = Mathf.Lerp(healthBar.value, bossHealth.GetCurrentHealth(), Time.deltaTime * 5f);
    }
}