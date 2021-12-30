using UnityEngine;

[CreateAssetMenu(fileName = "New Brick", menuName = "nikSOnik/Data/Add Brick")]
public class BrickData : ScriptableObject
{
    [SerializeField] GameObject prefab;
    [SerializeField] Color color;
    [SerializeField] int points;
    [SerializeField] int hitsToDestroy;
    public int Points => points;
    public int HitsToDestroy => hitsToDestroy;
    public bool IsIndestructible => hitsToDestroy == 0;
    public BrickObject CreateInstance()
    {
        GameObject o = Instantiate(prefab);

        var renderer = o.GetComponentInChildren<Renderer>();

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", color);
        renderer.SetPropertyBlock(block);

        return o.GetComponent<BrickObject>();

    }
}