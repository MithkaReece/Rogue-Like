using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReposController : MonoBehaviour
{
    private Sprite[] RingSprites;
    private SpriteRenderer RingRenderer;
    private EntityStats Stats;

    [SerializeField] protected float repos = 0f;
    private float reposCooldownCounter = 0f;

    //TODO: Make values pointers to the live values (as they may change)
    //TODO: Could just give reference to stat object then reference needed attributes
    public void Start()
    {
        RingSprites = Resources.LoadAll<Sprite>("Repos Ring");
        RingRenderer = GetComponent<SpriteRenderer>();
        Stats = transform.parent.gameObject.transform.parent.gameObject.GetComponent<EntityStats>();
    }

    public bool MaxRepos()
    {
        return Stats.Poise == repos;
    }

    public void Update()
    {
        if (Stats.Poise < 0)
        {
            Debug.Log("Negative Poise");
            return;
        }
        DecayRepos();

        int index = RingSprites.Length - 1;
        if (Stats.Poise > 0)
            index = (int)Mathf.Floor((RingSprites.Length-1) * repos / Stats.Poise);
        RingRenderer.sprite = RingSprites[index];
    }

    private void DecayRepos()
    {
        if (reposCooldownCounter <= 0f)
            repos = Mathf.Max(0, repos - Stats.ReposRegenSpeed * Time.deltaTime);
        reposCooldownCounter = Mathf.Max(0, reposCooldownCounter - Time.deltaTime);
    }

    public void AddRepos(float damage)
    {
        if (MaxRepos()) //Reset after damaging at max
            repos = 0f;
        else //Increase repos based on damage taken
            repos = Mathf.Min(repos + (float)damage, Stats.Poise);
        reposCooldownCounter = Stats.ReposCooldown;
    }
}
