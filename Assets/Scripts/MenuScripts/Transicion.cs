using UnityEngine;
using DG.Tweening;
using System;

public class Transicion : MonoBehaviour
{
    [SerializeField] private RectTransform ObjetoIzquierdo;
    [SerializeField] private RectTransform ObjetoDerecho;
    [SerializeField] private float PositionInitial; // fuera de cámara
    [SerializeField] private float PositionFinal;   // centro (cerrado)
    [SerializeField] private float Duracion;
    [SerializeField] private Ease Modificador;

    public static Transicion Instance;
    public static event Action OnFinishOpenDoors;    // se dispara cuando termina de abrir
    public static event Action OnFinishCloseDoors;   // se dispara cuando termina de cerrar

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        // Inicia con las puertas cerradas en el centro
        ObjetoIzquierdo.anchoredPosition = new Vector2(-PositionFinal, ObjetoIzquierdo.anchoredPosition.y);
        ObjetoDerecho.anchoredPosition = new Vector2(PositionFinal, ObjetoDerecho.anchoredPosition.y);

        // Abrir automáticamente al inicio
        TransicionAbrirPuertas();
    }

    public void TransicionAbrirPuertas()
    {
        DOTween.Kill(ObjetoIzquierdo);
        DOTween.Kill(ObjetoDerecho);

        Sequence secuencia = DOTween.Sequence();
        secuencia.Append(ObjetoIzquierdo.DOAnchorPosX(-PositionInitial, Duracion).SetEase(Modificador));
        secuencia.Join(ObjetoDerecho.DOAnchorPosX(PositionInitial, Duracion).SetEase(Modificador));
        secuencia.OnComplete(() => OnFinishOpenDoors?.Invoke());
    }

    public void TransicionCerrarPuertas()
    {
        DOTween.Kill(ObjetoIzquierdo);
        DOTween.Kill(ObjetoDerecho);

        Sequence secuencia = DOTween.Sequence();
        secuencia.Append(ObjetoIzquierdo.DOAnchorPosX(-PositionFinal, Duracion).SetEase(Modificador));
        secuencia.Join(ObjetoDerecho.DOAnchorPosX(PositionFinal, Duracion).SetEase(Modificador));
        secuencia.OnComplete(() => OnFinishCloseDoors?.Invoke());
    }
}
