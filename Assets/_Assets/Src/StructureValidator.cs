using UnityEngine;

public class StructureValidator : MonoBehaviour
{
    [SerializeField] private int minBlocks       = 3;
    [SerializeField] private float settleThreshold = 0.05f;

    public static bool IsReady { get; private set; }

    private string _failReason = "";

    void Update()
    {
        if (EarthquakeSimulator.IsShaking)
        {
            IsReady = false;
            return;
        }

        Validate();
    }

    void Validate()
    {
        int count = Block.AllBlocks.Count;

        if (count < minBlocks)
        {
            IsReady = false;
            _failReason = $"Cần ít nhất {minBlocks} blocks (hiện tại: {count})";
            return;
        }

        foreach (Block b in Block.AllBlocks)
        {
            if (b == null) continue;
            if (b.Rb.linearVelocity.magnitude > settleThreshold)
            {
                IsReady = false;
                _failReason = "Chờ structure ổn định...";
                return;
            }
        }

        IsReady = true;
        _failReason = "";
    }

    void OnGUI()
    {
        if (EarthquakeSimulator.IsShaking) return;

        string label = IsReady
            ? $"[SPACE] Ready! ({Block.AllBlocks.Count} blocks) — Bấm Space để test"
            : $"[SPACE] {_failReason}";

        Color prev = GUI.color;
        GUI.color = IsReady ? Color.green : Color.red;
        GUI.Label(new Rect(10, 30, 500, 25), label);
        GUI.color = prev;
    }
}
