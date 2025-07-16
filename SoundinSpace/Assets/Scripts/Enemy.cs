using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f;

    private Transform player;
    private float fireTimer = 0f;

    public ParticleSystem hitEffect;

    public int health = 10;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // Seguir al jugador
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Rotar hacia el jugador
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        // Disparar al jugador
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * projectileSpeed;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (hitEffect != null)
            {
                ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                effect.Play();
            }

            health--;
        }

        if (collision.gameObject.CompareTag("ProjStrong"))
        {
            if (hitEffect != null)
            {
                ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                effect.Play();
            }

            health -= 5;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
