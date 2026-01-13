using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UI_ButtonOpen : MonoBehaviour
{
    public Transform ListMenu;
    private bool isOpen = false;
    

    public void OpenCanvas()
    {
        ListMenu.gameObject.SetActive(true);
        isOpen = true;
    }
}
