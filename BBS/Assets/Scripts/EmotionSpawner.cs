using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmotionSpawner : MonoBehaviour
{
    public GameObject mysteryEmotionPrefab;
    public Transform[] spawnPoints;     // Точки спавна (создай пустые объекты)
    public int maxEmotions = 5;

    private List<GameObject> activeEmotions = new List<GameObject>();

    void Start()
    {
        SpawnRandomEmotions();
    }

    void SpawnRandomEmotions()
    {
        for (int i = 0; i < maxEmotions; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject emotion = Instantiate(mysteryEmotionPrefab, point.position, Quaternion.identity);
            activeEmotions.Add(emotion);
        }
    }

    // Вызывай, когда эмоцию собрали
    public void OnEmotionCollected(GameObject emotion)
    {
        activeEmotions.Remove(emotion);
        // Можно заспавнить новую через время
        StartCoroutine(RespawnEmotion());
    }

    IEnumerator RespawnEmotion()
    {
        yield return new WaitForSeconds(5f);
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject emotion = Instantiate(mysteryEmotionPrefab, point.position, Quaternion.identity);
        activeEmotions.Add(emotion);
    }
}