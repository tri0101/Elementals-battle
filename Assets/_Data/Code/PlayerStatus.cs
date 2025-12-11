using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    PlayerController pc;
    [SerializeField] private bool isFronzen = false;
    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }
    private void Update()
    {
        if(pc.StatusEffect == StatusEffect.Frozen && !isFronzen)
        {
            StartFrozen();
        }
    }
    public void StartFrozen()
    {
        isFronzen = true;
        StartCoroutine(FrozenCoroutine());
    }
    private IEnumerator FrozenCoroutine()
    {
        pc.Animator.SetTrigger("Hit");
        yield return new WaitForSeconds(0.1f);
        pc.PlayerReceiveDamage.IsImmortal = true;
        // Lưu lại màu cũ
        Color originalColor = pc.SpriteRenderer.color;
        pc.Animator.speed = 0f;
        // Màu frozen: RGB(32,45,211) → Unity Color dùng 0~1 nên chia 255
        Color frozenColor = new Color(32f / 255f, 45f / 255f, 211f / 255f);

        // đổi màu
        pc.SpriteRenderer.color = frozenColor;
        yield return new WaitForSeconds(0.5f);
        pc.Animator.speed = 0.5f;
        pc.PlayerReceiveDamage.IsImmortal = false;
        pc.PlayerMovement.MinusProperty(0.3f);
        // chờ 5 giây
        yield return new WaitForSeconds(2f);

        // trả về màu cũ
        pc.SpriteRenderer.color = originalColor;
        pc.Animator.speed = 1f;
        pc.PlayerMovement.PlusProperty(0.3f);
        // trạng thái về bình thường
        ChangeStatus(StatusEffect.Normal);
    }
    public void ChangeStatus(StatusEffect status)
    {
        pc.StatusEffect = status;
        isFronzen = false;

    }
}
