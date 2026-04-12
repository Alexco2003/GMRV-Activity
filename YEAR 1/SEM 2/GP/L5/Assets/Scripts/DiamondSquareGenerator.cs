using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DiamondSquareGenerator : MonoBehaviour
{
    [Header("Matrix Size (2^n + 1)")]
    [Range(1, 8)]
    public int n = 7;

    [Header("Terrain Settings")]
    public float meshScale = 1f;
    public float heightMultiplier = 15f;

    [Header("Diamond Square Settings")]
    public float initialRandomRange = 5f;
    [Range(0f, 1f)]
    public float roughness = 0.5f;

    private float[,] elevationMap;
    private int mapSize;

    void Start()
    {
        mapSize = (1 << n) + 1;
        elevationMap = new float[mapSize, mapSize];

        RunDiamondSquare();
        ConstructMesh();
    }

    void RunDiamondSquare()
    {
        float currentRange = initialRandomRange;

        elevationMap[0, 0] = Random.Range(-currentRange, currentRange);
        elevationMap[0, mapSize - 1] = Random.Range(-currentRange, currentRange);
        elevationMap[mapSize - 1, 0] = Random.Range(-currentRange, currentRange);
        elevationMap[mapSize - 1, mapSize - 1] = Random.Range(-currentRange, currentRange);

        int stepSize = mapSize - 1;

        while (stepSize > 1)
        {
            int halfStep = stepSize / 2;

            for (int y = 0; y < mapSize - 1; y += stepSize)
            {
                for (int x = 0; x < mapSize - 1; x += stepSize)
                {
                    float avg = (elevationMap[x, y] +
                                 elevationMap[x + stepSize, y] +
                                 elevationMap[x, y + stepSize] +
                                 elevationMap[x + stepSize, y + stepSize]) / 4.0f;

                    elevationMap[x + halfStep, y + halfStep] = avg + Random.Range(-currentRange, currentRange);
                }
            }

            for (int y = 0; y < mapSize; y += halfStep)
            {
                int startX = (y % stepSize == 0) ? halfStep : 0;

                for (int x = startX; x < mapSize; x += stepSize)
                {
                    float sum = 0;
                    int count = 0;

                    if (x >= halfStep) { sum += elevationMap[x - halfStep, y]; count++; } // Left
                    if (x + halfStep < mapSize) { sum += elevationMap[x + halfStep, y]; count++; } // Right
                    if (y >= halfStep) { sum += elevationMap[x, y - halfStep]; count++; } // Bottom
                    if (y + halfStep < mapSize) { sum += elevationMap[x, y + halfStep]; count++; } // Top

                    elevationMap[x, y] = (sum / count) + Random.Range(-currentRange, currentRange);
                }
            }

            currentRange *= roughness;
            stepSize /= 2;
        }
    }

    Color GetTerrainColor(float t)
    {
        Color ocean = new Color(0.1f, 0.4f, 0.8f); // Blue
        Color beach = new Color(0.9f, 0.8f, 0.6f); // Sand Yellow
        Color forest = new Color(0.2f, 0.5f, 0.2f); // Temperate Forest (Standard Green)
        Color taiga = new Color(0.3f, 0.5f, 0.4f); // Taiga (Cool Dark Green)
        Color tundra = new Color(0.6f, 0.7f, 0.7f); // Tundra (Light Blue-Grey)
        Color peaks = Color.white; // White


        if (t < 0.4f) return Color.Lerp(ocean, beach, t / 0.4f);
        if (t < 0.45f) return Color.Lerp(beach, forest, (t - 0.4f) / 0.05f);
        if (t < 0.65f) return Color.Lerp(forest, taiga, (t - 0.45f) / 0.2f);
        if (t < 0.8f) return Color.Lerp(taiga, tundra, (t - 0.65f) / 0.15f);
        return Color.Lerp(tundra, peaks, (t - 0.8f) / 0.2f);
    }

    void ConstructMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertices = new Vector3[mapSize * mapSize];
        Color[] colors = new Color[mapSize * mapSize];
        int[] triangles = new int[(mapSize - 1) * (mapSize - 1) * 6];

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float h = elevationMap[x, z];
                if (h < minHeight) minHeight = h;
                if (h > maxHeight) maxHeight = h;

                int idx = z * mapSize + x;
                vertices[idx] = new Vector3(x * meshScale, h * heightMultiplier, z * meshScale);
            }
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            float normalizedHeight = Mathf.InverseLerp(minHeight, maxHeight, elevationMap[i % mapSize, i / mapSize]);
            colors[i] = GetTerrainColor(normalizedHeight);
        }

        int triIdx = 0;
        for (int z = 0; z < mapSize - 1; z++)
        {
            for (int x = 0; x < mapSize - 1; x++)
            {
                int a = z * mapSize + x;
                int b = a + 1;
                int c = (z + 1) * mapSize + x;
                int d = c + 1;

                triangles[triIdx++] = a; triangles[triIdx++] = c; triangles[triIdx++] = b;
                triangles[triIdx++] = b; triangles[triIdx++] = c; triangles[triIdx++] = d;
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer.sharedMaterial == null)
        {
            Shader urpShader = Shader.Find("Universal Render Pipeline/Particles/Lit");
            if (urpShader != null)
            {
                Material mat = new Material(urpShader);
                renderer.material = mat;
            }
        }
    }
}