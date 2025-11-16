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
    public float qScaleLength = 1f;   // largo
    public float qScaleWidth = 1f;    // ancho
    public float qScaleHeight = 1f;   // grosor (eje Y en Mesh)

    [Header("Escala W y R")]
    public float wRadiusMultiplier = 1f;
    public float rRangeMultiplier = 1f;
    public float rImpactMultiplier = 1f;

    [Header("Offsets")]
    public float qForwardOffset = 1f;   // distancia desde la cola, NO SE TOCA

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

    // ----------------- SHIFT + Q/W/E/R -----------------

    public void OnAbilityQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Keyboard.current.shiftKey.isPressed)
            StartAiming(AbilityType.PrimaryAb);
    }
    public void OnAbilityW(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Keyboard.current.shiftKey.isPressed)
            StartAiming(AbilityType.SecondaryAb);
    }
    public void OnAbilityE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Keyboard.current.shiftKey.isPressed)
            StartAiming(AbilityType.ThirdAb);
    }
    public void OnAbilityR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Keyboard.current.shiftKey.isPressed)
            StartAiming(AbilityType.Ultimate);
    }

    // ----------------- INICIAR APUNTADO -----------------

    void StartAiming(AbilityType type)
    {
        if (isAiming) EndAiming();

        currentAbility = type;
        currentData = abilitySystem.abilities[type];
        isAiming = true;

        switch (type)
        {
            case AbilityType.PrimaryAb: // Q
                currentIndicator = Instantiate(qIndicatorPrefab);

                // buscar la cola
                qTailPivot = currentIndicator.transform.Find("Tail");

                // ESCALA del mesh
                Transform gfx = currentIndicator.transform.Find("GFX");
                if (gfx != null)
                {
                    float range = currentData.range;

                    gfx.localScale = new Vector3(
                        range * qScaleLength,   // largo
                        qScaleHeight,           // grosor
                        qScaleWidth             // ancho
                    );
                }
                break;

            case AbilityType.SecondaryAb: // W
                currentIndicator = Instantiate(wIndicatorPrefab, transform);
                currentIndicator.transform.localPosition = Vector3.zero;

                float wr = currentData.explosionRadius * wRadiusMultiplier * 2f;
                currentIndicator.transform.localScale = new Vector3(wr, wr, wr);
                break;

            case AbilityType.Ultimate: // R

                currentIndicator = Instantiate(rRangePrefab, transform);

                float rr = currentData.range * rRangeMultiplier * 2f;
                currentIndicator.transform.localScale = new Vector3(rr, rr, rr);

                currentIndicatorSecondary = Instantiate(rTargetPrefab);

                float ir = currentData.meteorRadius * rImpactMultiplier * 2f;
                currentIndicatorSecondary.transform.localScale = new Vector3(ir, ir, ir);
                break;
        }
    }

    // ----------------- UPDATE -----------------

    void Update()
    {
        if (!isAiming) return;

        if (!Keyboard.current.shiftKey.isPressed ||
            Mouse.current.leftButton.wasPressedThisFrame ||
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            EndAiming();
            return;
        }

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Vector3 point = hit.point;

        if (currentAbility == AbilityType.PrimaryAb) UpdateQ(point);
        if (currentAbility == AbilityType.Ultimate) UpdateR(point);

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            abilitySystem.TryCast(currentAbility);
            EndAiming();
        }
    }

    // ----------------- Q (NO SE MODIFICÓ LÓGICA, SOLO ESCALA) -----------------

    void UpdateQ(Vector3 point)
    {
        if (!currentIndicator) return;

        Vector3 playerPos = transform.position;

        Vector3 dir = point - playerPos;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion q = Quaternion.LookRotation(dir);
        q = Quaternion.Euler(0, q.eulerAngles.y, 0);
        currentIndicator.transform.rotation = q;

        Vector3 finalPos = playerPos + currentIndicator.transform.forward * qForwardOffset;
        finalPos.y = playerPos.y;

        currentIndicator.transform.position = finalPos;
    }

    // ----------------- R -----------------

    void UpdateR(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        dir.y = 0;

        float max = currentData.range;

        if (dir.magnitude > max)
            point = transform.position + dir.normalized * max;

        currentIndicatorSecondary.transform.position = point;
    }

    // ----------------- CANCELAR -----------------

    void EndAiming()
    {
        isAiming = false;
        currentAbility = AbilityType.None;
        currentData = null;

        if (currentIndicator) Destroy(currentIndicator);
        if (currentIndicatorSecondary) Destroy(currentIndicatorSecondary);

        qTailPivot = null;
    }
}
