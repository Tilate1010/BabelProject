using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum BossState { Idle, Chasing, Attacking }

public class HexBossAI : MonoBehaviour
{
    [Header("Phase de Colère (Burst)")]
    public int projectilesInBurst = 21; // Nombre de projectiles dans le cercle
    public float burstProjectileSpeed = 50f;

    [Header("References")]
    public Transform player;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Parametres de Combat")]
    public float detectionRange = 8f;
    public float meleeRange = 3f;
    public float moveSpeed = 10f;

    [Header("Patterns Distance (Idle)")]
    public float timeBetweenShots = 0.5f;
    public int burstCount = 3;

    [Header("Patterns Corps et Corps (Chasing)")]
    public float attackCooldown = 2f;
    public float dashAttackSpeed = 35f;

    private BossState currentState = BossState.Idle;
    private bool isPerformingAction = false;
    private Rigidbody2D rb;

    [System.Obsolete]
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        // On lance la boucle de d�cision
        StartCoroutine(BossLogicLoop());
    }

    [System.Obsolete]
    public void CircularBurst()
    {
        Debug.Log("Boss: Explosion circulaire !");

        float angleStep = 360f / projectilesInBurst;
        float angle = 0f;

        for (int i = 0; i < projectilesInBurst; i++)
        {
            // Calcul de la direction pour chaque projectile
            float projectileDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float projectileDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector2 projectileMoveVector = new Vector2(projectileDirX, projectileDirY);
            Vector2 projectileDir = (projectileMoveVector - (Vector2)transform.position).normalized;

            // Création du projectile
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            proj.GetComponent<Rigidbody2D>().velocity = projectileDir * burstProjectileSpeed;

            angle += angleStep;
        }
    }

    [System.Obsolete]
    IEnumerator BossLogicLoop()
    {
        while (true)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (!isPerformingAction)
            {
                if (distanceToPlayer > detectionRange)
                {
                    currentState = BossState.Idle;
                    yield return StartCoroutine(DistanceAttackPattern());
                }
                else
                {
                    currentState = BossState.Chasing;
                    yield return StartCoroutine(ChaseAndMeleePattern());
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // --- PATTERN 1 : DISTANCE (IDLE) ---
    IEnumerator DistanceAttackPattern()
    {
        isPerformingAction = true;
        Debug.Log("Boss: Mode Tourelle � distance");

        for (int i = 0; i < burstCount; i++)
        {
            // Petit "shake" ou changement de couleur pour pr�venir du tir
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            sr.color = originalColor;

            // Tirer le projectile vers le joueur
            GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Vector2 dir = (player.position - shootPoint.position).normalized;
            proj.GetComponent<Rigidbody2D>().linearVelocity = dir * 7f;

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(timeBetweenShots);
        isPerformingAction = false;
    }

    // --- PATTERN 2 : CHASING & CORPS � CORPS ---
    [System.Obsolete]
    IEnumerator ChaseAndMeleePattern()
    {
        isPerformingAction = true;

        float distance = Vector2.Distance(transform.position, player.position);

        // Phase d'approche
        while (distance > meleeRange)
        {
            Vector2 moveDir = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
            distance = Vector2.Distance(transform.position, player.position);
            yield return new FixedUpdate();

            // S�curit� : si le joueur s'enfuit trop loin, on repasse en distance
            if (distance > detectionRange + 2f) break;
        }

        // Attaque de contact (Dash Rapide)
        if (distance <= meleeRange)
        {
            Debug.Log("Boss: Attaque de contact !");
            Vector2 attackDir = (player.position - transform.position).normalized;

            // Recul de pr�paration
            rb.linearVelocity = -attackDir * 2f;
            yield return new WaitForSeconds(0.3f);

            // Dash vers le joueur
            rb.linearVelocity = attackDir * dashAttackSpeed;
            yield return new WaitForSeconds(0.2f);
            rb.linearVelocity = Vector2.zero;
            rb.velocity = attackDir * dashAttackSpeed;
            yield return new WaitForSeconds(0.2f);
            rb.velocity = Vector2.zero;

            // --- AJOUT : Explosion juste après le contact ---
            CircularBurst();

            yield return new WaitForSeconds(attackCooldown);
            isPerformingAction = false;
        }

        yield return new WaitForSeconds(attackCooldown);
        isPerformingAction = false;
    }
}