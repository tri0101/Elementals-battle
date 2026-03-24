using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }
    [SerializeField] Transform listEffect;
    public Transform ListEffect => listEffect;

    [Serializable]
    private sealed class EffectEntry
    {
        public AbilityEffectType type;
        public GameObject prefab;
    }

    [Header("Registry (assign in Inspector)")]
    [SerializeField] private List<EffectEntry> effectPrefabs = new List<EffectEntry>();

    private readonly Dictionary<AbilityEffectType, GameObject> prefabByType =
        new Dictionary<AbilityEffectType, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        listEffect = transform.GetChild(0);

        prefabByType.Clear();
        for (int i = 0; i < effectPrefabs.Count; i++)
        {
            var e = effectPrefabs[i];
            if (e == null || e.prefab == null) continue;

            // nếu trùng key thì thằng sau ghi đè
            prefabByType[e.type] = e.prefab;
        }
    }

    public GameObject Spawn(
        AbilityEffectType type,
        Transform parent,
        Vector3 localPosition,
        Quaternion localRotation,  
        List<Transform> listTarget = null
    )
    {
        if (parent == null)
        {
            Debug.LogWarning("[EffectManager] Spawn failed: parent is NULL.");
            return null;
        }

        if (!prefabByType.TryGetValue(type, out var prefab) || prefab == null)
        {
            Debug.LogWarning($"[EffectManager] No prefab registered for effect type: {type}");
            return null;
        }

        var go = Instantiate(prefab, parent, false);
        if(listTarget != null)
        {
            foreach (var target in listTarget)
            {
                var effectTarget = go.GetComponent<Effect_Item>();
                if (effectTarget != null)
                {
                    effectTarget.SetTarget(target);
                }
            }
        }
       
        go.transform.localPosition = localPosition;
        go.transform.localRotation = localRotation;

        return go;
    }

    public GameObject Spawn(
        AbilityEffectType type,
        Transform parent
    )
    {
        Vector3 vector = new Vector3(0f, 0f, -1f);
        return Spawn(type, parent, vector, Quaternion.identity);
    }
    public GameObject Spawn(
        AbilityEffectType type,
        Transform parent,
        List<Transform> listTarget,
        Vector3 localPosition
    )
    {
        localPosition.z = -1f;
        
        return Spawn(type, parent, localPosition, Quaternion.identity,  listTarget);
    }
}