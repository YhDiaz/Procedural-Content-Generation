using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGenerator : MonoBehaviour
{
    // Offsets para aleatoriedad
    private float offsetX;
    private float offsetY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Nuevos offsets aleatorios cada vez que se genera una textura
            offsetX = Random.Range(0f, 1000f);
            offsetY = Random.Range(0f, 1000f);

            Texture2D texture = GenerateClassicPerlinTexture(256, 256, 10f, offsetX, offsetY);
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
            Debug.Log("Generated random Perlin Noise Texture");
        }
    }

    Texture2D GenerateClassicPerlinTexture(int width, int height, float scale, float offsetX, float offsetY)
    {
        Texture2D texture = new Texture2D(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                texture.SetPixel(x, y, new Color(sample, sample, sample));
            }
        }
        texture.Apply();
        return texture;
    }

}
