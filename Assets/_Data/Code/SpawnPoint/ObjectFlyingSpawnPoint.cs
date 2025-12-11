
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UIElements;
using NUnit.Framework.Interfaces;

public class ObjectFlyingSpawnPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;
    public static ObjectFlyingSpawnPoint instance;  
    private void Awake()
    {
        instance = this;
        foreach (Transform chill in transform)
        {
            spawnPoints.Add(chill.gameObject);
        }
    }
    public GameObject BrowseList(string nameObject)
    {
        foreach (Transform chill in transform)
        {
            if(chill.name == nameObject)
            {
                return chill.gameObject;
            }
            
        }
        return null;
    }
    //public void SpawnObjectAtPosition(string objName, Vector3 objPosition, string player, Vector3 scale)
    //{
    //    GameObject objSpawn = BrowseList(objName);
    //    GameObject tmpSpawwn = Instantiate(objSpawn);
    //    tmpSpawwn.tag = player;
    //    tmpSpawwn.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.layer = LayerMask.NameToLayer(player);
    //    tmpSpawwn.transform.position = objPosition;
    //    tmpSpawwn.transform.localScale = scale;

    //    tmpSpawwn.SetActive(true);
    //}
    public void SpawnObjectAtPosition(string objName, Transform playerSpawnPos, string playerSpawnTag)
    {
        GameObject objSpawn = BrowseList(objName);
        ObjectFlyingController obc = objSpawn.GetComponent<ObjectFlyingController>();
        Vector3 spawnPoint = obc.ObjectFlyingSO.spawnPosition;
        GameObject tmpSpawwn = Instantiate(objSpawn);
        tmpSpawwn.tag = playerSpawnTag;
        tmpSpawwn.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.layer = LayerMask.NameToLayer(playerSpawnTag);

        //Tính giá trị cuối cùng của obj khi spawn ra với y default 
        Vector3 finalSpawnPoint = new Vector3(0, 0, 0);
        if(playerSpawnPos.parent.localScale.x  > 0)
        {
            finalSpawnPoint = playerSpawnPos.position + new Vector3(spawnPoint.x, 0, 0);
        }
        else
        {
            finalSpawnPoint = playerSpawnPos.position + new Vector3(-spawnPoint.x, 0, 0);

        }
        finalSpawnPoint.y = spawnPoint.y;
        tmpSpawwn.transform.position = finalSpawnPoint;
        //Tính giá trị scale 
        tmpSpawwn.transform.localScale = new Vector3(playerSpawnPos.parent.localScale.x > 0 ? 1 : -1, 1, 1);

        tmpSpawwn.SetActive(true);
    }
  

}
