using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToClose : MonoBehaviour, IPointerClickHandler
{
    public GameObject target; // Panel cần bật  (thường là panel cha)

    public void OnPointerClick(PointerEventData eventData)
    {
        if (target != null)
        {
            target.SetActive(true);
            gameObject.SetActive(false);
        }
           
    }
}
