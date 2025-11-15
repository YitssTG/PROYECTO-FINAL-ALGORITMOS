using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("📌 Camera Settings")]
    [Tooltip("Velocidad de movimiento de la cámara en modo libre")]
    [Range(1.0f, 50.0f)]
    public float movementSpeed = 10f;

    [Tooltip("Ángulo vertical de la cámara (grados)")]
    [Range(10.0f, 80.0f)]
    public float angle = 45f;

    [Header("📌 Rotación Manual (solo inspector)")]
    [Tooltip("Gira la cámara alrededor del jugador sin usar teclado/mouse")]
    public float rotationOffset = 0f;

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

    // 🔹 Movimiento por bordes, estilo LoL (solo en plano XZ)
    private void MoveCamera()
    {
        Vector3 mp = Input.mousePosition;
        int w = Screen.width;
        int h = Screen.height;

        // direcciones planas (sin subir/bajar la cámara)
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 move = Vector3.zero;

        // izquierda
        if (mp.x < w * hScreenPercentage)
            move -= right;

        // derecha
        if (mp.x > w - w * hScreenPercentage)
            move += right;

        // abajo
        if (mp.y < h * vScreenPercentage)
            move -= forward;

        // arriba
        if (mp.y > h - h * vScreenPercentage)
            move += forward;

        if (move.sqrMagnitude > 0.001f)
            transform.position += move.normalized * movementSpeed * Time.deltaTime;
    }

    // 🔹 Zoom con la rueda, usando tus parámetros distance/min/max
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // viejo Input System

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // igual que tu versión original
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    // 🔹 Seguir al player usando angle + distance + rotationOffset
    public void CenterAtPlayer()
    {
        if (player == null) return;

        float angleRad = Mathf.Deg2Rad * (90 - angle);

        float y = Mathf.Cos(angleRad) * distance;
        float z = Mathf.Sin(angleRad) * distance;

        Vector3 baseOffset = new Vector3(0, y, -z);

        // aplicamos la rotación manual de la cámara
        Quaternion rot = Quaternion.Euler(0f, rotationOffset, 0f);
        Vector3 rotatedOffset = rot * baseOffset;

        transform.position = player.position + rotatedOffset;
        transform.LookAt(player);
    }
}
