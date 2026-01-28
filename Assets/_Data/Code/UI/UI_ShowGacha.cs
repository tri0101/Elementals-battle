using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UI_ShowGacha : MonoBehaviour
{
    public Transform prefabHeroSample;
    public Transform prefabHeroShardSample;
    public HeroDatabase heroDatabase;
    public Transform panelGacha;
    public bool panelIsActive;
    private GridLayoutGroup grid;

    private bool isShowing = false; 
    public bool IsShowing => isShowing;

    /* ================= BUTTON ================= */
    void Awake()
    {
        grid = panelGacha.GetComponent<GridLayoutGroup>();
    }
    public void OnClickRoll()
    {
        if (isShowing || panelIsActive) return;

        panelIsActive = true;
        panelGacha.parent.gameObject.SetActive(true);
        ClearOldCards();

        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.padding = new RectOffset(0, 0, 0, 0);

        StartCoroutine(RollOne());
    }


    public void OnClickRollTen()
    {
        if (isShowing || panelIsActive) return;

        panelIsActive = true;
        panelGacha.parent.gameObject.SetActive(true);
        ClearOldCards();

        grid.childAlignment = TextAnchor.UpperLeft;
        grid.padding = new RectOffset(200, 150, 40, 100);

        StartCoroutine(RollTen());
    }



    IEnumerator RollOne()
    {
        isShowing = true;

        IGachaService gacha = GachaManager.Instance;
        GachaResult result = gacha.Roll();
        yield return ShowResult(result);

        isShowing = false;
    }

    IEnumerator RollTen()
    {
        isShowing = true;
        IGachaService gacha = GachaManager.Instance;
        for (int i = 0; i < 10; i++)
        {
            
            GachaResult result = gacha.Roll();
            yield return ShowResult(result);

            yield return new WaitForSeconds(0.1f);
        }

        isShowing = false;
    }

    void ClearOldCards()
    {
        foreach (Transform child in panelGacha)
            Destroy(child.gameObject);
    }



    IEnumerator ShowResult(GachaResult result)
    {
        HeroInfo hero = heroDatabase.GetHero(result.heroId);
        if (hero == null) yield break;

       

        Transform item = Instantiate(
            result.type == GachaResultType.Hero
                ? prefabHeroSample
                : prefabHeroShardSample
        );

        item.SetParent(panelGacha, false);
        item.gameObject.SetActive(true);

        Image icon = item.Find("HeroIcon").GetComponent<Image>();
        Image light = item.Find("LightOverlay").GetComponent<Image>();

        yield return RevealWithLight(icon, light, hero.iconFace);
    }

 

    IEnumerator RevealWithLight(Image icon, Image light, Sprite sprite)
    {
        icon.enabled = false;
        light.enabled = true;
        light.color = new Color(1f, 1f, 1f, 0f);

        float t = 0f;
        float duration = 0.07f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / duration);
            light.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        icon.sprite = sprite;
        icon.enabled = true;

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / duration);
            light.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        light.enabled = false;
    }
    private void Update()
    {
        if (panelIsActive && !isShowing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                panelIsActive = false;
                panelGacha.parent.gameObject.SetActive(false);
            }
        }
    }

}
