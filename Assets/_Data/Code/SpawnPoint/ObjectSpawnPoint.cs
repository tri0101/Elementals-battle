
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UIElements;

public class ObjectSpawnPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private List<GameObject> poolObject;
    private Transform holder;
    public static ObjectSpawnPoint instance;
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
        foreach(GameObject obj in poolObject)
        {
            if(obj.name == gameObject.name)
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
        if(objSpawn == null)
        {
            Debug.LogWarning("Object không có trong list");
            return;
        }
        ObjectSpawnPointController obc = objSpawn.GetComponent<ObjectSpawnPointController>();
        Vector3 spawnPoint = obc.ObjectSpawnPointSO.spawnPosition;
        GameObject tmpSpawwn = GetObjectFromPool(objSpawn);
        tmpSpawwn.tag = heroSpawnTag;
        tmpSpawwn.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.layer = LayerMask.NameToLayer(heroSpawnTag);

        //Tính giá tr? cu?i cùng c?a obj khi spawn ra v?i y default 
        Vector3 finalSpawnPoint = new Vector3(0, 0, 0);
        if (heroSpawnPos.parent.localScale.x > 0)
        {
            finalSpawnPoint = heroSpawnPos.position + new Vector3(spawnPoint.x, 0, 0);
        }
        else
        {
            finalSpawnPoint = heroSpawnPos.position + new Vector3(-spawnPoint.x, 0, 0);

        }
        finalSpawnPoint.y = spawnPoint.y;
        tmpSpawwn.transform.position = finalSpawnPoint;


        tmpSpawwn.SetActive(true);
        tmpSpawwn.transform.SetParent(holder);
    }



    public void AddToPool(GameObject obj)
    {
        poolObject.Add(obj);
        obj.SetActive(false);
    }
}
