using System.Collections;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Réglages Dégâts")]
    public int damageAmount = 1;
    public float damageInterval = 1f; /* Temps entre deux dégâts */

    [Header("Réglages Recul")]
    public float knockbackForce = 15f; // La force du recul

    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // ###1. Gestion des PV ###
        Health playerHealth = collision.GetComponent<Health>();
        if (playerHealth != null)
        {
            damageCoroutine = StartCoroutine(DealDamageOverTime(playerHealth));
            KnockbackReceiver kb = collision.GetComponent<KnockbackReceiver>();

            if (kb != null)
            {
                // On applique le recul :
                // Source du choc = transform.position (le centre de cet objet)
                kb.ApplyKnockback(transform.position, knockbackForce);
            }
        }

        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    /* Les degats dans le temps */
    IEnumerator DealDamageOverTime(Health health)
    {
        // On applique les dégâts immédiatement à l'entrée
        health.TakeDamage(-damageAmount);
        yield return new WaitForSeconds(damageInterval);

        while (true)
        {
            health.TakeDamage(-damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}