using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovementPro : MonoBehaviour
{
    [Header("Mouvement")]
    // ... (variables de vitesse existantes) ...
    public float maxSpeed = 7f;
    public float acceleration = 50f;
    public float deceleration = 40f;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    
    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;
    public string dashLayerName = "Dodge";
    private int originalLayer;
    [Header("Dash Visuals")]
    public GameObject ghostPrefab; // GLISSE le PREFAB "DashGhost" ICI
    public float ghostSpawnDelay = 0.05f; // Temps entre chaque fantôme

    [Header("Références")]
    public Rigidbody2D rb;
    public Health playerHealth;
    public Animator animator; //ANIMATOR ICI
    public SpriteRenderer spriteRend; // SPRITE RENDERER ICI

    // ... (Update et FixedUpdate ne changent pas) ...
    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && !isDashing)
        {
            // ... (Lecture des touches ZSQD) ...
            float targetX = 0; float targetY = 0;
            if (kb.wKey.isPressed) targetY = 1;
            if (kb.sKey.isPressed) targetY = -1;
            if (kb.aKey.isPressed) targetX = -1;
            if (kb.dKey.isPressed) targetX = 1;
            moveInput = new Vector2(targetX, targetY).normalized;

            if (kb.spaceKey.wasPressedThisFrame && canDash)
            { /* ok ici c'est ok */
                StartCoroutine(Dash());
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        // ... (Calcul de la vitesse progressive) ...
        float targetSpeedX = moveInput.x * maxSpeed;
        float targetSpeedY = moveInput.y * maxSpeed;
        currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetSpeedX, (moveInput.x != 0 ? acceleration : deceleration) * Time.fixedDeltaTime);
        currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, targetSpeedY, (moveInput.y != 0 ? acceleration : deceleration) * Time.fixedDeltaTime);
        rb.linearVelocity = currentVelocity;
    }


    // --- C'EST ICI QUE TOUT CHANGE ---
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        gameObject.layer = LayerMask.NameToLayer(dashLayerName);

        // 2. Lancer l'effet de traînée en parallèle
        StartCoroutine(SpawnGhostsRoutine());

        Vector2 dashDir = moveInput == Vector2.zero ? new Vector2(1, 0) : moveInput;

        // 3. Invincibilité et Transparence du joueur                                                    ici souci "i guess" 
        if (playerHealth != null)
        {
            // On utilise la fonction synchronisée créée précédemment
            // Assure-toi que Health.cs applique bien une transparence (alpha 0.5f)
            playerHealth.TriggerDashInvulnerability(dashDuration);
        }

        rb.linearVelocity = dashDir * dashSpeed;

        // Le dash dure X secondes
        yield return new WaitForSeconds(dashDuration);
        gameObject.layer = originalLayer;
        // FIN DU DASH
        isDashing = false;
        if (animator != null) animator.SetBool("isDashing", false);
        // Note: La coroutine SpawnGhostsRoutine s'arrêtera toute seule car isDashing est false

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // Nouvelle Coroutine pour faire poper les fantômes
    // Exemple à mettre sur l'épée du joueur
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            // On récupère le script de santé du boss
            BossHealth bh = other.GetComponent<BossHealth>();

            if (bh != null)
            {
                bh.TakeDamage(10f); // Inflige 10 points de dégâts
            }
        }
    }
    private IEnumerator SpawnGhostsRoutine()
    {
        while (isDashing)
        {

            // On crée un fantôme à la position actuelle du joueur
            GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);

            // IMPORTANT : On donne au fantôme le sprite actuel du joueur pour qu'il ait la bonne pose
            ghost.GetComponent<SpriteRenderer>().sprite = spriteRend.sprite;

            // On attend un peu avant de faire poper le suivant
            yield return new WaitForSeconds(ghostSpawnDelay);
        }
    }
}