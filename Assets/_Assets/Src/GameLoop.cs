using UnityEngine;
using UnityEngine.InputSystem;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private float surviveTime = 10f;

    public static bool IsGameOver { get; private set; }

    private enum State { Building, Testing, Win, Lose }
    private State _state = State.Building;

    private int _blockCountAtStart;
    private float _surviveElapsed;
    private Keyboard _keyboard;

    void OnEnable()  { EarthquakeSimulator.OnStarted += OnEarthquakeStarted; }
    void OnDisable() { EarthquakeSimulator.OnStarted -= OnEarthquakeStarted; }

    void Start()
    {
        _keyboard = Keyboard.current;
        IsGameOver = false;
    }

    void Update()
    {
        if (_state == State.Testing)
            UpdateTesting();

        if (IsGameOver && _keyboard != null && _keyboard.enterKey.wasPressedThisFrame)
            Retry();
    }

    void OnEarthquakeStarted()
    {
        _blockCountAtStart = Block.AllBlocks.Count;
        _surviveElapsed = 0f;
        _state = State.Testing;
        IsGameOver = false;
        ScoreSystem.Reset();
    }

    void UpdateTesting()
    {
        if (Block.AllBlocks.Count == 0)
        {
            EndGame(false);
            return;
        }

        _surviveElapsed += Time.deltaTime;
        if (_surviveElapsed >= surviveTime)
            EndGame(true);
    }

    void EndGame(bool win)
    {
        _state = win ? State.Win : State.Lose;
        IsGameOver = true;
        EarthquakeSimulator.ForceStop();

        if (win)
        {
            int score = ScoreSystem.Calculate(Block.AllBlocks.Count, _blockCountAtStart, _surviveElapsed);
            ScoreSystem.Submit(score);
            SoundManager.Instance?.PlayWin();
        }
        else
        {
            SoundManager.Instance?.PlayLose();
        }
    }

    void Retry()
    {
        foreach (Block b in Block.AllBlocks.ToArray())
            if (b != null) Destroy(b.gameObject);

        _state = State.Building;
        IsGameOver = false;
    }

    void OnGUI()
    {
        // HUD dòng 3: best score khi đang build
        if (_state == State.Building && ScoreSystem.BestScore > 0)
        {
            GUI.color = new Color(1f, 1f, 0.4f);
            GUI.Label(new Rect(10, 50, 300, 25), $"Best: {ScoreSystem.BestScore:N0} pts");
            GUI.color = Color.white;
        }

        if (_state == State.Testing)
        {
            float remaining = Mathf.Max(0f, surviveTime - _surviveElapsed);
            GUI.color = Color.yellow;
            GUI.Label(new Rect(10, 50, 350, 25),
                $"Survive: {remaining:F1}s  |  Blocks: {Block.AllBlocks.Count}/{_blockCountAtStart}");
            GUI.color = Color.white;
            return;
        }

        if (_state != State.Win && _state != State.Lose) return;

        float bw = 360f, bh = 140f;
        Rect box = new Rect((Screen.width - bw) * 0.5f, (Screen.height - bh) * 0.5f, bw, bh);
        GUI.Box(box, "");

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            { fontSize = 22, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        GUIStyle subStyle = new GUIStyle(GUI.skin.label)
            { fontSize = 13, alignment = TextAnchor.MiddleCenter };
        GUIStyle scoreStyle = new GUIStyle(GUI.skin.label)
            { fontSize = 16, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };

        GUI.color = _state == State.Win ? Color.green : Color.red;
        string title = _state == State.Win ? "STRUCTURE SURVIVED!" : "STRUCTURE COLLAPSED!";
        GUI.Label(new Rect(box.x, box.y + 8, bw, 36), title, titleStyle);
        GUI.color = Color.white;

        if (_state == State.Win)
        {
            string newBest = ScoreSystem.IsNewBest ? "  ★ NEW BEST!" : $"  (Best: {ScoreSystem.BestScore:N0})";
            GUI.color = ScoreSystem.IsNewBest ? Color.yellow : Color.white;
            GUI.Label(new Rect(box.x, box.y + 46, bw, 26),
                $"Score: {ScoreSystem.LastScore:N0} pts{newBest}", scoreStyle);
            GUI.color = Color.white;

            string stat = $"Còn {Block.AllBlocks.Count}/{_blockCountAtStart} blocks sau {_surviveElapsed:F1}s";
            GUI.Label(new Rect(box.x, box.y + 74, bw, 22), stat, subStyle);
        }
        else
        {
            string stat = $"Sập sau {_surviveElapsed:F1}s  ({_blockCountAtStart - Block.AllBlocks.Count}/{_blockCountAtStart} blocks mất)";
            GUI.Label(new Rect(box.x, box.y + 50, bw, 22), stat, subStyle);
            if (ScoreSystem.BestScore > 0)
                GUI.Label(new Rect(box.x, box.y + 74, bw, 22), $"Best: {ScoreSystem.BestScore:N0} pts", subStyle);
        }

        GUI.Label(new Rect(box.x, box.y + 110, bw, 22), "[Enter] Retry", subStyle);
    }
}
