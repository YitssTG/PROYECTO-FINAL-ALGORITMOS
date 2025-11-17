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
    public float qForwardOffset = 1f;   // distancia desde la cola

    [Header("Altura indicadores")]
    public float indicatorsY = 0.05f;   // altura fija para TODOS los indicadores

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

    // ──────────────────────────────────────────────
    // INPUTS: SHIFT + Q/W/E/R → ENTRAR A MODO APUNTADO
    // ──────────────────────────────────────────────

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
        // La E NO usa modo de apuntado
        // Si presionas SHIFT + E → no apuntar, castear normal
        if (ctx.performed)
            abilitySystem.TryCast(AbilityType.ThirdAb);
    }

    public void OnAbilityR(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Keyboard.current.shiftKey.isPressed && !isAiming)
            StartAiming(AbilityType.Ultimate);
    }

    // ──────────────────────────────────────────────
    // ENTRAR A MODO APUNTADO
    // ──────────────────────────────────────────────

    void StartAiming(AbilityType type)
    {
        if (isAiming) EndAiming();

        currentAbility = type;
        currentData = abilitySystem.abilities[type];
        isAiming = true;

        switch (type)
        {
            // ------------- Q (flecha) -------------
            case AbilityType.PrimaryAb:
                currentIndicator = Instantiate(qIndicatorPrefab);

                qTailPivot = currentIndicator.transform.Find("Tail");

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

                currentIndicator.transform.position = new Vector3(
                    transform.position.x,
                    indicatorsY,
                    transform.position.z
                );
                break;

            // ------------- W (radio en el player) -------------
            case AbilityType.SecondaryAb:
                currentIndicator = Instantiate(wIndicatorPrefab, transform);

                float wr = currentData.explosionRadius * wRadiusMultiplier * 2f;
                currentIndicator.transform.localScale = new Vector3(wr, wr, wr);

                currentIndicator.transform.position = new Vector3(
                    transform.position.x,
                    indicatorsY,
                    transform.position.z
                );
                break;

            // ------------- E (por ahora sin indicador) -------------
            case AbilityType.ThirdAb:
                // si luego quieres un indicador, se agrega aquí
                break;

            // ------------- R (radio máximo + target) -------------
            case AbilityType.Ultimate:

                // círculo grande (rango máximo)
                currentIndicator = Instantiate(rRangePrefab, transform);

                float rr = currentData.range * rRangeMultiplier * 2f;
                currentIndicator.transform.localScale = new Vector3(rr, rr, rr);

                currentIndicator.transform.position = new Vector3(
                    transform.position.x,
                    indicatorsY,
                    transform.position.z
                );

                // círculo pequeño (punto donde cae)
                currentIndicatorSecondary = Instantiate(rTargetPrefab);

                float ir = currentData.meteorRadius * rImpactMultiplier * 2f;
                currentIndicatorSecondary.transform.localScale = new Vector3(ir, ir, ir);

                currentIndicatorSecondary.transform.position = new Vector3(
                    transform.position.x,
                    indicatorsY,
                    transform.position.z
                );
                break;
        }
    }

    // ──────────────────────────────────────────────
    // UPDATE MODO APUNTADO
    // ──────────────────────────────────────────────

    void Update()
    {
        if (!isAiming) return;

        // ESC cancela sin mover
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            EndAiming();
            return;
        }

        // Plano horizontal a altura fija (no usa colliders)
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, indicatorsY, 0));
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        float dist;
        if (!groundPlane.Raycast(ray, out dist))
            return;

        Vector3 point = ray.GetPoint(dist);

        // Actualizar indicadores según habilidad
        if (currentAbility == AbilityType.PrimaryAb)
            UpdateQ(point);
        else if (currentAbility == AbilityType.Ultimate)
            UpdateR(point);
        // W no necesita update: es solo radio alrededor del player
        // E por ahora sin indicador

        // ───── CONFIRMAR (CLICK IZQUIERDO) ─────
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            bool launched = abilitySystem.TryCast(currentAbility);

            // SOLO cerrar si realmente lanzó
            if (launched)
                EndAiming();

            return;
        }

        // ───── CANCELAR (CLICK DERECHO) ─────
        // Aquí NO bloqueamos movimiento: PlayerController se encargará
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            EndAiming();
            // no hacemos return para no interferir con otros sistemas
        }
    }

    // ──────────────────────────────────────────────
    // Q → FLECHA
    // ──────────────────────────────────────────────

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

    // ──────────────────────────────────────────────
    // R → rango máximo + punto de impacto
    // ──────────────────────────────────────────────

    void UpdateR(Vector3 point)
    {
        Vector3 playerPos = transform.position;
        playerPos.y = indicatorsY;

        Vector3 dir = point - playerPos;
        dir.y = 0;

        float max = currentData.range;
        if (dir.magnitude > max)
            point = playerPos + dir.normalized * max;

        currentIndicatorSecondary.transform.position = new Vector3(
            point.x,
            indicatorsY,
            point.z
        );
    }

    // ──────────────────────────────────────────────
    // SALIR DE MODO APUNTADO
    // ──────────────────────────────────────────────

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
