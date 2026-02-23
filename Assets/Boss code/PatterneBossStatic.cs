using System.Collections;
using UnityEngine;

public class StaticBossAI : MonoBehaviour
{
    [Header("Allié")]
    public SupportBossAI supportBoss;

    [Header("Références")]
    public Transform player;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public SpriteRenderer bossSprite;

    [Header("Paramètres Généraux")]
    public float timeBetweenPatterns = 2f;
    public int attacksBeforeVulnerable = 4;
    private int attackCounter = 0;
    private bool isInvulnerable = true;
    private bool isActing = false;
    public bool IsInvulnerable => isInvulnerable;
    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(BossLoop());
    }

    IEnumerator BossLoop()
    {
        while (true)
        {
            if (!isActing)
            {
                if (attackCounter >= attacksBeforeVulnerable)
                {
                    yield return StartCoroutine(VulnerabilityPhase());
                    {
                        if (Random.value > 0.66f)
                        {
                            int randSupport = Random.Range(1, 5);
                            switch (randSupport)
                            {
                                case 1: yield return StartCoroutine(supportBoss.Pattern_RainOfFire()); break;
                                case 2: yield return StartCoroutine(supportBoss.Pattern_HorizontalSweep()); break;
                                case 3: yield return StartCoroutine(supportBoss.Pattern_GridAttack()); break;
                                case 4: yield return StartCoroutine(supportBoss.Pattern_Convergence()); break;
                            }
                        }
                        else
                        {
                            // Choisit un pattern aléatoire entre 1 et 5
                            int rand = Random.Range(1, 6);
                            yield return StartCoroutine(ExecutePattern(rand));
                            attackCounter++;
                        }

                    }
                    yield return new WaitForSeconds(timeBetweenPatterns);
                }
            }

            [System.Obsolete]
            IEnumerator ExecutePattern(int index)
            {
                isActing = true;
                Debug.Log("Boss lance le Pattern : " + index);

                switch (index)
                {
                    case 1: yield return StartCoroutine(Pattern_SniperShot()); break;
                    case 2: yield return StartCoroutine(Pattern_RadialBurst()); break;
                    case 3: yield return StartCoroutine(Pattern_FastBarrage()); break;
                    case 4: yield return StartCoroutine(Pattern_KnockbackWave()); break;
                    case 5: yield return StartCoroutine(Pattern_SpiralRain()); break;
                }

                isActing = false;
            }

            // --- PATTERN 1 : TIR PRÉCIS (Dégâts élevés) ---
            IEnumerator Pattern_SniperShot()
            {
                bossSprite.color = Color.red; // Anticipation
                yield return new WaitForSeconds(1f);
                FireProjectile(player.position - transform.position, 12f, 2f);
                bossSprite.color = Color.white;
            }

            // --- PATTERN 2 : EXPLOSION RADIALE (Contrôle de zone) ---
            IEnumerator Pattern_RadialBurst()
            {
                for (int i = 0; i < 12; i++)
                {
                    float angle = i * 30f;
                    Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    FireProjectile(dir, 5f, 1f);
                }
                yield return null;
            }

            // --- PATTERN 3 : BARRAGE RAPIDE (Agitation) ---
            [System.Obsolete]
            IEnumerator Pattern_FastBarrage()
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 randomDir = (player.position - transform.position).normalized;
                    randomDir += new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
                    FireProjectile(randomDir, 15f, 0.5f);
                    yield return new WaitForSeconds(0.15f);
                }
            }

            // --- PATTERN 4 : ONDE DE CHOC (Knockback massif) ---
            IEnumerator Pattern_KnockbackWave()
            {
                bossSprite.color = Color.blue;
                yield return new WaitForSeconds(0.5f);
                // Tire des projectiles larges qui ont un gros knockback (configuré sur le projectile)
                for (int i = 0; i < 16; i++)
                {
                    float angle = i * 22.5f;
                    Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    FireProjectile(dir, 8f, 10f); // Force de recul de 10
                }
                bossSprite.color = Color.white;
            }

            // --- PATTERN 5 : SPIRALE (Esquive complexe) ---
            IEnumerator Pattern_SpiralRain()
            {
                float angle = 0;
                for (int i = 0; i < 20; i++)
                {
                    Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    FireProjectile(dir, 6f, 1f);
                    angle += 20f;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // --- PHASE DE VULNÉRABILITÉ ---
            IEnumerator VulnerabilityPhase()
            {
                isActing = true;
                isInvulnerable = false;
                bossSprite.color = Color.gray; // Le boss devient terne
                Debug.Log("BOSS VULNÉRABLE ! FRAPPEZ !");

                yield return new WaitForSeconds(5f); // Temps pour le joueur de taper

                bossSprite.color = Color.white;
                isInvulnerable = true;
                attackCounter = 0;
                isActing = false;
            }

            [System.Obsolete]
            void FireProjectile(Vector2 direction, float speed, float kbForce)
            {
                GameObject p = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
                Projectile script = p.GetComponent<Projectile>();
                script.Setup(direction.normalized * speed, kbForce);
            }
        }
    }
}
        