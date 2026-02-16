using DG.Tweening;
using UnityEngine;
using UnityEngine.UI; // Indispensable pour modifier les Images
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class HealthBarUI : MonoBehaviour
{
    [Header("Réglages visuels")]
    public GameObject segmentPrefab;   // Ton prefab de barre blanche
    public Color fullColor = Color.red;
    public Color emptyColor = Color.orange;

    private List<Image> segmentImages = new List<Image>();

    // Appelé au Start par Health.cs
    public void SetupHearts(int maxHealth)
    {
        // On nettoie les anciens segments
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        segmentImages.Clear();

        // On crée les segments
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject newSegment = Instantiate(segmentPrefab, transform);
            Image img = newSegment.GetComponent<Image>();
            if (img != null)
            {
                segmentImages.Add(img);
                img.color = fullColor;
            }
        }
    }

    // Appelé à chaque dégât par Health.cs
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < segmentImages.Count; i++) // A PATCH 
        {
            // Si l'index est inférieur à la vie actuelle, segment plein
            if (i < currentHealth)
            {
                segmentImages[i].color = fullColor;
                // Optionnel : on peut aussi laisser le segment actif
                // segmentImages[i].gameObject.SetActive(true);
            }
            else
            {
                // Le segment devient gris (ou on peut le désactiver)
                var tween = segmentImages[i].transform.DOScale(Vector3.one * 0, .5f)
                    .From(Vector3.one)
                    .SetEase(Ease.InElastic)
                    .OnComplete(() => segmentImages[i].color = emptyColor);
                // segmentImages[i].gameObject.SetActive(false); 
            }
        }
    }


}