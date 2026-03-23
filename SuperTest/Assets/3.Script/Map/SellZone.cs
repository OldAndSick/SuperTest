using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellZone : MonoBehaviour
{
    [Header("Sell settings")]
    public Transform counterPoint;
    public Transform moneyPoint;
    public float itemHeight = 0.5f;

    [Header("Prefab")]
    public GameObject moneyPrefab;

    [Header("Money Stacking Settings")]
    public float moneyHeight = 0.1f;
    public float moneySpacingX = 0.6f;
    public float moneySpacingZ = 0.8f;

    private List<GameObject> counterItems = new List<GameObject>();
    private List<GameObject> moneyList = new List<GameObject>();
    private PlayerStack currentPlayer;
    private Coroutine takeRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            currentPlayer = other.GetComponent<PlayerStack>();
            if(currentPlayer != null && takeRoutine == null)
            {
                //takeRoutine = StartCoroutine(TakeFromPlayerRoutine());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(takeRoutine != null )
            {
                StopCoroutine(takeRoutine);
                takeRoutine = null;
                currentPlayer = null;
            }
        }
    }
    //private IEnumerator TakeFromPlayerRoutine()
    //{
    //    while(true)
    //    {
    //        if(currentPlayer != null && currentPlayer.HasItem())
    //        {
    //            GameObject item = currentPlayer.RemoveItem();
    //            item.transform.SetParent(counterPoint);
    //
    //            Vector3 targetLocalPos = new Vector3(0, counterItems.Count * itemHeight, 0);
    //            counterItems.Add(item);
    //            StartCoroutine(FlyToPoint(item, targetLocalPos));
    //        }
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}
    private IEnumerator FlyToPoint(GameObject item, Vector3 targetLocalPos)
    {
        Vector3 startLocalPos = item.transform.localPosition;
        float time = 0f;
        float duration = 0.2f;
        while(time < duration)
        {
            time += Time.deltaTime;
            Vector3 currentPos = Vector3.Lerp(startLocalPos, targetLocalPos, time / duration);
            currentPos.y += Mathf.Sin((time / duration) * Mathf.PI) * 1.5f;
            item.transform.localPosition = currentPos;
            yield return null;
        }
        item.transform.localPosition = targetLocalPos;
    }
    public bool TryTakeCarrot()
    {
        if(counterItems.Count > 0)
        {
            int lastIdx = counterItems.Count - 1;
            GameObject carrot = counterItems[lastIdx];
            counterItems.RemoveAt(lastIdx);

            carrot.SetActive(false);
            return true;
        }
        return false;
    }
    public void DropMoney(int buyCarrotCount)
    {
        int billsToDrop = buyCarrotCount * 2;
        for(int i = 0; i<billsToDrop; i++)
        {
            int index = moneyList.Count;
            int layer = index / 6;
            int indexInLayer = index % 6;

            int col = indexInLayer % 2;
            int row = indexInLayer / 2;

            float x = (col - 0.5f) * moneySpacingX;
            float z = (row - 1.0f) * moneySpacingZ;
            float y = layer * moneyHeight;

            Vector3 targetLocalPos = new Vector3(x, y, z);
            GameObject money = Instantiate(moneyPrefab, moneyPoint);
            moneyList.Add(money);

            StartCoroutine(FlyToPoint(money, targetLocalPos));
        }
    }
}
