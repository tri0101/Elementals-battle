using UnityEngine;
[CreateAssetMenu(menuName = "Settings/KeyBinding")]
public class KeyBinding : ScriptableObject
{
    public KeyCode leftMove;
    public KeyCode rightMove;
    public KeyCode blockKey;
    public KeyCode jumpKey;
    public KeyCode attackKey;
    public KeyCode dashKey;
    public KeyCode skillKey;
    public KeyCode transformKey;

}
