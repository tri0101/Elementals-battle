using System.Collections;
using UnityEngine;

public class Effect_Fire : MonoBehaviour
{
    [SerializeField] private Transform listEffect;

    [Header("Timing")]
    [SerializeField] private float intervalSeconds = 1f;
    [SerializeField] private float activeDurationSeconds = 0.5f;

    private Coroutine loopRoutine;

    private void Awake()
    {
        if (listEffect == null && transform.childCount > 0)
            listEffect = transform.GetChild(0);
    }

    private void OnEnable()
    {
        listEffect.gameObject.SetActive(true);
        loopRoutine = StartCoroutine(CoLoopChildren());
    }

    private void OnDisable()
    {
        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }
  
        SetAllChildrenActive(false);
       
    }

    private IEnumerator CoLoopChildren()
    {
        if (listEffect == null)
            yield break;

        if (listEffect.childCount == 0)
            yield break;

        // 1) bắt đầu: tắt hết children
        SetAllChildrenActive(false);

        while (true)
        {
            // 2) duyệt từng con
            for (int i = 0; i < listEffect.childCount; i++)
            {
                yield return new WaitForSeconds(intervalSeconds);

                Transform child = listEffect.GetChild(i);
                if (child == null) continue;

                child.gameObject.SetActive(true);

                yield return new WaitForSeconds(activeDurationSeconds);

                if (child != null)
                    child.gameObject.SetActive(false);
            }
        }
    }

    private void SetAllChildrenActive(bool active)
    {
        if (listEffect == null) return;

        for (int i = 0; i < listEffect.childCount; i++)
        {
            Transform child = listEffect.GetChild(i);
            if (child != null)
                child.gameObject.SetActive(active);
        }
    }
}