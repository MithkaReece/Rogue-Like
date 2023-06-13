using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HealthRingController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] ringSprites;
    private EntityStats entityStats;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ringSprites = Resources.LoadAll<Sprite>("Health Ring");
        entityStats = GetComponentInParent<EntityStats>();
        entityStats.OnHealthChanged += UpdateHealthRing; //Subscribe to event
    }


    void UpdateHealthRing()
    {
        float HealthLeft = 100 * entityStats.CurrentHealth / entityStats.MaxHealth;
        int[] Stages = { 90, 75, 55, 45, 30, 20, 10, 5, 0 };
        int index = Stages.Count(s => s >= HealthLeft);
        spriteRenderer.sprite = ringSprites[index];
    }

    void OnDestroy()
    {
        entityStats.OnHealthChanged -= UpdateHealthRing; //Unsubscribe event
    }
}
