using UnityEngine;

[CreateAssetMenu(fileName = "BlockMaterial", menuName = "TinyFarm/Block Material")]
public class BlockMaterialSO : ScriptableObject
{
    public string displayName = "Wood";
    public Color tint         = Color.white;
    public float mass         = 1f;
    public float breakForce   = 500f;
    public int   cost         = 1;
    public PhysicsMaterial2D physicsMaterial;
}
