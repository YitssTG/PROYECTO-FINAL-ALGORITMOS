using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityAimingSystem : MonoBehaviour
{
    [Header("Referencias")]
    public AbilitySystem abilitySystem;
    public Camera cam;

    [Header("Prefabs")]
    public GameObject qIndicatorPrefab;
    public GameObject wIndicatorPrefab;
    public GameObject rRangePrefab;
    public GameObject rTargetPrefab;

    [Header("Escala Q (solo visual)")]
    public float qScaleLength = 1f;
    public float qScaleWidth = 1f;
    public float qScaleHeight = 1f;

    [Header("Escala W y R")]
    public float wRadiusMultiplier = 1f;
    public float rRangeMultiplier = 1f;
    public float rImpactMultiplier = 1f;

    [Header("Offsets")]
    public float qForwardOffset = 1f;

    [Header("Altura indicadores")]
    public float indicatorsY = 0.05f;

    private GameObject currentIndicator;
    private GameObject currentIndicatorSecondary;
    private Transform qTailPivot;
    private AbilityType currentAbility = AbilityType.None;
    private Ability currentData;
    private bool isAiming = false;
    public bool IsAiming => isAiming;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    public void OnAbilityQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Keyboard.current.shiftKey.isPressed && !isAiming)
            StartAiming(AbilityType.PrimaryAb);
    }

    public void OnAbilityW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Keyboard.current.shiftKey.isPressed && !isAiming)
            StartAiming(AbilityType.SecondaryAb);
    }

    public void OnAbilityE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            abilitySystem.TryCast(AbilityType.ThirdAb);
    }

    public void OnAbilityR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Keyboard.current.shiftKey.isPressed && !isAiming)
            StartAiming(AbilityType.Ultimate);
    }

    void StartAiming(AbilityType type)
    {
        if (isAiming) EndAiming();

        if (!abilitySystem.abilities.ContainsKey(type)) return;

        currentAbility = type;
        currentData = abilitySystem.abilities[type];
        isAiming = true;

        switch (type)
        {
            case AbilityType.PrimaryAb:
                currentIndicator = Instantiate(qIndicatorPrefab);
                qTailPivot = currentIndicator.transform.Find("Tail");
                Transform gfx = currentIndicator.transform.Find("GFX");
                if (gfx != null)
                {
                    float range = currentData.range;
                    gfx.localScale = new Vector3(range * qScaleLength, qScaleHeight, qScaleWidth);
                }
                currentIndicator.transform.position = new Vector3(transform.position.x, indicatorsY, transform.position.z);
                break;

            case AbilityType.SecondaryAb:
                currentIndicator = Instantiate(wIndicatorPrefab, transform);
                float wr = currentData.explosionRadius * wRadiusMultiplier * 2f;
                currentIndicator.transform.localScale = new Vector3(wr, wr, wr);
                currentIndicator.transform.position = new Vector3(transform.position.x, indicatorsY, transform.position.z);
                break;

            case AbilityType.Ultimate:
                currentIndicator = Instantiate(rRangePrefab, transform);
                float rr = currentData.range * rRangeMultiplier * 2f;
                currentIndicator.transform.localScale = new Vector3(rr, rr, rr);
                currentIndicator.transform.position = new Vector3(transform.position.x, indicatorsY, transform.position.z);

                currentIndicatorSecondary = Instantiate(rTargetPrefab);
                float ir = currentData.meteorRadius * rImpactMultiplier * 2f;
                currentIndicatorSecondary.transform.localScale = new Vector3(ir, ir, ir);
                currentIndicatorSecondary.transform.position = new Vector3(transform.position.x, indicatorsY, transform.position.z);
                break;
        }
    }

    void Update()
    {
        if (!isAiming) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            EndAiming();
            return;
        }

        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, indicatorsY, 0));
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        float dist;
        if (!groundPlane.Raycast(ray, out dist))
            return;

        Vector3 point = ray.GetPoint(dist);

        if (currentAbility == AbilityType.PrimaryAb)
            UpdateQ(point);
        else if (currentAbility == AbilityType.Ultimate)
            UpdateR(point);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            bool launched = abilitySystem.TryCast(currentAbility);
            if (launched)
                EndAiming();
            return;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            EndAiming();
        }
    }

    void UpdateQ(Vector3 point)
    {
        if (!currentIndicator) return;

        Vector3 playerPos = transform.position;
        playerPos.y = indicatorsY;

        Vector3 dir = point - playerPos;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion q = Quaternion.LookRotation(dir);
        q = Quaternion.Euler(0, q.eulerAngles.y, 0);
        currentIndicator.transform.rotation = q;

        Vector3 finalPos = playerPos + currentIndicator.transform.forward * qForwardOffset;
        finalPos.y = indicatorsY;
        currentIndicator.transform.position = finalPos;
    }

    void UpdateR(Vector3 point)
    {
        Vector3 playerPos = transform.position;
        playerPos.y = indicatorsY;

        Vector3 dir = point - playerPos;
        dir.y = 0;

        float max = currentData.range;
        if (dir.magnitude > max)
            point = playerPos + dir.normalized * max;

        currentIndicatorSecondary.transform.position = new Vector3(point.x, indicatorsY, point.z);
    }

    void EndAiming()
    {
        isAiming = false;
        currentAbility = AbilityType.None;
        currentData = null;

        if (currentIndicator) Destroy(currentIndicator);
        if (currentIndicatorSecondary) Destroy(currentIndicatorSecondary);

        currentIndicator = null;
        currentIndicatorSecondary = null;
        qTailPivot = null;
    }
}