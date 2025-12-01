using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public float lifeTime = 3f;

    // ⭐ NUEVO: Para seguir al objetivo si se mueve
    private Transform target;
    private bool hasTarget = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (hasTarget && target != null)
        {
            // ⭐ SEGUIR al objetivo si está asignado
            Vector3 direction = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            // ⭐ MOVIMIENTO normal si no hay objetivo
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    // ⭐ NUEVO: Método para asignar objetivo
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        hasTarget = true;

        // Apuntar hacia el objetivo inmediatamente
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerStats.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

         //⭐ NUEVO: Dañar torretas
        TowerHealth tower = other.GetComponent<TowerHealth>();
        if (tower != null)
        {
            tower.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}