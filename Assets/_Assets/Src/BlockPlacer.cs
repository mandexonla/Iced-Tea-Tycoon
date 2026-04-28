using UnityEngine;
using UnityEngine.InputSystem;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private float gridSize  = 1f;
    [SerializeField] private int   maxBudget = 15;
    [SerializeField] private BlockMaterialSO[] materials;

    private GameObject _ghost;
    private SpriteRenderer _ghostRenderer;
    private Mouse _mouse;
    private Keyboard _keyboard;
    private int _matIndex;
    private bool _deleteMode;
    private Block _hoveredBlock;

    private readonly Color _colorBlocked = new Color(1f, 0f, 0f, 0.4f);
    private readonly Color _colorHover   = new Color(1f, 0.2f, 0.2f, 0.8f);

    BlockMaterialSO CurrentMat => (materials != null && materials.Length > 0)
                                  ? materials[_matIndex] : null;

    int BudgetUsed()
    {
        int total = 0;
        foreach (Block b in Block.AllBlocks)
            if (b != null && b.Material != null)
                total += b.Material.cost;
        return total;
    }

    bool CanAfford() => CurrentMat == null || (BudgetUsed() + CurrentMat.cost <= maxBudget);

    void Start()
    {
        _mouse    = Mouse.current;
        _keyboard = Keyboard.current;
        if (ghostPrefab != null)
        {
            _ghost = Instantiate(ghostPrefab);
            _ghostRenderer = _ghost.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (_mouse == null) return;

        if (GameLoop.IsGameOver)
        {
            ClearHover();
            if (_ghost != null) _ghost.SetActive(false);
            return;
        }

        if (_keyboard != null && _keyboard.rKey.wasPressedThisFrame)
        {
            _deleteMode = !_deleteMode;
            if (!_deleteMode) ClearHover();
        }

        if (EarthquakeSimulator.IsShaking)
        {
            ClearHover();
            if (_ghost != null) _ghost.SetActive(false);
            return;
        }

        if (_deleteMode)
        {
            if (_ghost != null) _ghost.SetActive(false);
            HandleDeleteMode();
            return;
        }

        // --- Build mode ---
        HandleMaterialSwitch();

        if (_ghost != null) _ghost.SetActive(true);

        Vector3 snappedPos = GetMouseWorldPos();
        bool blocked   = IsOccupied(snappedPos);
        bool cantAfford = !CanAfford();

        if (_ghost != null && _ghostRenderer != null)
        {
            _ghost.transform.position = snappedPos;
            if (blocked || cantAfford)
            {
                _ghostRenderer.color = _colorBlocked;
            }
            else
            {
                Color tint = CurrentMat != null ? CurrentMat.tint : Color.white;
                _ghostRenderer.color = new Color(tint.r, tint.g, tint.b, 0.4f);
            }
        }

        if (_mouse.leftButton.wasPressedThisFrame && !blocked && !cantAfford)
            PlaceBlock(snappedPos);
    }

    void HandleDeleteMode()
    {
        Vector2 mouseWorld = GetMouseWorldPos2D();
        Collider2D hit = Physics2D.OverlapPoint(mouseWorld);
        Block block = hit != null ? hit.GetComponent<Block>() : null;

        if (block != _hoveredBlock)
        {
            ClearHover();
            _hoveredBlock = block;
            if (_hoveredBlock != null)
                _hoveredBlock.GetComponent<SpriteRenderer>().color = _colorHover;
        }

        if (_mouse.leftButton.wasPressedThisFrame && _hoveredBlock != null)
        {
            Destroy(_hoveredBlock.gameObject);
            _hoveredBlock = null;
        }
    }

    void ClearHover()
    {
        if (_hoveredBlock == null) return;
        Color restore = _hoveredBlock.Material != null ? _hoveredBlock.Material.tint : Color.white;
        _hoveredBlock.GetComponent<SpriteRenderer>().color = restore;
        _hoveredBlock = null;
    }

    void HandleMaterialSwitch()
    {
        if (_keyboard == null || materials == null) return;
        for (int i = 0; i < Mathf.Min(materials.Length, 9); i++)
        {
            if (_keyboard[(Key)(Key.Digit1 + i)].wasPressedThisFrame)
                _matIndex = i;
        }
    }

    void PlaceBlock(Vector3 pos)
    {
        if (blockPrefab == null) return;
        GameObject go = Instantiate(blockPrefab, pos, Quaternion.identity);
        go.GetComponent<Block>()?.ApplyMaterial(CurrentMat);
        SoundManager.Instance?.PlayPlace();
    }

    void OnGUI()
    {
        if (GameLoop.IsGameOver || EarthquakeSimulator.IsShaking) return;

        // Dòng 1: mode + material
        string modeLabel = _deleteMode
            ? "[R] Mode: DELETE — click block để xóa"
            : $"[R] Mode: BUILD  |  [1/2/3] {(CurrentMat != null ? CurrentMat.displayName : "None")} (cost:{(CurrentMat != null ? CurrentMat.cost.ToString() : "-")})  |  Mass:{(CurrentMat != null ? CurrentMat.mass.ToString() : "-")}  BreakForce:{(CurrentMat != null ? CurrentMat.breakForce.ToString() : "-")}";
        GUI.Label(new Rect(10, 10, 700, 25), modeLabel);

        // Dòng 4: budget bar
        if (!_deleteMode)
        {
            int used = BudgetUsed();
            int left = maxBudget - used;
            float ratio = (float)used / maxBudget;
            GUI.color = ratio < 0.6f ? Color.green : ratio < 0.85f ? Color.yellow : Color.red;
            GUI.Label(new Rect(10, 70, 300, 25), $"Budget: {used}/{maxBudget}  (còn {left})");
            GUI.color = Color.white;
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector2 screenPos = _mouse.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        worldPos.z = 0f;
        return SnapToGrid(worldPos);
    }

    Vector2 GetMouseWorldPos2D()
    {
        Vector2 screenPos = _mouse.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        return new Vector2(worldPos.x, worldPos.y);
    }

    bool IsOccupied(Vector3 pos)
    {
        Collider2D hit = Physics2D.OverlapBox(
            new Vector2(pos.x, pos.y),
            new Vector2(gridSize * 0.9f, gridSize * 0.9f),
            0f
        );
        return hit != null;
    }

    Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Floor(pos.x / gridSize) * gridSize + gridSize * 0.5f,
            Mathf.Floor(pos.y / gridSize) * gridSize + gridSize * 0.5f,
            0f
        );
    }
}
