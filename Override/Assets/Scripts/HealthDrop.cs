using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : MonoBehaviour
{
    Player player;
    SpriteRenderer healthSprite;

    void Start()
    {
        StartCoroutine(DespawnRoutine());
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        healthSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.currentHealth = player.maxHealth;
            Destroy(gameObject);
        }
    }

    IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(20);
        healthSprite.enabled = false;
        yield return new WaitForSeconds(1);
        healthSprite.enabled = true;
        yield return new WaitForSeconds(1);
        healthSprite.enabled = false;
        yield return new WaitForSeconds(1);
        healthSprite.enabled = true;
        yield return new WaitForSeconds(1);
        healthSprite.enabled = false;
        yield return new WaitForSeconds(1);
        healthSprite.enabled = true;
        yield return new WaitForSeconds(1);
        healthSprite.enabled = false;
        yield return new WaitForSeconds(1);
        healthSprite.enabled = true;
        yield return new WaitForSeconds(1);
        healthSprite.enabled = false;
        yield return new WaitForSeconds(1);
        healthSprite.enabled = true;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
