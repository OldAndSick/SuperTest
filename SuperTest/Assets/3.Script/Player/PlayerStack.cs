using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStack : MonoBehaviour
{

    [Header("Stack Settings")]
    public Transform stackPoint;
    public float itemHeight = 0.5f;
    public float columnDepth = 0.5f;
    public int maxTypes = 2;
    public float flyDuration = 0.3f;

    private List<string> activeTypes = new List<string>();
    private Dictionary<string, List<GameObject>> stackDict = new Dictionary<string, List<GameObject>>();

    public bool CanAddItem(string itemTag)
    {
        if (activeTypes.Contains(itemTag)) return true;
        if (activeTypes.Count < maxTypes) return true;
        return false;
    }
    public void AddItem(GameObject item)
    {
        string tag = item.tag;
        if (!stackDict.ContainsKey(tag))
        {
            stackDict[tag] = new List<GameObject>();
            if(!activeTypes.Contains(tag)) activeTypes.Add(tag);
        }

            //int colIndex = activeTypes.IndexOf(tag);
            int rowIndex = stackDict[tag].Count;

            //Vector2 targetLocalPos = new Vector3(0, rowIndex * itemHeight, -colIndex * columnDepth);

            Quaternion targetLocalRot = Quaternion.identity;
            if (tag == "RawCarrot") targetLocalRot = Quaternion.Euler(90f, 0f, 0f);
            else targetLocalRot = Quaternion.identity;

            stackDict[tag].Add(item);
            item.transform.SetParent(stackPoint);

            //StartCoroutine(FlyToBag(item, targetLocalPos, targetLocalRot));
        
    }
    private IEnumerator FlyToBag(GameObject item, Vector3 targetLocalPos, Quaternion targetLocalRot)
    {
        Vector3 startLocalPos = item.transform.localPosition;
        Quaternion startLocalRot = item.transform.localRotation;

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

    public bool HasItem(string tag)
    {
        return stackDict.ContainsKey(tag) && stackDict[tag].Count > 0;
    }
    public GameObject RemoveItem(string tag)
    {
        if (HasItem(tag))
        {
            var list = stackDict[tag];
            GameObject item = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);

            if(list.Count == 0)
            {
                stackDict.Remove(tag);
                activeTypes.Remove(tag);
            }
            return item;
        }
        return null;
    }
}
