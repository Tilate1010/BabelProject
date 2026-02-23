using UnityEngine;
using System.Collections;

public class WarningZone : MonoBehaviour
{
    public float warningDuration = 1.5f; // Combien de temps la zone d'alerte reste visible
    public float flashSpeed = 0.2f;      // Vitesse du clignotement
    public Color startColor = new Color(1f, 0.5f, 0f, 0.5f); // Orange transparent
    public Color endColor = new Color(1f, 0f, 0f, 0.8f);   // Rouge plus opaque

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = startColor;
    }

    public IEnumerator StartWarning()       
    {
        float timer = 0f;
        while (timer < warningDuration)
        {
            // Clignotement : Alterne entre les deux couleurs
            sr.color = Color.Lerp(startColor, endColor, Mathf.PingPong(timer / flashSpeed, 1f));
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject); // La zone d'avertissement disparaît
    }
}