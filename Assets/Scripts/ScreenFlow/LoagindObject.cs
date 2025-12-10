using DG.Tweening;
using UnityEngine;

public class LoagindObject : MonoBehaviour
{
    public float rangoMovimiento = 50f;        
    public float tiempoMovimiento = 1.5f;       
    public float escalaLatido = 1.1f;           
    public float duracionLatido = 0.5f;         

    private RectTransform rectTransform;
    private Vector2 posicionInicial;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();posicionInicial = rectTransform.anchoredPosition;MoverAleatorio();Latir();
    }
    void MoverAleatorio()
    {
        Vector2 destino = posicionInicial + new Vector2(Random.Range(-rangoMovimiento, rangoMovimiento),Random.Range(-rangoMovimiento, rangoMovimiento));
        rectTransform.DOAnchorPos(destino, tiempoMovimiento).SetEase(Ease.InOutSine).OnComplete(MoverAleatorio); 
    }
    void Latir()
    {
        rectTransform.DOScale(escalaLatido, duracionLatido).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo); 
    }
}
