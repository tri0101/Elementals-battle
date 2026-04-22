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
        holder = transform.GetChild(0);
    }

   
    public GameObject BrowseList(string nameObject)
    {
        foreach (GameObject chill in spawnPoints)
        {
            if (chill.name == nameObject)
            {
                return chill;
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

    public void SpawnObjectAtPosition(Vector3 vector, string nameObjectSpawn, float scaleX, HeroControl spawner)
    {
        

        GameObject prefab = BrowseList(nameObjectSpawn);
        if (prefab == null)
            return;

        GameObject spawned = Instantiate(prefab);
        spawned.name = prefab.name;

        if (holder != null)
            spawned.transform.SetParent(holder, false);

        ObjectSpawnPointController controller = spawned.GetComponent<ObjectSpawnPointController>();
        controller.SetSpawner(spawner);
        Vector3 scale = controller.transform.localScale;
        scale.x = scaleX;
        controller.transform.localScale = scale;
        if (controller == null || controller.ObjectSpawnPointSO == null)
        {
            
            return;
        }

        spawned.transform.position = vector;

        if (controller.ObjectSpawnPointSO.canFly && controller.ObjectSpawnPointFly != null)
        {
            controller.ObjectSpawnPointFly.SetDirectionFromUnitScale(scaleX);
            controller.ObjectSpawnPointFly.Fly();
        }
    }

    public void AddToPool(GameObject obj)
    {
        poolObject.Add(obj);
        obj.SetActive(false);
    }
}