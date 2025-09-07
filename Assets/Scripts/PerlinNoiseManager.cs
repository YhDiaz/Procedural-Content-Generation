using UnityEditor;
using UnityEngine;

public class PerlinNoiseManager : MonoBehaviour
{
    public static System.Action OnPerlinNoiseModified;

    public int seed = 0;
    public float scale = 0.01f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public float brightness = 0.2f;
    public Vector2 offset = Vector2.zero;
    [SerializeField] private Color baseColor = Color.red;
    [SerializeField] private Color targetColor = Color.yellow;

    //[SerializeField] private int seed = 0;
    //[SerializeField, Range(0.001f, 0.1f)] private float scale = 0.01f;
    //[SerializeField, Range(1, 8)] private int octaves = 4;
    //[SerializeField, Range(0.01f, 0.99f)] private float persistence = 0.5f;
    //[SerializeField, Range(0f, 10f)] private float lacunarity = 2f;
    //[SerializeField, Range(0f, 1f)] private float brightness = 0.2f;
    //[SerializeField] private Vector2 offset = Vector2.zero;
    //[SerializeField] private Color baseColor = Color.red;
    //[SerializeField] private Color targetColor = Color.yellow;

    public Texture2D globalGradientMap;
    private int[] permutation;
    private int lastSeed = -1;

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        if (lastSeed != seed)
        {
            InitializePermutationTable();
            lastSeed = seed;
        }

        OnPerlinNoiseModified?.Invoke();
    }


    public void GenerateGlobalGradientMap(int mapWidth, int mapHeight)
    {
        InitializePermutationTable();
        globalGradientMap = new Texture2D(mapWidth, mapHeight);
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseValue = 0f;

                for (int o = 0; o < octaves; o++)
                {
                    float sampleX = (x + offset.x) * scale * frequency;
                    float sampleY = (y + offset.y) * scale * frequency;

                    float perlinValue = Perlin(sampleX, sampleY) * 2 - 1;
                    noiseValue += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                float normalizedNoise = (noiseValue + 1f) * 0.5f;
                normalizedNoise = Mathf.Clamp01(normalizedNoise + brightness);
                Color color = Color.Lerp(baseColor, targetColor, normalizedNoise);
                globalGradientMap.SetPixel(x, y, color);
            }
        }
        globalGradientMap.Apply();
    }

    public Texture2D GenerateRoomTextureFromGlobalMap(RectInt region)
    {
        if (globalGradientMap == null)
        {
            Debug.LogError("Global gradient map not generated!");
            return null;
        }

        Texture2D roomTexture = new Texture2D(region.width, region.height);
        for (int x = 0; x < region.width; x++)
        {
            for (int y = 0; y < region.height; y++)
            {
                Color c = globalGradientMap.GetPixel(region.x + x, region.y + y);
                roomTexture.SetPixel(x, y, c);
            }
        }
        roomTexture.Apply();
        return roomTexture;
    }

    // Inicializa la tabla de permutación basada en la semilla
    private void InitializePermutationTable()
    {
        Random.InitState(seed);
        permutation = new int[512];
        int[] basePerm = new int[256];

        // Llena la tabla base con valores 0-255
        for (int i = 0; i < 256; i++)
            basePerm[i] = i;

        // Mezcla los valores (Fisher-Yates shuffle)
        for (int i = 255; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (basePerm[i], basePerm[j]) = (basePerm[j], basePerm[i]);
        }

        // Duplica la tabla a 512 para evitar desbordes
        for (int i = 0; i < 512; i++)
            permutation[i] = basePerm[i % 256];

        Debug.Log("Permutation table initialized with seed: " + seed);
    }

    private float Perlin(float x, float y)
    {
        int xi = (int)x & 255;
        int yi = (int)y & 255;
        float xf = x - (int)x;
        float yf = y - (int)y;

        int g00 = permutation[(permutation[xi] + yi) & 255];
        int g10 = permutation[(permutation[xi + 1] + yi) & 255];
        int g01 = permutation[(permutation[xi] + yi + 1) & 255];
        int g11 = permutation[(permutation[xi + 1] + yi + 1) & 255];

        Vector2[] dists = new Vector2[4];
        dists[0] = new Vector2(xf, yf);
        dists[1] = new Vector2(xf - 1, yf);
        dists[2] = new Vector2(xf, yf - 1);
        dists[3] = new Vector2(xf - 1, yf - 1);

        float[] dots = new float[4];
        for (int i = 0; i < 4; i++)
        {
            Vector2 grad = GetGradientVector(permutation[(permutation[xi + (i % 2)] + yi + (i / 2)) & 255]);
            dots[i] = Vector2.Dot(dists[i], grad);
        }

        float u = Fade(xf);
        float v = Fade(yf);
        float a = Lerp(dots[0], dots[1], u);
        float b = Lerp(dots[2], dots[3], u);
        return Lerp(a, b, v);
    }

    private Vector2 GetGradientVector(int hash)
    {
        int h = hash % 12;
        switch (h)
        {
            case 0: return new Vector2(1, 0);
            case 1: return new Vector2(-1, 0);
            case 2: return new Vector2(0, 1);
            case 3: return new Vector2(0, -1);
            case 4: return new Vector2(1, 1).normalized;
            case 5: return new Vector2(-1, 1).normalized;
            case 6: return new Vector2(1, -1).normalized;
            case 7: return new Vector2(-1, -1).normalized;
            case 8: return new Vector2(0.707f, 0.707f);
            case 9: return new Vector2(-0.707f, 0.707f);
            case 10: return new Vector2(0.707f, -0.707f);
            case 11: return new Vector2(-0.707f, -0.707f);
            default: return Vector2.zero;
        }
    }

    private float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
    private float Lerp(float a, float b, float t) => a + t * (b - a);
}