using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3(0, 2, -4);  // Смещение от игрока
    public float mouseSensitivity = 500f;           // Чувствительность мыши
    public float verticalLimit = 80f;               // Ограничение поворота вверх/вниз

    private float xRotation = 0f;                   // Поворот по вертикали
    private float yRotation = 0f;                   // Поворот по горизонтали

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Блокируем курсор в центре экрана (опционально)
        Cursor.lockState = CursorLockMode.Locked;

        // Сохраняем начальный поворот камеры
        yRotation = transform.eulerAngles.y;
        xRotation = transform.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Получаем движение мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Вращение по горизонтали (влево-вправо)
        yRotation += mouseX;

        // Вращение по вертикали (вверх-вниз) с ограничением
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLimit, verticalLimit);

        // Применяем поворот к камере
        Quaternion cameraRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.rotation = cameraRotation;

        // Позиция: следуем за игроком + смещение, повёрнутое вместе с камерой
        Vector3 desiredPosition = player.transform.position +
                                  transform.rotation * offset;
        transform.position = desiredPosition;
    }
}
