using System.Collections;
using UnityEngine;

public class SupportBossAI : MonoBehaviour
{
    [Header("R�f�rences")]
    public GameObject largeProjectilePrefab; // Un projectile avec un gros visuel
    public GameObject warningZonePrefab; // << NOUVEAU : ton prefab de WarningZone
    public Transform player;

    [Header("Param�tres d'Ar�ne")]
    public float arenaWidth = 20f;
    public float arenaHeight = 12f;


    void Start()
    {
        if (player == null)
        {
            // Cherche l'objet avec le tag "Player"
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else Debug.LogError("Le SupportBoss ne trouve pas d'objet avec le tag 'Player' !");
        }
    }
    // --- PATTERN A : LA PLUIE DE CATASTROPHE (Verticale) ---
    public IEnumerator Pattern_RainOfFire()
    {
        Debug.Log("Support: Pluie de feu !");
        // Crée les zones d'avertissement
        for (int i = 0; i < 15; i++)
        {
            float randomX = Random.Range(-arenaWidth / 2, arenaWidth / 2);
            Vector3 spawnPos = new Vector3(randomX, player.position.y + arenaHeight / 2 - 2, 0); // Légèrement au-dessus du joueur
            GameObject warning = Instantiate(warningZonePrefab, spawnPos, Quaternion.identity);
            warning.transform.localScale = new Vector3(2f, 0.5f, 1f); // Taille de la zone de chute
            StartCoroutine(warning.GetComponent<WarningZone>().StartWarning());
        }
        yield return new WaitForSeconds(warningZonePrefab.GetComponent<WarningZone>().warningDuration); // Attend la fin de l'avertissement

        // Puis l'attaque réelle se déclenche
        for (int i = 0; i < 15; i++)
        {
            float randomX = Random.Range(-arenaWidth / 2, arenaWidth / 2);
            Vector3 spawnPos = new Vector3(randomX, arenaHeight / 2 + 5, 0);

            GameObject p = Instantiate(largeProjectilePrefab, spawnPos, Quaternion.identity);
            p.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * 8f;

            yield return new WaitForSeconds(0.15f);
        }
    }

    // --- PATTERN B : LE BALAYAGE LASER (Horizontal) ---
    public IEnumerator Pattern_HorizontalSweep()
    {
        Debug.Log("Support: Balayage horizontal !");
        float startY = Random.Range(-arenaHeight / 3, arenaHeight / 3);

        // Crée la zone d'avertissement
        GameObject warning = Instantiate(warningZonePrefab, new Vector3(0, startY, 0), Quaternion.identity);
        warning.transform.localScale = new Vector3(arenaWidth + 5f, 1.5f, 1f); // Bande horizontale large
        StartCoroutine(warning.GetComponent<WarningZone>().StartWarning());
        yield return new WaitForSeconds(warningZonePrefab.GetComponent<WarningZone>().warningDuration);

        // Puis l'attaque réelle se déclenche
        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPos = new Vector3(-arenaWidth / 2 - 2, startY + (i * 1.5f), 0);
            GameObject p = Instantiate(largeProjectilePrefab, spawnPos, Quaternion.identity);
            p.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * 12f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // --- PATTERN C : LE QUADRILLAGE (Grille) ---
    public IEnumerator Pattern_GridAttack()
    {
        Debug.Log("Support: Quadrillage !");
        // Dessine la grille d'avertissement
        for (float x = -arenaWidth / 2; x <= arenaWidth / 2; x += 4f)
        {
            GameObject warning = Instantiate(warningZonePrefab, new Vector3(x, 0, 0), Quaternion.identity);
            warning.transform.localScale = new Vector3(0.5f, arenaHeight + 2, 1f); // Barre verticale
            StartCoroutine(warning.GetComponent<WarningZone>().StartWarning());
        }
        for (float y = -arenaHeight / 2; y <= arenaHeight / 2; y += 4f)
        {
            GameObject warning = Instantiate(warningZonePrefab, new Vector3(0, y, 0), Quaternion.identity);
            warning.transform.localScale = new Vector3(arenaWidth + 2, 0.5f, 1f); // Barre horizontale
            StartCoroutine(warning.GetComponent<WarningZone>().StartWarning());
        }
        yield return new WaitForSeconds(warningZonePrefab.GetComponent<WarningZone>().warningDuration);

        // Puis les projectiles suivent le même chemin
        for (float x = -arenaWidth / 2; x <= arenaWidth / 2; x += 4f)
        {
            Vector3 spawnPos = new Vector3(x, arenaHeight / 2 + 5, 0);
            Instantiate(largeProjectilePrefab, spawnPos, Quaternion.identity).GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * 6f;
        }
        yield return new WaitForSeconds(1f);
        for (float y = -arenaHeight / 2; y <= arenaHeight / 2; y += 4f)
        {
            Vector3 spawnPos = new Vector3(-arenaWidth / 2 - 5, y, 0);
            Instantiate(largeProjectilePrefab, spawnPos, Quaternion.identity).GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * 6f;
        }
    }

    // --- PATTERN D : L'EFFONDREMENT (Cercle convergent) ---
    public IEnumerator Pattern_Convergence()
    {
        Debug.Log("Support: Convergence sur le joueur !");
        Vector3 targetPos = player.position; // Cible le joueur au début de l'avertissement

        // Crée les avertissements autour de la cible
        for (int i = 0; i < 12; i++)
        {
            float angle = i * 30 * Mathf.Deg2Rad;
            Vector3 spawnPos = targetPos + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 8f;
            GameObject warning = Instantiate(warningZonePrefab, spawnPos, Quaternion.identity);
            warning.transform.localScale = new Vector3(1f, 1f, 1f); // Petits cercles
            StartCoroutine(warning.GetComponent<WarningZone>().StartWarning());
        }
        yield return new WaitForSeconds(warningZonePrefab.GetComponent<WarningZone>().warningDuration);

        // Puis les projectiles réels
        for (int i = 0; i < 12; i++)
        {
            float angle = i * 30 * Mathf.Deg2Rad;
            Vector3 spawnPos = targetPos + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 8f;

            Vector2 dir = (targetPos - spawnPos).normalized;
            GameObject p = Instantiate(largeProjectilePrefab, spawnPos, Quaternion.identity);
            p.GetComponent<Rigidbody2D>().linearVelocity = dir * 10f;
        }
        yield return new WaitForSeconds(0.5f);
    }
}

    