﻿using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("📌 Camera Settings")]
    [Tooltip("Velocidad de movimiento de la cámara en modo libre")]
    [Range(1.0f, 50.0f)]
    public float movementSpeed = 10f;

    [Tooltip("Ángulo vertical de la cámara (grados)")]
    [Range(10.0f, 80.0f)]
    public float angle = 45f;

    [Header("📌 Zoom Settings")]
    [Tooltip("Distancia inicial de la cámara al jugador")]
    public float distance = 12f;

    [Tooltip("Distancia mínima permitida (zoom máximo)")]
    public float minDistance = 5f;

    [Tooltip("Distancia máxima permitida (zoom mínimo)")]
    public float maxDistance = 25f;

    [Tooltip("Velocidad de zoom con la rueda del mouse")]
    [Range(1.0f, 20.0f)]
    public float zoomSpeed = 5f;

    [Header("📌 Edge Scrolling")]
    [Tooltip("Porcentaje de pantalla que activa el desplazamiento en modo libre")]
    [Range(0.01f, 0.3f)]
    public float hScreenPercentage = 0.1f;
    public float vScreenPercentage = 0.1f;

    [Header("📌 Player Reference")]
    [Tooltip("Referencia al jugador (arrastrar en el Inspector)")]
    public Transform player;

    private bool freeMode = false; 

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        CenterAtPlayer();
    }

    void FixedUpdate()
    {
        HandleZoom();

        if (freeMode)
            MoveCamera(); 
        else
            CenterAtPlayer(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            freeMode = !freeMode; 
            if (!freeMode)
                CenterAtPlayer();
        }
    }

    private void MoveCamera()
    {
        Vector3 mp = Input.mousePosition;
        int w = Screen.width;
        int h = Screen.height;

        if (mp.x < w * hScreenPercentage)
            transform.position -= new Vector3(1, 0, 0) * movementSpeed * Time.deltaTime;
        else if (mp.x > w - w * hScreenPercentage)
            transform.position += new Vector3(1, 0, 0) * movementSpeed * Time.deltaTime;

        if (mp.y < h * vScreenPercentage)
            transform.position -= new Vector3(0, 0, 1) * movementSpeed * Time.deltaTime;
        else if (mp.y > h - h * vScreenPercentage)
            transform.position += new Vector3(0, 0, 1) * movementSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    public void CenterAtPlayer()
    {
        if (player == null) return;

        float angleRad = Mathf.Deg2Rad * (90 - angle);

        float y = Mathf.Cos(angleRad) * distance;
        float z = Mathf.Sin(angleRad) * distance;

        transform.position = player.position + new Vector3(0, y, -z);
        transform.LookAt(player);
    }
}