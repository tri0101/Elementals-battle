using UnityEngine;

public class UI_ButtonLoadScene : MonoBehaviour
{
    public void OnClickUnLoadAdditiveScene(int sceneId)
    {
        GameManager.Instance.UnLoadAdditiveScene((SceneId)sceneId);
    }
    public void OnClickLoadAdditiveScene(int sceneId)
    {
        GameManager.Instance.LoadAdditiveScene((SceneId)sceneId);
    }
}
