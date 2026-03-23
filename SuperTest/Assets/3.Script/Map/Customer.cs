using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;

    [Header("Points")]
    public Transform shopPoint;
    public Transform endPoint;
    public SellZone targetShop;

    [Header("Shopping Settings")]
    public int wantedAmount;
    public float dealInterval = 0.5f;

    private CustomerSpawner spawner;
    private int boughtAmount;
    private Transform counterPoint;
    private Transform entryPoint;
    private Transform stableCenter;

    public void Init(SellZone shop, Transform shopPt, Transform entryPt, Transform stableCenterPt, CustomerSpawner mySpawner)
    {
        targetShop = shop;
        counterPoint = shopPt;
        entryPoint = entryPt;
        stableCenter = stableCenterPt;
        spawner = mySpawner;

        boughtAmount = 0;

        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        wantedAmount = Random.Range(2, 4);
        yield return StartCoroutine(MoveToPoint(shopPoint.position));
        targetShop.DropMoney(boughtAmount);

        yield return StartCoroutine(MoveToPoint(entryPoint.position));

        
        yield return StartCoroutine(LookAtPoint(stableCenter.position));

        
        Vector2 randomCircle = Random.insideUnitCircle * 3f; // in stable pos setting
        Vector3 finalStablePos = stableCenter.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        yield return StartCoroutine(MoveToPoint(finalStablePos));
    }

    private IEnumerator MoveToPoint(Vector3 targetPos)
    {
        while(Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
    private IEnumerator LookAtPoint(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;
        if(direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            while(Quaternion.Angle(transform.rotation, targetRot) > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                yield return null;
            }
            transform.rotation = targetRot;
        }
    }
}
