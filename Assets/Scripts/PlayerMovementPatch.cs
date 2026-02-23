using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement Standard")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    [Header("Paramètres de Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Gestion des Layers (Collisions)")]
    [Tooltip("Nom du layer à utiliser pendant le dash (ex: Ghost)")]
    public string dashLayerName = "Ghost";

    private Vector2 movement;
    private int originalLayer;
    private Vector2 linearVelocity;
    private bool isDashing = false;
    private bool canDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // On sauvegarde le layer actuel du joueur (ex: 'Player')
        originalLayer = gameObject.layer;
    }

    void Update()
    {
        // On ignore les inputs de mouvement classiques pendant le dash
        if (isDashing) return;

        // Inputs de déplacement (Top-Down)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Lancer le Dash
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        // Si on dash, la physique est gérée par la coroutine (velocity)
        if (isDashing) return;

        // Déplacement normal
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // 1. Changer le layer pour passer à travers les obstacles/ennemis
        gameObject.layer = LayerMask.NameToLayer(dashLayerName);

        // 2. Calculer la direction du dash
        // Si aucune touche n'est pressée, on dash vers le haut par défaut (ou vers la droite selon ton choix)
        Vector2 dashDir = movement.magnitude > 0 ? movement.normalized : Vector2.up;

        // 3. Appliquer la vitesse de dash
        linearVelocity = dashDir * dashSpeed;

        // 4. Attendre la fin de la durée du dash
        yield return new WaitForSeconds(dashDuration);

        // 5. Rétablir le layer d'origine (redevient vulnérable/bloqué)
        gameObject.layer = originalLayer;

        linearVelocity = Vector2.zero; // Stop le dash net
        isDashing = false;

        // 6. Cooldown avant de pouvoir dasher à nouveau
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}