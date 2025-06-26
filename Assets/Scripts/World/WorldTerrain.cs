using System.Collections.Generic;
using UnityEngine;

public class WorldTerrain : MonoBehaviour
{
    [SerializeField] private FastNoiseLite.NoiseType noiseType;
    [SerializeField] private float frequency;
    [SerializeField] private float nodeSize;
    [SerializeField] private int nodesHalfLength;
    [SerializeField] private float obstacleThreshold;
    [SerializeField] private GameObject obstacleCubePrefab;
    [SerializeField] private GameObject instancedObstaclePoolPrefab;
    [SerializeField] private bool pressToRegenerate;
    [SerializeField] private float maxAdditionalHeight;
    [SerializeField] private int seed = 1337;
    private MeshFilter sharedMeshPool;
    private FastNoiseLite noise;

    public void OnValidate()
    {
        if (pressToRegenerate == true)
        {
            pressToRegenerate = false;
            Generate();
        }
    }

    private void Awake()
    {
        Generate();
    }

    public void Generate()
    {
        if (sharedMeshPool != null) Destroy(sharedMeshPool.gameObject);
        noise = new FastNoiseLite(seed);
        noise.SetNoiseType(noiseType);
        noise.SetFrequency(frequency * nodeSize);
        var baseMesh = obstacleCubePrefab.GetComponent<MeshFilter>().sharedMesh;
        sharedMeshPool = GameObject.Instantiate(instancedObstaclePoolPrefab, transform).GetComponent<MeshFilter>();
        List<CombineInstance> combinedMeshes = new List<CombineInstance>(nodesHalfLength * 4);
        int realNodeHalfLength = (int)((float)nodesHalfLength / nodeSize);
        for (int x = -realNodeHalfLength; x <= realNodeHalfLength; x++)
        {
            for (int z = -realNodeHalfLength; z <= realNodeHalfLength; z++)
            {
                float height = noise.GetNoise(x, z);
                if (height > obstacleThreshold || Mathf.Abs(z) == realNodeHalfLength || Mathf.Abs(x) == realNodeHalfLength)
                {
                    var scale = obstacleCubePrefab.transform.localScale;
                    scale.x *= nodeSize;
                    scale.z *= nodeSize;
                    var y = obstacleCubePrefab.transform.position.y;
                    var offset = Mathf.RoundToInt((((height - obstacleThreshold)/(1-obstacleThreshold)* maxAdditionalHeight) / nodeSize)) * nodeSize; 
                    if(offset < 0) { offset = 0; }
                    y += offset;
                    CombineInstance newMesh = new CombineInstance
                    {
                        mesh = baseMesh,
                        transform = Matrix4x4.TRS(new Vector3(x * nodeSize, y, z * nodeSize), obstacleCubePrefab.transform.rotation, scale)
                    };
                    combinedMeshes.Add(newMesh);
                }
            }
        }
        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        finalMesh.CombineMeshes(combinedMeshes.ToArray());
        sharedMeshPool.mesh = finalMesh;
        sharedMeshPool.GetComponent<MeshCollider>().sharedMesh = finalMesh;

    }
}
