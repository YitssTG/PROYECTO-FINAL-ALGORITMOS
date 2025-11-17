using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float range = 8f;
    public int damage;
    public float explosionRadius;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 🌟 Giro suave en su propio eje (natural)
        transform.Rotate(Vector3.forward * 720f * Time.deltaTime);

        float step = speed * Time.deltaTime;
        Vector3 nextPos = transform.position + transform.forward * step;

        // 🌟 SphereCast – RAYCAST ANCHO
        float radius = 0.4f; // ← Cambia esto para hacerlo más “gordo”

        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, step))
        {
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            if (!hit.collider.isTrigger)
            {
                Destroy(gameObject);
                return;
            }
        }

        // Movimiento final
        transform.position = nextPos;

        // Destruir al llegar al rango
        if (Vector3.Distance(startPos, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }
}
