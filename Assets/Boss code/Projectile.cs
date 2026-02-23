using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 velocity;
    private float knockback;

    [System.Obsolete]
    public void Setup(Vector2 vel, float kb)
    {
        velocity = vel;
        knockback = kb;
        GetComponent<Rigidbody2D>().velocity = vel;
        Destroy(gameObject, 4f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Appliquer les dégâts
            Health health = other.GetComponent<Health>();
            if (health != null && other.gameObject.layer != LayerMask.NameToLayer("Ghost"))
            {
                health.TakeDamage(-1);

                // APPLIQUER LE KNOCKBACK
                Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 pushDir = (other.transform.position - transform.position).normalized;
                    playerRb.AddForce(pushDir * knockback, ForceMode2D.Impulse);
                }
            }
            Destroy(gameObject);
        }
    }
}