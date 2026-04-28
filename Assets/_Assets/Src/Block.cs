using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Block : MonoBehaviour
{
    [SerializeField] private float breakForce = 500f;
    [SerializeField] private GameObject breakParticlePrefab;

    public static readonly List<Block> AllBlocks = new List<Block>();
    public Rigidbody2D Rb { get; private set; }
    public BlockMaterialSO Material { get; private set; }

    private readonly HashSet<Rigidbody2D> _connected = new HashSet<Rigidbody2D>();
    private const int MaxConnections = 4;

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()  { AllBlocks.Add(this); }
    void OnDisable() { AllBlocks.Remove(this); }

    public void ApplyMaterial(BlockMaterialSO mat)
    {
        if (mat == null) return;
        Material = mat;
        GetComponent<SpriteRenderer>().color = mat.tint;
        Rb.mass = mat.mass;
        breakForce = mat.breakForce;
        if (mat.physicsMaterial != null)
            GetComponent<BoxCollider2D>().sharedMaterial = mat.physicsMaterial;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_connected.Count >= MaxConnections) return;
        if (!collision.gameObject.CompareTag("Block")) return;

        Rigidbody2D otherRb = collision.rigidbody;
        if (otherRb == null || _connected.Contains(otherRb)) return;

        _connected.Add(otherRb);

        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = otherRb;
        joint.breakForce = breakForce;
        joint.breakTorque = Mathf.Infinity;
        joint.autoConfigureConnectedAnchor = true;
    }

    void OnJointBreak2D(Joint2D brokenJoint)
    {
        SoundManager.Instance?.PlayBreak();

        if (breakParticlePrefab != null)
        {
            GameObject vfx = Instantiate(breakParticlePrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }
    }
}
