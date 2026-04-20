using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public struct UIElement
{
    public AbilityEffectType nameType;
    public Vector3 position;
}
[CreateAssetMenu(menuName = "hero/UIforBattle")]
public class UIForBattleSO : ScriptableObject
{
   
    public List<UIElement> listElement = new List<UIElement>();
    public Vector3 GetPosition(AbilityEffectType type)
    {
        foreach (var element in listElement)
        {
            if (element.nameType == type)
                return element.position;
        }
        return Vector3.zero; 
    }
}
