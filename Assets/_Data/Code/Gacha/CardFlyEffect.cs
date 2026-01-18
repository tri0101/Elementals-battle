using System.Collections;
using UnityEngine;

public class CardFlyEffect : MonoBehaviour
{
    public float flyTime = 0.4f;
    public AnimationCurve curve;

    public IEnumerator Fly(RectTransform from, RectTransform to)
    {
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 startPos = from.position;
        Vector3 endPos = to.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / flyTime;
            float curveValue = curve.Evaluate(t);

            rect.position = Vector3.Lerp(startPos, endPos, curveValue);
            yield return null;
        }

        rect.position = endPos;
    }
}
