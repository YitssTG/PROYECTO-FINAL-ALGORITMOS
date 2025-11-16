using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float range = 8f;
    public int damage;
    public float explosionRadius;
    private Rigidbody rb;
    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;

        Debug.Log($"[Fireball] Creada en {startPos} | speed={speed} | range={range} | damage={damage}");
        if (rb == null)
            Debug.LogError("[Fireball] NO tiene Rigidbody.");
    }

    void Update()
    {
        if (rb == null) return;

        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= range)
        {
            Debug.Log("[Fireball] Se destruye por alcance máximo.");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Fireball] OnTriggerEnter con {other.name}");

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            Debug.Log($"[Fireball] Golpea ENEMIGO, daño={damage}");
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
