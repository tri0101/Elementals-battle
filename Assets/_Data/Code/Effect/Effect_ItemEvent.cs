using UnityEngine;

public class Effect_ItemEvent : MonoBehaviour
{
    public void SetDisappear()
    {
        Destroy(transform.parent.gameObject);
    }
}
