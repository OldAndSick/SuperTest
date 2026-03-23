using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStack : MonoBehaviour
{

    [Header("Stack Settings")]
    public Transform stackPoint;
    public float itemHeight = 0.5f;
    public float flyDuration = 0.3f;

    private List<GameObject> stackItems = new List<GameObject>();

    public void AddItem(GameObject itemPrefab)
    {
        Vector3 targetLocalPos = new Vector3(0, stackItems.Count * itemHeight, 0);
        stackItems.Add(itemPrefab);
        itemPrefab.transform.SetParent(stackPoint);

        StartCoroutine(FlyToBag(itemPrefab, targetLocalPos));
    }
    private IEnumerator FlyToBag(GameObject item, Vector3 targetLocalPos)
    {
        Vector3 startLocalPos = item.transform.localPosition;
        Quaternion startLocalRot = item.transform.localRotation;
        Quaternion targetLocalRot = Quaternion.Euler(90f, 0f, 0f);

        float time = 0f;

        while(time<flyDuration)
        {
            time += Time.deltaTime;
            float percent = time / flyDuration;
            Vector3 currentPos = Vector3.Lerp(startLocalPos, targetLocalPos, percent);

            currentPos.y += Mathf.Sin(percent * Mathf.PI) * 5f; // sin -> make goksun

            item.transform.localPosition = currentPos;
            item.transform.localRotation = Quaternion.Slerp(startLocalRot, targetLocalRot, percent);

            yield return null;
        }

        item.transform.localPosition = targetLocalPos;
        item.transform.localRotation = targetLocalRot;

    }

    public bool HasItem()
    {
        return stackItems.Count > 0;
    }
    public GameObject RemoveItem()
    {
        if (stackItems.Count == 0) return null;

        int lastIndex = stackItems.Count - 1;
        GameObject itemToGive = stackItems[lastIndex];
        stackItems.RemoveAt(lastIndex);

        return itemToGive;
    }
}
