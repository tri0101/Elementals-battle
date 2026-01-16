using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraFollowhero : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform hero1;
    public Transform hero2;

    [Header("Camera settings")]
    public float smoothSpeed = 5f;
    public float fixedY = 0f;                  // giữ trục Y cố định
    public Vector2 minMaxX = new Vector2(-7f, 7f);

    [Header("Zoom Settings")]
    public float defaultDistance = 14f;        // khoảng cách mặc định
    public float maxDistance = 30f;            // khoảng cách tối đa
    public int defaultRefResX = 80;            // refResolution khi gần
    public int minRefResX = 50;                // refResolution khi xa
    public float zoomSmooth = 5f;              // tốc độ zoom mượt

    private PixelPerfectCamera pixelCam;
    private float currentRefX;                  // float trung gian để Lerp mượt

    private void Awake()
    {
        hero1 = GameObject.FindGameObjectWithTag("hero1")?.transform;
        hero2 = GameObject.FindGameObjectWithTag("hero2")?.transform;

        pixelCam = GetComponent<PixelPerfectCamera>();
        if (pixelCam == null)
        {
            Debug.LogError("❌ Camera chưa có Pixel Perfect Camera component!");
        }

        currentRefX = pixelCam.refResolutionX;
    }

    private void LateUpdate()
    {
        if (hero1 == null || hero2 == null || pixelCam == null) return;

        // ==========================
        // 1. CAMERA FOLLOW TRỤC X
        // ==========================
        float centerX = (hero1.position.x + hero2.position.x) / 2f;
        centerX = Mathf.Clamp(centerX, minMaxX.x, minMaxX.y);

        Vector3 targetPos = new Vector3(centerX, fixedY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        // ==========================
        // 2. ZOOM MƯỢT THEO KHOẢNG CÁCH
        // ==========================
        float distance = Mathf.Abs(hero1.position.x - hero2.position.x);

        if (distance <= defaultDistance)
        {
            // Nếu khoảng cách nhỏ hơn mặc định → giữ refResolutionX gần
            currentRefX = Mathf.Lerp(currentRefX, defaultRefResX, Time.deltaTime * zoomSmooth);
        }
        else
        {
            // Nếu khoảng cách lớn hơn → zoom mượt từ defaultRefResX → minRefResX
            float t = Mathf.InverseLerp(defaultDistance, maxDistance, distance);
            float targetRefX = Mathf.Lerp(defaultRefResX, minRefResX, t);
            currentRefX = Mathf.Lerp(currentRefX, targetRefX, Time.deltaTime * zoomSmooth);
        }

        pixelCam.refResolutionX = Mathf.RoundToInt(currentRefX);
    }
}
