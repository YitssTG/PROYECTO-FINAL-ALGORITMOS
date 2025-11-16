using DG.Tweening;
using UnityEngine;

public class MeteorProjectile : MonoBehaviour
{
    public Vector3 targetPosition;
    public float delay = 1.5f;
    public float radius = 4f;
    public int damage = 100;

    void Start()
    {
        // Caída con DOTween
        transform.DOMove(targetPosition + Vector3.up * 0.5f, delay)
            .SetEase(Ease.InQuad)
            .OnComplete(Explode);
    }

    void Explode()
    {
        // Daño en área usando EnemyManager
        foreach (var mov in EnemyManager.Enemigos)
        {
            if (mov == null) continue;
            float dist = Vector3.Distance(targetPosition, mov.transform.position);
            if (dist <= radius)
            {
                EnemyBase eb = mov.GetComponent<EnemyBase>();
                if (eb != null)
                    eb.TakeDamage(damage);
            }
        }

        // Aquí podrías instanciar un prefab de explosión (VFX)

        Destroy(gameObject);
    }
}
