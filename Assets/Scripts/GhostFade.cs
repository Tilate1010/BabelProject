using UnityEngine;
using System.Collections;

public class GhostFade : MonoBehaviour
{
    private SpriteRenderer rend;
    public float fadeSpeed = 5f; // Vitesse de disparition

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        // Optionnel : teinter le fantôme en bleu/cyan comme sur l'image
        rend.color = new Color(0f, 0.8f, 1f, 0.5f);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // Boucle tant que l'alpha est supérieur à 0
        while (rend.color.a > 0)
        {
            Color objectColor = rend.color;
            // On réduit l'alpha progressivement
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
            objectColor.a = fadeAmount;
            rend.color = objectColor;
            yield return null; // Attendre la prochaine frame
        }
        // Une fois invisible, on détruit l'objet pour nettoyer la mémoire
        Destroy(gameObject);
    }
}