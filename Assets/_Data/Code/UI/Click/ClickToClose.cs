using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToClose : MonoBehaviour, IPointerClickHandler
{
    public GameObject target; // Panel cần tắt (thường là panel cha)

    public void OnPointerClick(PointerEventData eventData)
    {
        if (target != null)
            target.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
