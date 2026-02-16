using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackReceiver : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 sourcePosition, float force)
    {
        // Calcul de la direction opposée à la source
        Vector2 direction = (transform.position - (Vector3)sourcePosition).normalized;

        // Reset de la vélocité (version Unity 6)
        rb.linearVelocity = Vector2.zero;

        // Application de l'impact
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Petite coroutine pour calmer le jeu après l'impact (optionnel)
        StartCoroutine(ResetVelocity());
    }

    IEnumerator ResetVelocity()
    {
        yield return new WaitForSeconds(0.2f);
        // On freine l'objet (optionnel, dépend de ton game feel)
        rb.linearVelocity = rb.linearVelocity * 0.1f;
    }
}