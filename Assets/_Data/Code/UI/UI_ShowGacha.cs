using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShowGacha : MonoBehaviour
{
    public Transform prefabHeroSample;
    public Transform prefabHeroShardSample;
    public HeroDatabase heroDatabase;

    public void OnClickRoll()
    {
        ClearOldCards();
        GachaResult result = GachaManager.Instance.Roll();
        ShowResult(result);
    }

    public void OnClickRollTen()
    {
        ClearOldCards();

        List<GachaResult> results = new();
        for (int i = 0; i < 10; i++)
            results.Add(GachaManager.Instance.Roll());

        foreach (var result in results)
            ShowResult(result);
    }

    void ClearOldCards()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    void ShowResult(GachaResult result)
    {
        HeroInfo hero = heroDatabase.GetHero(result.heroId);
        if (hero == null) return;

        Transform item = Instantiate(
            result.type == GachaResultType.Hero
                ? prefabHeroSample
                : prefabHeroShardSample
        );

        // set parent trước
        item.SetParent(transform, false);
        item.gameObject.SetActive(true);

        //// icon CHƯA gán
        //Image icon = item.Find("HeroIcon").GetComponent<Image>();
        //icon.enabled = false;

        //// bắt đầu hiệu ứng xoay
        //StartCoroutine(RotateFastY(item, icon, hero.iconFace));
        Image icon = item.Find("HeroIcon").GetComponent<Image>();
        Image light = item.Find("LightOverlay").GetComponent<Image>();

        StartCoroutine(
            RotateFastWithLight(item, icon, light, hero.iconFace)
        );
    }

    IEnumerator RotateFastY(Transform card, Image icon, Sprite sprite)
    {
        float currentY = 0f;

        int rounds = 1;                    // số vòng xoay
        float targetY = 360f * rounds;     // tổng độ cần xoay

        float rotateSpeed = 2000f;         // độ / giây (cực nhanh)

        // ẩn icon trong lúc xoay
        icon.enabled = false;

        while (currentY < targetY)
        {
            currentY += rotateSpeed * Time.deltaTime;

            if (currentY > targetY)
                currentY = targetY;

            card.localRotation = Quaternion.Euler(0f, currentY, 0f);
            yield return null;
        }

        // reset rotation cho đẹp
        card.localRotation = Quaternion.identity;

        // hiện icon sau khi xoay xong
        icon.sprite = sprite;
        icon.enabled = true;
    }
    IEnumerator RotateFastWithLight(
    Transform card,
    Image icon,
    Image light,
    Sprite sprite
)
    {
        float currentY = 0f;

        int rounds = 2;
        float targetY = 180f * rounds;
        float rotateSpeed = 1000f;

        icon.enabled = false;
        light.color = new Color(1, 1, 1, 0);
        light.enabled = true;

        bool revealed = false;

        while (currentY < targetY)
        {
            currentY += rotateSpeed * Time.deltaTime;

            if (currentY > targetY)
                currentY = targetY;

            card.localRotation = Quaternion.Euler(0f, currentY, 0f);

            // vòng cuối → bật flash
            if (!revealed && currentY > targetY - 180f)
            {
                revealed = true;

                // bật ánh sáng
                StartCoroutine(FlashLight(light));

                // hiện icon
                icon.sprite = sprite;
                icon.enabled = true;
            }

            yield return null;
        }

        card.localRotation = Quaternion.identity;
    }
    IEnumerator RotateFastWithBlinkLight(
    Transform card,
    Image icon,
    Image light,
    Sprite sprite
)
    {
        float currentY = 0f;

        int rounds = 5;
        float targetY = 360f * rounds;
        float rotateSpeed = 2200f;

        icon.enabled = false;
        light.enabled = true;

        bool revealed = false;

        while (currentY < targetY)
        {
            currentY += rotateSpeed * Time.deltaTime;
            if (currentY > targetY)
                currentY = targetY;

            card.localRotation = Quaternion.Euler(0f, currentY, 0f);

            // ánh sáng chớp trong lúc quay
            float blink = Mathf.PingPong(Time.time * 6f, 1f); // tốc độ chớp
            light.color = new Color(1f, 1f, 1f, blink * 0.6f);

            // vòng cuối → reveal
            if (!revealed && currentY > targetY - 180f)
            {
                revealed = true;

                // flash mạnh
                StartCoroutine(FlashStrong(light));

                icon.sprite = sprite;
                icon.enabled = true;
            }

            yield return null;
        }

        card.localRotation = Quaternion.identity;
        light.enabled = false;
    }

    IEnumerator FlashStrong(Image light)
    {
        float t = 0f;
        float duration = 0.25f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0.6f, 1f, t / duration);
            light.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / duration);
            light.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }
    }

    IEnumerator FlashLight(Image light)
    {
        float t = 0f;
        float duration = 0.4f;

        // fade in
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 0.8f, t / duration);
            light.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        // fade out
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0.8f, 0f, t / duration);
            light.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        light.enabled = false;
    }

}
