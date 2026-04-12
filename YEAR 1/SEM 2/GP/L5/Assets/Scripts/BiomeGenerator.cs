using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BiomeGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int mapWidth = 100;
    public int mapHeight = 100;
    public float meshScale = 1f;
    public float heightMultiplier = 15f;
    public float waterLevel = 0.35f;

    [Header("Fractal Noise Settings")]
    public float noiseScale = 3f;
    [Range(1, 8)] public int octaves = 6;
    [Range(0, 1)] public float persistence = 0.5f;
    public float lacunarity = 2f;

    [Header("Seeds")]
    public float elevationSeed = 100f;
    public float moistureSeed = 500f;

    private float[,] elevationMap;
    private float[,] moistureMap;

    void Start()
    {
        GenerateMaps();
        ConstructMesh();
    }


    void GenerateMaps()
    {
        elevationMap = new float[mapWidth, mapHeight];
        moistureMap = new float[mapWidth, mapHeight];

        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                elevationMap[x, z] = FractalNoise(x, z, mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity, elevationSeed);

                moistureMap[x, z] = FractalNoise(x, z, mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity, moistureSeed);
            }
        }
    }

 
    float FractalNoise(int x, int y, int width, int height, float scale, int octaves, float pers, float lacun, float seedOffset)
    {
        float noiseHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxPossibleHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            float xCoord = (float)x / width * scale * frequency + seedOffset;
            float yCoord = (float)y / height * scale * frequency + seedOffset;

            float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * 2f - 1f;
            noiseHeight += perlinValue * amplitude;

            maxPossibleHeight += amplitude;
            amplitude *= pers;
            frequency *= lacun;
        }

        return (noiseHeight / maxPossibleHeight + 1f) / 2f;
    }


    Color GetBiomeColor(float elevation, float moisture)
    {
        // Ocean
        if (elevation < 0.35f) return new Color(0.1f, 0.4f, 0.8f); // Blue

        // Beach
        if (elevation < 0.40f) return new Color(0.9f, 0.8f, 0.6f); // Sand Yellow

        // Snowy Peaks
        if (elevation > 0.75f) return Color.white; // White

        // Tundra / Taiga
        if (elevation >= 0.60f)
        {
            if (moisture < 0.40f) return new Color(0.6f, 0.7f, 0.7f); // Tundra (Light Blue-Grey)
            else return new Color(0.3f, 0.5f, 0.4f);                  // Taiga (Cool Dark Green)
        }

        if (moisture < 0.20f) return new Color(0.8f, 0.7f, 0.4f);      // Desert (Warm Yellow-Brown)
        if (moisture < 0.40f) return new Color(0.6f, 0.7f, 0.3f);      // Savanna (Yellow-Green)
        if (moisture < 0.65f) return new Color(0.2f, 0.5f, 0.2f);      // Temperate Forest (Standard Green)
        if (moisture < 0.85f) return new Color(0.1f, 0.4f, 0.1f);      // Jungle (Deep Green)

        return new Color(0.2f, 0.3f, 0.2f);                            // Swamp (Murky Olive Green)
    }



    void ConstructMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertices = new Vector3[mapWidth * mapHeight];
        Color[] colors = new Color[mapWidth * mapHeight];

        int[] triangles = new int[(mapWidth - 1) * (mapHeight - 1) * 6];


        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int idx = z * mapWidth + x;

                float height = elevationMap[x, z] < waterLevel ? waterLevel : elevationMap[x, z];

                vertices[idx] = new Vector3(x * meshScale, height * heightMultiplier, z * meshScale);

                colors[idx] = GetBiomeColor(elevationMap[x, z], moistureMap[x, z]);
            }
        }

        int triIdx = 0;
        for (int z = 0; z < mapHeight - 1; z++)
        {
            for (int x = 0; x < mapWidth - 1; x++)
            {
                int a = z * mapWidth + x;
                int b = a + 1;
                int c = (z + 1) * mapWidth + x;
                int d = c + 1;

                triangles[triIdx++] = a;
                triangles[triIdx++] = c;
                triangles[triIdx++] = b;

                triangles[triIdx++] = b;
                triangles[triIdx++] = c;
                triangles[triIdx++] = d;
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
            Material mat = new Material(urpShader);
            renderer.material = mat;

        }
    }
}