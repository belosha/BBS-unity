using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;
    private Transform cameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        rb.freezeRotation = true;
    }

    void Update()
    {
        // Получаем ввод
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Направление камеры (только горизонтальная плоскость)
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Вектор движения относительно камеры
        Vector3 moveDirection = (camForward * vertical + camRight * horizontal).normalized;

        // Применяем скорость
        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        // Если есть движение — поворачиваем персонажа в сторону движения (плавно)
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Отправляем скорость в аниматор
        float speedMagnitude = new Vector3(horizontal, 0, vertical).magnitude;
        if (animator != null)
        {
            animator.SetFloat("Speed", speedMagnitude);
        }
    }
}