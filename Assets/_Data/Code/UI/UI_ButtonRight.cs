using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_ButtonRight : MonoBehaviour
{
    public Transform ListMenu;
    public float duration = 2f;
    public Image backButton;

    bool isOpen = false;
    bool isAnimating = false;   // 🔒 khóa animation
    Coroutine animCo;

    public void AnimationList()
    {
        // ⛔ đang animate thì không cho bấm
        if (isAnimating) return;

        if (animCo != null)
            StopCoroutine(animCo);

        float from = isOpen ? 1f : 0f;
        float to = isOpen ? 0f : 1f;

        if (isOpen)
            SetValueBColor(1f);
        else
            SetValueBColor(87f / 255f);

        animCo = StartCoroutine(ScaleList(from, to));
        isOpen = !isOpen;
    }

    void SetValueBColor(float value)
    {
        Color c = backButton.color;
        c.b = value;
        backButton.color = c;
    }

    IEnumerator ScaleList(float from, float to)
    {
        isAnimating = true;   // 🔒 khóa

        float time = 0f;
        Vector3 startScale = new Vector3(1, from, 1);
        Vector3 endScale = new Vector3(1, to, 1);

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            ListMenu.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        ListMenu.localScale = endScale;
        isAnimating = false;  // 🔓 mở khóa
    }
}
