using UnityEngine;
using DG.Tweening;

public class MeteorProjectile : MonoBehaviour
{
    public Vector3 targetPosition;
    public float delay = 1.5f;
    public float radius = 4f;
    public int damage = 100;

    // capa de enemigos (pon la Layer correcta en el inspector)
    public LayerMask enemyLayer;

    void Start()
    {
        transform.DOMove(targetPosition + Vector3.up * 0.5f, delay)
            .SetEase(Ease.InQuad)
            .OnComplete(Explode);
    }

    void Explode()
    {
        Debug.Log($"[Meteor] Explode en {targetPosition} | radius={radius}");

        Collider[] hits = Physics.OverlapSphere(targetPosition, radius, enemyLayer);

        Debug.Log($"[Meteor] Enemigos encontrados: {hits.Length}");

        foreach (var hit in hits)
        {
            EnemyBase eb = hit.GetComponent<EnemyBase>();
            if (eb != null)
            {
                Debug.Log($"[Meteor] DAÑO a {hit.name} por {damage}");
                eb.TakeDamage(damage);
            }
        }

        // TODO: instanciar VFX de explosión si quieres

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, radius);
    }
}