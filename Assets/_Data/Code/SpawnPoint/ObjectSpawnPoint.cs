
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UIElements;

public class ObjectSpawnPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;
    public static ObjectSpawnPoint instance;
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
            if (chill.name == nameObject)
            {
                return chill.gameObject;
            }

        }
        return null;
    }
    public void SpawnObjectAtPosition(string objName, Transform playerSpawnPos, string playerSpawnTag)
    {
        GameObject objSpawn = BrowseList(objName);
        ObjectSpawnPointController obc = objSpawn.GetComponent<ObjectSpawnPointController>();
        Vector3 spawnPoint = obc.ObjectSpawnPointSO.spawnPosition;
        GameObject tmpSpawwn = Instantiate(objSpawn);
        tmpSpawwn.tag = playerSpawnTag;
        tmpSpawwn.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.layer = LayerMask.NameToLayer(playerSpawnTag);

        //Tính giá tr? cu?i cùng c?a obj khi spawn ra v?i y default 
        Vector3 finalSpawnPoint = new Vector3(0, 0, 0);
        if (playerSpawnPos.parent.localScale.x > 0)
        {
            finalSpawnPoint = playerSpawnPos.position + new Vector3(spawnPoint.x, 0, 0);
        }
        else
        {
            finalSpawnPoint = playerSpawnPos.position + new Vector3(-spawnPoint.x, 0, 0);

        }
        finalSpawnPoint.y = spawnPoint.y;
        tmpSpawwn.transform.position = finalSpawnPoint;


        tmpSpawwn.SetActive(true);
    }
}
