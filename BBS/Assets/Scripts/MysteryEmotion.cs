using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MysteryEmotion : MonoBehaviour
{
    [Header("Visual")]
    public GameObject successVisual; // Голубая сфера

    [Header("UI")]
    public GameObject interactionPrompt;
    public GameObject skillCheckCanvas;
    public Image radialProgress;
    public Image successZoneImage;
    public RectTransform sliderMarker;

    [Header("Settings")]
    public float skillCheckDuration = 2f;
    public float successZoneSize = 0.2f;

    private bool isPlayerNear = false;
    private bool isPerformingCheck = false;
    private bool isSuccess = false;
    private bool isCollected = false;

    private float currentAngle = 0f;
    private float successStartAngle;
    private float successEndAngle;

    void Start()
    {
        ShowMysteryVisual();

        if (interactionPrompt) interactionPrompt.SetActive(false);
        if (skillCheckCanvas) skillCheckCanvas.SetActive(false);

        SetupSuccessZone();
    }

    void Update()
    {
        if (isPlayerNear && !isPerformingCheck && !isCollected)
        {
            if (interactionPrompt) interactionPrompt.SetActive(true);
            if (Input.GetKeyDown(KeyCode.B)) StartSkillCheck();
        }
        else
        {
            if (interactionPrompt && !isPerformingCheck) interactionPrompt.SetActive(false);
        }

        if (isPerformingCheck) UpdateSkillCheck();
    }

    void SetupSuccessZone()
    {
        float zoneAngle = 360f * successZoneSize;
        successStartAngle = Random.Range(0f, 360f - zoneAngle);
        successEndAngle = successStartAngle + zoneAngle;

        if (successZoneImage != null)
        {
            successZoneImage.rectTransform.rotation = Quaternion.Euler(0, 0, -successStartAngle);
        }
    }

    void StartSkillCheck()
    {
        isPerformingCheck = true;
        currentAngle = 0f;

        skillCheckCanvas.SetActive(true);
        interactionPrompt.SetActive(false);

        if (successZoneImage) successZoneImage.gameObject.SetActive(true);
        if (radialProgress) radialProgress.fillAmount = 0f;

        UpdateSliderPosition();
    }

    void UpdateSkillCheck()
    {
        float progress = currentAngle / 360f;
        if (radialProgress) radialProgress.fillAmount = progress;

        currentAngle += 360f * (Time.deltaTime / skillCheckDuration);
        UpdateSliderPosition();

        if (currentAngle >= 360f)
        {
            FailSkillCheck();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            float angle = currentAngle % 360f;
            bool isInZone = (angle >= successStartAngle && angle <= successEndAngle);

            if (isInZone) SuccessSkillCheck();
            else FailSkillCheck();
        }
    }

    void UpdateSliderPosition()
    {
        if (sliderMarker == null) return;

        float angleRad = currentAngle * Mathf.Deg2Rad;
        float radius = 100f;
        float x = Mathf.Sin(angleRad) * radius;
        float y = Mathf.Cos(angleRad) * radius;
        sliderMarker.anchoredPosition = new Vector2(x, y);
    }

    void SuccessSkillCheck()
    {
        isPerformingCheck = false;
        isSuccess = true;
        skillCheckCanvas.SetActive(false);
        ShowSuccessVisual();
        Debug.Log("Успех!");
    }

    void FailSkillCheck()
    {
        isPerformingCheck = false;
        isSuccess = false;
        skillCheckCanvas.SetActive(false);
        StartCoroutine(FadeAndDestroy());
        Debug.Log("Провал!");
    }

    IEnumerator FadeAndDestroy()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Color color = rend.material.color;
            float elapsed = 0f;
            float duration = 1f;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isSuccess && !isCollected && other.CompareTag("Player")) Collect();
    }

    void Collect()
    {
        isCollected = true;
        if (successVisual != null) Destroy(successVisual, 0.5f);
        Destroy(gameObject, 0.5f);
    }
}