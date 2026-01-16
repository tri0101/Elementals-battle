
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectFlyingSpawnPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private List<GameObject> poolObject;
    private Transform holder;
    public static ObjectFlyingSpawnPoint instance;
    private void Awake()
    {
        instance = this;
        holder = transform.GetChild(1);
        AddPrefabs();
    }
    private void AddPrefabs()
    {
        foreach (Transform chill in transform.GetChild(0))
        {
            spawnPoints.Add(chill.gameObject);
        }
    }

    public GameObject BrowseList(string nameObject)
    {
        foreach (Transform chill in transform.GetChild(0))
        {
            if (chill.name == nameObject)
            {
                return chill.gameObject;
            }

        }
        return null;
    }
    private GameObject GetObjectFromPool(GameObject gameObject)
    {
        foreach (GameObject obj in poolObject)
        {
            if (obj.name == gameObject.name)
            {
                poolObject.Remove(obj);
                return obj;
            }
        }
        GameObject tmpSpawwn = Instantiate(gameObject);
        tmpSpawwn.name = gameObject.name;
        return tmpSpawwn;
    }

    public void SpawnObjectAtPosition(string objName, Transform heroSpawnPos, string heroSpawnTag)
    {
        GameObject objSpawn = BrowseList(objName);
        if (objSpawn == null)
        {
            Debug.LogWarning("Object không có trong list");
            return;
        }
        ObjectFlyingController obc = objSpawn.GetComponent<ObjectFlyingController>();
        Vector3 spawnPoint = obc.ObjectFlyingSO.spawnPosition;
        GameObject tmpSpawwn = GetObjectFromPool(objSpawn);
        tmpSpawwn.tag = heroSpawnTag;
        tmpSpawwn.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.layer = LayerMask.NameToLayer(heroSpawnTag);

        //Tính giá trị cuối cùng của obj khi spawn ra với y default 
        Vector3 finalSpawnPoint = new Vector3(0, 0, 0);
        if (heroSpawnPos.parent.localScale.x > 0)
        {
            finalSpawnPoint = heroSpawnPos.position + new Vector3(spawnPoint.x, 0, 0);
        }
        else
        {
            finalSpawnPoint = heroSpawnPos.position + new Vector3(-spawnPoint.x, 0, 0);

        }
        finalSpawnPoint.y = heroSpawnPos.position.y + spawnPoint.y;
        tmpSpawwn.transform.position = finalSpawnPoint;
        //Tính giá trị scale 
        tmpSpawwn.transform.localScale = new Vector3(heroSpawnPos.parent.localScale.x > 0 ? 1 : -1, 1, 1);

        tmpSpawwn.SetActive(true);
        tmpSpawwn.transform.SetParent(holder);
    }
    public void AddToPool(GameObject obj)
    {
        poolObject.Add(obj);
        obj.SetActive(false);
    }
    public void AddToPoolTimer(GameObject obj, float timer)
    {
        StartCoroutine(AddToPoolCoroutine(obj, timer));
    }
    private IEnumerator AddToPoolCoroutine(GameObject obj, float timer)
    {
        yield return new WaitForSeconds(timer);
        AddToPool(obj);
    }
}
