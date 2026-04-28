using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] private float maxOffset  = 0.25f;
    [SerializeField] private float shakeSpeed = 25f;

    private Camera  _cam;
    private Vector3 _basePos;

    void Start()
    {
        _cam     = Camera.main;
        _basePos = _cam.transform.position;
    }

    // LateUpdate để chạy sau tất cả Update khác
    void LateUpdate()
    {
        if (!EarthquakeSimulator.IsShaking)
        {
            _cam.transform.position = _basePos;
            return;
        }

        // Intensity tăng dần trong 8 giây đầu, tương ứng với escalate của earthquake
        float t         = Mathf.Clamp01(EarthquakeSimulator.Elapsed / 8f);
        float intensity = t * maxOffset;

        float ox = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f)  * 2f - 1f) * intensity;
        float oy = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed)  * 2f - 1f) * intensity;

        _cam.transform.position = _basePos + new Vector3(ox, oy, 0f);
    }
}
