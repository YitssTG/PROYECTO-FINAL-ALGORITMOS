using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;
public class Transicion : MonoBehaviour
{
    [SerializeField] private RectTransform ObjetoIzquierdo;
    [SerializeField] private RectTransform ObjetoDerecho;
    [SerializeField] private float PositionInitial;// fuerda de camara
    [SerializeField] private float PositionFinal;//dentro de camara
    [SerializeField] private float Duracion;
    [SerializeField] private Ease Modificador;
    public static Transicion Instance;
    public static event Action OnFinishOpenDoors;
    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    public void TransicionAbrirPuertas()
    {
        DOTween.Kill(ObjetoIzquierdo);
        DOTween.Kill(ObjetoDerecho);
        Sequence secuencia = DOTween.Sequence();
        secuencia.Append(ObjetoIzquierdo.DOAnchorPosX(-PositionInitial, Duracion).SetEase(Modificador));
        secuencia.Join(ObjetoDerecho.DOAnchorPosX(PositionInitial, Duracion).SetEase(Modificador));
        secuencia.OnComplete(CallEventOpen);
    }
    public void TransicionCerrarPuertas()
    {
        DOTween.Kill(ObjetoIzquierdo);
        DOTween.Kill(ObjetoDerecho);
        Sequence secuencia = DOTween.Sequence();
        secuencia.Append(ObjetoIzquierdo.DOAnchorPosX(PositionFinal, Duracion).SetEase(Modificador));
        secuencia.Join(ObjetoDerecho.DOAnchorPosX(PositionFinal, Duracion).SetEase(Modificador));
         StartCoroutine( WaitToopen());
    }
   IEnumerator WaitToopen()
    {
        yield return new WaitForSecondsRealtime(2f);
        TransicionAbrirPuertas();
    }
    public void CallEventOpen()
    {
        OnFinishOpenDoors?.Invoke();
    }
}
