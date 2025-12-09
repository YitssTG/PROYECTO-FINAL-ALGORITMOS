using UnityEngine;
using DG.Tweening;

public class IndicatorFX : MonoBehaviour
{
    public Transform arrowN;
    public Transform arrowS;
    public Transform arrowE;
    public Transform arrowW;

    public float separation = 0.70f;
    public float tilt = 65f;        
    public float lift = 0.22f;    
    public float duration = 0.45f;

    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack);

        PositionArrow(arrowN, 0);
        PositionArrow(arrowE, 90);
        PositionArrow(arrowS, 180);
        PositionArrow(arrowW, 270);

        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in srs)
            sr.DOFade(0, 0.35f).SetDelay(0.25f);

        Destroy(gameObject, 0.7f);
    }

    void PositionArrow(Transform arrow, float angle)
    {
        if (arrow == null) return;

        Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * separation;

        arrow.localPosition = offset + Vector3.up * lift;

        Vector3 toCenter = (Vector3.zero - offset).normalized;
        Quaternion lookRot = Quaternion.LookRotation(toCenter, Vector3.up);

        arrow.localRotation = lookRot * Quaternion.Euler(tilt, 0, 0);
    }
}
