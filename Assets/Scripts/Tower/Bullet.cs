using UnityEngine;

public class Bullet : MonoBehaviour
{
    private EnemyBase target;
    private float speed = 15f;
    private int damage;

    public void SetTarget(EnemyBase newTarget, int dmg)
    {
        target = newTarget;
        damage = dmg;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
