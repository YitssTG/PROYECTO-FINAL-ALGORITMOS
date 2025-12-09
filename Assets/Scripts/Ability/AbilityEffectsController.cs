using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Collections;

public class AbilityEffectsController : MonoBehaviour
{
    [Header("Referencias")]
    public AbilitySystem abilitySystem;
    public PlayerStats playerStats;
    public Camera cam;
    public Transform firePoint;

    [Header("Prefabs")]
    public GameObject fireballPrefab;
    public GameObject auraPrefab;
    public GameObject meteorPrefab;

    private void Start()
    {
        if (abilitySystem != null)
        {
            abilitySystem.OnAbilityCast += OnAbilityCast;
        }
        else
        {
            Debug.LogError("AbilitySystem no asignado en EffectsController");
        }

        if (cam == null) cam = Camera.main;
        if (playerStats == null && GameManager.Instance != null)
            playerStats = GameManager.Instance.playerStats;
    }

    private void OnDestroy()
    {
        if (abilitySystem != null)
        {
            abilitySystem.OnAbilityCast -= OnAbilityCast;
        }
    }

    private void OnAbilityCast(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.PrimaryAb:
                CastFireball();
                break;
            case AbilityType.SecondaryAb:
                CastAura();
                break;
            case AbilityType.ThirdAb:
                CastDash();
                break;
            case AbilityType.Ultimate:
                CastMeteor();
                break;
        }
    }

    void CastFireball()
    {
        if (abilitySystem == null)
        {
            Debug.LogError("[Effects] abilitySystem es NULL.");
            return;
        }

        Ability ab = abilitySystem.abilities[AbilityType.PrimaryAb];
        int damage = abilitySystem.GetCalculatedDamage(AbilityType.PrimaryAb);

        if (fireballPrefab == null)
        {
            Debug.LogError("[Effects] fireballPrefab es NULL.");
            return;
        }
        if (firePoint == null)
        {
            Debug.LogError("[Effects] firePoint es NULL.");
            return;
        }
        if (cam == null)
        {
            Debug.LogError("[Effects] cam es NULL.");
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.LogWarning("[Effects] Raycast NO golpeó nada.");
            return;
        }

        Vector3 dir = hit.point - firePoint.position;
        dir.y = 0f;
        Vector3 direction = dir.normalized;

        Vector3 spawnPos = firePoint.position + Vector3.down * 0.3f;
        GameObject fb = Instantiate(fireballPrefab, spawnPos, Quaternion.LookRotation(direction));

        if (fb == null)
        {
            Debug.LogError("[Effects] Instantiate devolvió NULL.");
            return;
        }

        FireballProjectile proj = fb.GetComponent<FireballProjectile>();
        if (proj == null)
        {
            Debug.LogError("[Effects] El prefab NO tiene FireballProjectile.");
            return;
        }

        proj.speed = ab.projectileSpeed;
        proj.range = ab.range;
        proj.damage = damage;
        proj.explosionRadius = ab.explosionRadius;
    }

    void CastAura()
    {
        Ability ab = abilitySystem.abilities[AbilityType.SecondaryAb];

        if (auraPrefab != null)
        {
            GameObject aura = Instantiate(auraPrefab, transform.position, Quaternion.identity, transform);
            aura.transform.localPosition = Vector3.zero;

            aura.transform.localScale = Vector3.zero;
            aura.transform
                .DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    aura.transform
                        .DOScale(1.2f, 0.4f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
                });

            Destroy(aura, ab.duration);
        }

        if (playerStats != null)
            StartCoroutine(InvulnerabilityRoutine(ab.duration));
    }

    IEnumerator InvulnerabilityRoutine(float duration)
    {
        Debug.Log("[Effects] Invulnerable ON");
        playerStats.isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        playerStats.isInvulnerable = false;
        Debug.Log("[Effects] Invulnerable OFF");
    }

    void CastDash()
    {
        Ability ab = abilitySystem.abilities[AbilityType.ThirdAb];
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        Vector3 dir = hit.point - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f)
            return;

        float mouseDist = dir.magnitude;
        float finalDist = Mathf.Min(mouseDist, ab.dashDistance);
        Vector3 target = transform.position + dir.normalized * finalDist;
        target.y = transform.position.y;

        if (NavMesh.SamplePosition(target, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
            target = navHit.position;

        agent.Warp(target);
    }

    void CastMeteor()
    {
        Ability ab = abilitySystem.abilities[AbilityType.Ultimate];
        int damage = abilitySystem.GetCalculatedDamage(AbilityType.Ultimate);

        if (meteorPrefab == null || cam == null)
        {
            Debug.LogWarning("[Effects] Meteor: falta prefab o cam.");
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.LogWarning("[Effects] Meteor: raycast no golpea nada.");
            return;
        }

        Vector3 dir = (hit.point - transform.position);
        float dist = Mathf.Min(dir.magnitude, ab.range);
        Vector3 impactPoint = transform.position + dir.normalized * dist;

        Vector3 spawnPos = impactPoint + Vector3.up * 25f;

        GameObject m = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        MeteorProjectile proj = m.GetComponent<MeteorProjectile>();

        if (proj == null)
        {
            Debug.LogError("[Effects] El prefab de meteor NO tiene MeteorProjectile.");
            return;
        }

        proj.targetPosition = impactPoint;
        proj.delay = ab.meteorDelay;
        proj.radius = ab.meteorRadius;
        proj.damage = damage;
    }
}