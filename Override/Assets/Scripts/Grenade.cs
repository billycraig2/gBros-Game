using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float delay = 2f;
    [SerializeField] float damage = 100f;
    [SerializeField] float splashRange = 20f;
    AudioSource explosionSound;

    void Start()
    {
        explosionSound = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine(Explode());        
    }

    public IEnumerator Explode()
    {
        yield return new WaitForSeconds(delay);
        explosionSound.Play();
        if (splashRange > 0)
        {
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, splashRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Robot"))
                {
                    var enemy = hitCollider.GetComponent<Robot>();
                    if (enemy)
                    {
                        var closestPoint = hitCollider.ClosestPoint(transform.position);
                        var distance = Vector3.Distance(closestPoint, transform.position);

                        var damagePercent = Mathf.InverseLerp(splashRange, 0, distance);
                        enemy.currentHealth -= damage;
                    }
                }
                else if (hitCollider.CompareTag("RobotShooter"))
                {
                    var enemy = hitCollider.GetComponent<RobotShooter>();
                    if (enemy)
                    {
                        var closestPoint = hitCollider.ClosestPoint(transform.position);
                        var distance = Vector3.Distance(closestPoint, transform.position);

                        var damagePercent = Mathf.InverseLerp(splashRange, 0, distance);
                        enemy.currentHealth -= damage;
                    }
                }
            }
        }
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
