using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JugadorMov : MonoBehaviour
{
    public float moveSpeed = 10f; // Velocidad de movimiento
    public Camera mainCamera;     // Cámara principal
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform firePoint;         // Punto de salida del disparo
    public float projectileSpeed = 80f; // Velocidad del proyectil
    public float fireRate = 0.2f; // Tiempo entre disparos en segundos
    private float fireTimer = 0f;
    public GameObject strongProjectilePrefab; // Prefab de la bala especial
    public float strongProjectileSpeed = 100f; // Velocidad de la bala especial
    public float strongFireRate = 0.1f; // Tiempo entre disparos especiales
    private float strongFireTimer = 0f;
    public RawImage strongMissileImage;

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleShooting();         // Autoshoot
        HandleStrongShooting();   // Disparo especial
    }

    void HandleMovement()
    {
        // Movimiento en plano XZ (top-down estilo)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void HandleRotation()
    {
        // Obtener el punto donde el mouse apunta en el mundo
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 direction = (hitPoint - transform.position).normalized;

            // Rotar suavemente hacia el punto del mouse
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 20f * Time.deltaTime);
        }
    }

    void HandleShooting()
    {
        if (strongMissileImage != null)
        {
            strongMissileImage.enabled = strongFireTimer >= strongFireRate;
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            // Instanciar el proyectil en el firePoint mirando hacia adelante
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Asignar velocidad al proyectil
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * projectileSpeed;
            }

            fireTimer = 0f; // Reinicia el temporizador
        }
    }
    void HandleStrongShooting()
    {
        strongFireTimer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && strongFireTimer >= strongFireRate)
        {
            GameObject projectile = Instantiate(strongProjectilePrefab, firePoint.position, firePoint.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * strongProjectileSpeed;
            }

            strongFireTimer = 0f; // Reinicia el temporizador especial
        }
    }
}
