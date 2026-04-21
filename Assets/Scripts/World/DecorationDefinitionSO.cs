using UnityEngine;

[CreateAssetMenu(fileName = "Decoration", menuName = "Vox Obscura/World Generation/Decoration Definition")]
public class DecorationDefinitionSO : ScriptableObject
{
    [Header("Prefabs and Types")]
    public string decorationName;
    public GameObject prefab;

    [Header("Placement tuning")]
    [SerializeField, Range(0,1.0f)] private float spawnChance = 1.0f; 
    [SerializeField] private float clearanceHeight = 4.0f;
    [SerializeField] private float flatnessRadius = 1.0f;
    [SerializeField] private int edgePadding = 4;
    [SerializeField] private float minimumSpacing = 4f;

    [Header("Initialisation Settings")]
    [SerializeField] private bool initialiseAsVoxelObject = false;
    [SerializeField] private Vector3Int voxelObjectSize = new Vector3Int(8,8,8);
    [SerializeField] private float voxelScale = 0.5f;

    public float ClearanceHeight => clearanceHeight;
    public float FlatnessRadius => flatnessRadius;
    public int EdgePadding => edgePadding;
    public float MinimumSpacing => minimumSpacing;
    public bool InitialiseAsVoxelObject => initialiseAsVoxelObject;
    public Vector3 VoxelObjectSize => voxelObjectSize;
    public float VoxelScale => voxelScale;
}
