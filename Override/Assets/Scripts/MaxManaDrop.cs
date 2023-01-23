using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxManaDrop : MonoBehaviour
{
    Player player;
    SpriteRenderer maxManaSprite;

    void Start()
    {
        StartCoroutine(DespawnRoutine());
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        maxManaSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.ActivateMaxMana();
            Destroy(gameObject);
        }
    }

    IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(20);
        maxManaSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxManaSprite.enabled = true;
        yield return new WaitForSeconds(1);
        maxManaSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxManaSprite.enabled = true;
        yield return new WaitForSeconds(1);
        maxManaSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxManaSprite.enabled = true;
        yield return new WaitForSeconds(1);
        maxManaSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxManaSprite.enabled = true;
        yield return new WaitForSeconds(1);
        maxManaSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxManaSprite.enabled = true;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
