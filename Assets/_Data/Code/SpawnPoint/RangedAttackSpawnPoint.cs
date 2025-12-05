
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UIElements;

public class RangedAttackSpawnPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;
    public static RangedAttackSpawnPoint instance;  
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
    public void SpawnObjectAtPosition(string objName, Vector3 objPosition, string tag, Vector3 scale)
    {
        GameObject objSpawn = BrowseList(objName);
        GameObject tmpSpawwn = Instantiate(objSpawn);
        tmpSpawwn.tag = tag;
        tmpSpawwn.transform.position = objPosition;
        tmpSpawwn.transform.localScale = scale;

        tmpSpawwn.SetActive(true);
    }
}
