using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineZone : MonoBehaviour
{
    [Header("Machine points")]
    public Transform dropZone;
    public Transform spawnZone;

    [Header("Settings")]
    public GameObject processedPrefab;
    public float dropInterval = 0.1f;
    public float processTime = 1.0f;
    public float itemHeight = 0.5f;

    [Header("Stack Settings")]
    public int dropColumns = 2;
    public float columnSpacing = 0.6f;
    public int maxProcessedItmes = 10;

    private List<GameObject> dropItems = new List<GameObject>();
    private List<GameObject> processItems = new List<GameObject>();

    private PlayerStack currentPlayer;
    private Coroutine takeRoutine;

    private void Start()
    {
        StartCoroutine(ProcessRoutine());
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            currentPlayer = other.GetComponent<PlayerStack>();
            if(currentPlayer != null && takeRoutine == null)
            {
                takeRoutine = StartCoroutine(TakeFromPlayerRoutine());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(takeRoutine != null)
            {
                StopCoroutine(takeRoutine);
                takeRoutine = null;
            }
            currentPlayer = null;
        }
    }

    private IEnumerator TakeFromPlayerRoutine()
    {
        while(true)
        {
            if(currentPlayer != null && currentPlayer.HasItem())
            {
                GameObject item = currentPlayer.RemoveItem();
                item.transform.SetParent(dropZone);

                int index = dropItems.Count;
                int row = index / dropColumns;
                int col = index % dropColumns;

                float startX = -(dropColumns - 1) * columnSpacing / 2f;
                float targetX = startX + (col * columnSpacing);
                float targetY = row * itemHeight;

                Vector3 targetLocalPos = new Vector3(0, dropItems.Count * itemHeight, 0);
                dropItems.Add(item);

                StartCoroutine(FlyToPoint(item, targetLocalPos));
            }
            yield return new WaitForSeconds(dropInterval);
        }
    }
    private IEnumerator FlyToPoint(GameObject item, Vector3 targetLocalPos)
    {
        Vector3 startLocalPos = item.transform.localPosition;
        float time = 0f;
        float duration = 0.2f;
        while(time< duration)
        {
            time += Time.deltaTime;
            float percent = time / duration;

            Vector3 currentPos = Vector3.Lerp(startLocalPos, targetLocalPos, percent);
            currentPos.y += Mathf.Sin(percent * Mathf.PI) * 1.5f;
            item.transform.localPosition = currentPos;

            yield return null;
        }
        item.transform.localPosition = targetLocalPos;
    }
    private IEnumerator ProcessRoutine()
    {
        while(true)
        {
            if (dropItems.Count > 0 && processItems.Count < maxProcessedItmes)
            {
                yield return new WaitForSeconds(processTime);

                GameObject rawItem = dropItems[dropItems.Count - 1];
                dropItems.RemoveAt(dropItems.Count - 1);
                rawItem.SetActive(false);

                Vector3 spawnLocalPos = new Vector3(0, processItems.Count * itemHeight, 0);
                GameObject newProcessed = Instantiate(processedPrefab, spawnZone);
                newProcessed.transform.localPosition = spawnLocalPos;
                processItems.Add(newProcessed);
            }
            else
            {
                yield return null;
            }
        }
    }
}
