using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxAmmo : MonoBehaviour
{
    Player player;
    SpriteRenderer maxAmmoSprite;

    void Start()
    {
        StartCoroutine(DespawnRoutine());
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        maxAmmoSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.MaxAmmo();
            Destroy(gameObject);
        }
    }

    IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(20);
        maxAmmoSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxAmmoSprite.enabled = true;
        yield return new WaitForSeconds(1);
        maxAmmoSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxAmmoSprite.enabled = true;
        yield return new WaitForSeconds(1);
        maxAmmoSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxAmmoSprite.enabled = true;
        yield return new WaitForSeconds(1);
        maxAmmoSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxAmmoSprite.enabled = true;
        yield return new WaitForSeconds(1);
        maxAmmoSprite.enabled = false;
        yield return new WaitForSeconds(1);
        maxAmmoSprite.enabled = true;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
