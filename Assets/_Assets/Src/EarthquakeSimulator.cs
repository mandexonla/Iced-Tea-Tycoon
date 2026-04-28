using UnityEngine;
using UnityEngine.InputSystem;

public class EarthquakeSimulator : MonoBehaviour
{
    [SerializeField] private float amplitude    = 15f;
    [SerializeField] private float frequency    = 1.5f;
    [SerializeField] private float escalateRate = 3f;
    [SerializeField] private float maxAmplitude = 60f;

    public static bool  IsShaking { get; private set; }
    public static float Elapsed   { get; private set; }
    public static event System.Action OnStarted;

    private Keyboard _keyboard;

    void Start()
    {
        _keyboard = Keyboard.current;
    }

    void Update()
    {
        if (_keyboard != null && _keyboard.spaceKey.wasPressedThisFrame)
            ToggleEarthquake();

        if (!IsShaking) return;

        Elapsed += Time.deltaTime;

        float currentAmplitude = Mathf.Min(amplitude + Elapsed * escalateRate, maxAmplitude);
        float force = Mathf.Sin(Elapsed * frequency * Mathf.PI * 2f) * currentAmplitude;

        foreach (Block block in Block.AllBlocks)
        {
            if (block == null) continue;
            block.Rb.AddForce(new Vector2(force, 0f), ForceMode2D.Force);
        }
    }

    void ToggleEarthquake()
    {
        if (!IsShaking && !StructureValidator.IsReady) return;

        IsShaking = !IsShaking;
        if (IsShaking)
        {
            Elapsed = 0f;
            OnStarted?.Invoke();
            SoundManager.Instance?.PlayEarthquake();
        }
    }

    public static void ForceStop()
    {
        IsShaking = false;
    }
}
