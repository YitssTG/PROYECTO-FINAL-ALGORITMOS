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

    private bool alreadySubscribed = false;

    IEnumerator Start()
    {
        Debug.Log("[Effects] Start()");

        if (alreadySubscribed)
        {
            Debug.LogWarning("[Effects] YA ESTABA SUSCRITO → evitando duplicado");
            yield break;
        }

        alreadySubscribed = true;

        if (abilitySystem == null)
            abilitySystem = GetComponent<AbilitySystem>();

        if (playerStats == null && GameManager.Instance != null)
            playerStats = GameManager.Instance.playerStats;

        if (cam == null)
            cam = Camera.main;

        float t = 0f;
        while ((abilitySystem == null || abilitySystem.abilities.Count == 0) && t < 5f)
        {
            t += Time.deltaTime;
            Debug.Log("[Effects] Esperando a AbilitySystem...");
            yield return null;
        }

        if (abilitySystem == null || abilitySystem.abilities.Count == 0)
        {
            Debug.LogError("[Effects] AbilitySystem no está listo, desactivando.");
            enabled = false;
            yield break;
        }

        Debug.Log("[Effects] AbilitySystem listo. Suscribiendo eventos…");

        if (abilitySystem.abilities.ContainsKey(AbilityType.PrimaryAb))
        {
            abilitySystem.abilities[AbilityType.PrimaryAb].OnCast += CastFireball;
            Debug.Log("[Effects] Suscrito a Q (PrimaryAb)");
        }

        if (abilitySystem.abilities.ContainsKey(AbilityType.SecondaryAb))
        {
            abilitySystem.abilities[AbilityType.SecondaryAb].OnCast += CastAura;
            Debug.Log("[Effects] Suscrito a W (SecondaryAb)");
        }

        if (abilitySystem.abilities.ContainsKey(AbilityType.ThirdAb))
        {
            abilitySystem.abilities[AbilityType.ThirdAb].OnCast += CastDash;
            Debug.Log("[Effects] Suscrito a E (ThirdAb)");
        }

        if (abilitySystem.abilities.ContainsKey(AbilityType.Ultimate))
        {
            abilitySystem.abilities[AbilityType.Ultimate].OnCast += CastMeteor;
            Debug.Log("[Effects] Suscrito a R (Ultimate)");
        }

        Debug.Log("[Effects] Suscripciones completadas.");
    }

    // ───────────────── FIREBALL ─────────────────
    void CastFireball()
    {
        Debug.Log("[Effects] CastFireball() llamado.");

        if (abilitySystem == null)
        {
            Debug.LogError("[Effects] abilitySystem es NULL.");
            return;
        }

        Ability ab = abilitySystem.abilities[AbilityType.PrimaryAb];

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

        // Ray hacia el mouse
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.Log("[Effects] Lanzando Raycast desde mouse…");

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.LogWarning("[Effects] Raycast NO golpeó nada.");
            return;
        }

        Debug.Log($"[Effects] Raycast hit → {hit.collider.name} en {hit.point}");

        // 🌟 DIRECCIÓN HORIZONTAL REAL
        Vector3 dir = hit.point - firePoint.position;
        dir.y = 0f;               // ← IGNORA ALTURA DEL TERRENO
        Vector3 direction = dir.normalized;

        Debug.Log("[Effects] Instanciando Fireball…");
        Vector3 spawnPos = firePoint.position + Vector3.down * 0.3f; // Baja un poco
        GameObject fb = Instantiate(fireballPrefab, spawnPos, Quaternion.LookRotation(direction));

        if (fb == null)
        {
            Debug.LogError("[Effects] Instantiate devolvió NULL.");
            return;
        }

        Debug.Log($"[Effects] Fireball instanciada → {fb.name} en {fb.transform.position}");

        FireballProjectile proj = fb.GetComponent<FireballProjectile>();
        if (proj == null)
        {
            Debug.LogError("[Effects] El prefab NO tiene FireballProjectile.");
            return;
        }

        proj.speed = ab.projectileSpeed;
        proj.range = ab.range;
        proj.damage = Mathf.RoundToInt(ab.damageBase + ab.damagePerLevel * ab.level);
        proj.explosionRadius = ab.explosionRadius;

        Debug.Log($"[Effects] Fireball configurada: speed={proj.speed}, range={proj.range}, damage={proj.damage}");
    }

    // ─────────── AURA (W) ───────────
    void CastAura()
    {
        Debug.Log("[Effects] CastAura() llamado.");

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

    // ─────────── DASH (E) ───────────
    void CastDash()
    {
        Ability ab = abilitySystem.abilities[AbilityType.ThirdAb];
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        // Ray del mouse al suelo
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        // Dirección solo en plano
        Vector3 dir = hit.point - transform.position;
        dir.y = 0f;

        // Si el mouse está muy cerca, no hacer dos movimientos
        if (dir.sqrMagnitude < 0.01f)
            return;

        float mouseDist = dir.magnitude;

        // UNA SOLA distancia final → evita el doble salto
        float finalDist = Mathf.Min(mouseDist, ab.dashDistance);

        // Punto final horizontal exacto
        Vector3 target = transform.position + dir.normalized * finalDist;
        target.y = transform.position.y;

        // Ajustar navmesh si es necesario
        if (NavMesh.SamplePosition(target, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
            target = navHit.position;

        // Teletransporte INSTANT, el único movimiento real
        agent.Warp(target);
    }
    // ─────────── METEORO (R) ───────────
    void CastMeteor()
    {
        Debug.Log("[Effects] CastMeteor() llamado.");

        Ability ab = abilitySystem.abilities[AbilityType.Ultimate];

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
        proj.damage = Mathf.RoundToInt(ab.damageBase + ab.damagePerLevel * ab.level);

        Debug.Log($"[Effects] Meteor configurado → target={impactPoint}, delay={proj.delay}, radius={proj.radius}, damage={proj.damage}");
    }
}
