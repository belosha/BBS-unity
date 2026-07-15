using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MysteryEmotion : MonoBehaviour
{
    [Header("Visual")]
    public GameObject successVisual; // Голубая сфера (префаб)

    [Header("UI")]
    public GameObject interactionPrompt;    // Текст "[B] Взаимодействие"
    public GameObject skillCheckCanvas;     // Canvas с мини-игрой
    public RectTransform circleImage;       // Круг (RadialProgress) — RectTransform
    public Image successZoneImage;          // Жирная зона (сектор)
    public RectTransform sliderMarker;      // Ползунок (стрелка)

    [Header("Settings")]
    public float interactionRadius = 3f;    // Радиус взаимодействия (увеличил)
    public float skillCheckDuration = 2f;
    public float successZoneSize = 0.2f;    // Размер жирной зоны (20% круга)
    public float uiScale = 0.5f;            // Масштаб UI (уменьши, если большой)

    private Transform player;
    private bool isPerformingCheck = false;
    private bool isSuccess = false;
    private bool isCollected = false;

    private float currentAngle = 0f;
    private float successStartAngle;
    private float successEndAngle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        ShowMysteryVisual();

        // Выключаем весь UI при старте
        if (interactionPrompt) interactionPrompt.SetActive(false);
        if (skillCheckCanvas) skillCheckCanvas.SetActive(false);

        // Масштабируем UI
        if (skillCheckCanvas != null)
        {
            skillCheckCanvas.GetComponent<RectTransform>().localScale = Vector3.one * uiScale;
        }

        SetupSuccessZone();
    }

    void Update()
    {
        if (player == null) return;

        // Проверяем расстояние до игрока (вместо триггера)
        float distance = Vector3.Distance(transform.position, player.position);
        bool isPlayerInRange = distance <= interactionRadius;

        // Показываем/скрываем подсказку
        if (isPlayerInRange && !isPerformingCheck && !isCollected && !isSuccess)
        {
            if (interactionPrompt) interactionPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.B))
            {
                StartSkillCheck();
            }
        }
        else
        {
            if (interactionPrompt && !isPerformingCheck) interactionPrompt.SetActive(false);
        }

        // Скрываем подсказку во время мини-игры
        if (isPerformingCheck && interactionPrompt) interactionPrompt.SetActive(false);

        if (isPerformingCheck) UpdateSkillCheck();

        // Сбор после успеха
        if (isSuccess && !isCollected && isPlayerInRange)
        {
            Collect();
        }
    }

    void SetupSuccessZone()
    {
        float zoneAngle = 360f * successZoneSize;
        successStartAngle = Random.Range(0f, 360f - zoneAngle);
        successEndAngle = successStartAngle + zoneAngle;

        if (successZoneImage != null)
        {
            // Поворачиваем жирную зону на случайный угол
            successZoneImage.rectTransform.rotation = Quaternion.Euler(0, 0, -successStartAngle);
        }
    }

    void StartSkillCheck()
    {
        isPerformingCheck = true;
        currentAngle = 0f;

        // Показываем Canvas с мини-игрой
        skillCheckCanvas.SetActive(true);

        // Показываем жирную зону
        if (successZoneImage) successZoneImage.gameObject.SetActive(true);

        // Ставим ползунок на стартовую позицию
        UpdateSliderPosition();

        Debug.Log("Скилл-чек начат");
    }

    void UpdateSkillCheck()
    {
        // Обновляем угол
        currentAngle += 360f * (Time.deltaTime / skillCheckDuration);

        // Обновляем позицию ползунка
        UpdateSliderPosition();

        // Проверяем клик
        if (Input.GetMouseButtonDown(0))
        {
            float angle = currentAngle % 360f;
            bool isInZone = (angle >= successStartAngle && angle <= successEndAngle);

            if (isInZone)
            {
                SuccessSkillCheck();
            }
            else
            {
                FailSkillCheck();
            }
        }

        // Тайм-аут (провал через 2 секунды)
        if (currentAngle >= 360f)
        {
            FailSkillCheck();
        }
    }

    void UpdateSliderPosition()
    {
        if (sliderMarker == null) return;

        // Позиция на окружности (радиус = половина ширины круга)
        float radius = 100f * uiScale;
        float angleRad = currentAngle * Mathf.Deg2Rad;

        float x = Mathf.Sin(angleRad) * radius;
        float y = Mathf.Cos(angleRad) * radius;

        sliderMarker.anchoredPosition = new Vector2(x, y);

        // Поворачиваем стрелку по касательной к окружности
        sliderMarker.rotation = Quaternion.Euler(0, 0, currentAngle - 90f);
    }

    void SuccessSkillCheck()
    {
        isPerformingCheck = false;
        isSuccess = true;

        // Скрываем UI
        skillCheckCanvas.SetActive(false);
        if (interactionPrompt) interactionPrompt.SetActive(false);

        // Показываем голубую сферу
        ShowSuccessVisual();

        Debug.Log("Успех! Эмоция готова к сбору.");
    }

    void FailSkillCheck()
    {
        isPerformingCheck = false;

        // Скрываем UI
        skillCheckCanvas.SetActive(false);
        if (interactionPrompt) interactionPrompt.SetActive(false);

        // Запускаем анимацию исчезновения
        StartCoroutine(FadeAndDestroy());

        Debug.Log("Провал! Эмоция исчезает.");
    }

    IEnumerator FadeAndDestroy()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Color color = rend.material.color;
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                rend.material.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
        }
        Destroy(gameObject);
    }

    void ShowMysteryVisual()
    {
        gameObject.SetActive(true);
        if (successVisual != null) successVisual.SetActive(false);
    }

    void ShowSuccessVisual()
    {
        gameObject.SetActive(false);
        if (successVisual != null) successVisual.SetActive(true);
    }

    void Collect()
    {
        isCollected = true;
        Debug.Log("Эмоция собрана!");
        if (successVisual != null) Destroy(successVisual, 0.2f);
        Destroy(gameObject, 0.2f);
    }

    // Визуализация радиуса в редакторе (для удобства настройки)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}