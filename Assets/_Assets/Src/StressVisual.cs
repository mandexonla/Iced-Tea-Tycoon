using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Block))]
public class StressVisual : MonoBehaviour
{
    [SerializeField] private float maxStressVelocity = 5f;

    private static readonly Color ColorNormal  = Color.white;
    private static readonly Color ColorSafe    = Color.green;
    private static readonly Color ColorDanger  = Color.red;

    private SpriteRenderer _renderer;
    private Block _block;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _block    = GetComponent<Block>();
    }

    void Update()
    {
        if (!EarthquakeSimulator.IsShaking)
        {
            _renderer.color = ColorNormal;
            return;
        }

        float stress = Mathf.Clamp01(_block.Rb.linearVelocity.magnitude / maxStressVelocity);
        _renderer.color = Color.Lerp(ColorSafe, ColorDanger, stress);
    }
}
