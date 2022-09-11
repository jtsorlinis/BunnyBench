using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class Scene : MonoBehaviour
{
  float gravity = 0.5f / 75;

  public Text fpsText;
  public Text bunnyText;
  public Material mat;

  float minX = -6.8f;
  float maxX = 6.1f;
  float minY = 4.3f;
  float maxY = -5f;

  int max = 2000000;

  private ComputeBuffer positionBuffer;

  Mesh mesh;
  Bounds bounds = new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f));

  Vector4[] positions = new Vector4[0];
  Vector2[] velocities = new Vector2[0];

  void Start()
  {
    mesh = MakeQuad(.7f, .7f);

    Array.Resize(ref positions, 10);
    Array.Resize(ref velocities, 10);

    for (int i = 0; i < 10; i++)
    {
      positions[i] = new Vector4(minX, minY, 0, 0);
      velocities[i] = new Vector2(UnityEngine.Random.Range(0, 0.13f), UnityEngine.Random.Range(-.06f, 0.06f));
    }
    positionBuffer = new ComputeBuffer(max, 16);
    positionBuffer.SetData(positions);
    mat.SetBuffer("positionBuffer", positionBuffer);

  }

  void Update()
  {

    fpsText.text = "FPS: " + ((int)(1 / Time.smoothDeltaTime));


    for (int i = 0; i < positions.Length; i++)
    {
      Vector4 pos = positions[i];
      Vector2 vel = velocities[i];

      pos.x += vel.x;
      pos.y += vel.y;
      vel.y -= gravity;

      if (pos.x > maxX)
      {
        vel.x *= -1;
        pos.x = maxX;
      }
      else if (pos.x < minX)
      {
        vel.x *= -1;
        pos.x = minX;
      }

      if (pos.y < this.maxY)
      {
        vel.y *= -0.85f;
        pos.y = maxY;
        pos.w = UnityEngine.Random.Range(-0.1f, 0.1f); // Passing rotation through w value of position
        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
        {
          vel.y += UnityEngine.Random.Range(0f, .1f);
        }
      }
      else if (pos.y > this.minY)
      {
        vel.y = 0;
        pos.y = minY;
      }

      positions[i] = pos;
      velocities[i] = vel;
    }

    positionBuffer.SetData(positions);
    Graphics.DrawMeshInstancedProcedural(mesh, 0, mat, bounds, positions.Length, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null, UnityEngine.Rendering.LightProbeUsage.Off, null);

    // Add bunnies while over 59fps
    if (1 / Time.smoothDeltaTime > 59)
    {
      Array.Resize(ref positions, positions.Length + 100);
      Array.Resize(ref velocities, positions.Length + 100);
      for (int i = 0; i < 100; i++)
      {
        positions[positions.Length - 100 + i] = new Vector4(minX, minY, 0, 0);
        velocities[positions.Length - 100 + i] = new Vector2(UnityEngine.Random.Range(0, 0.13f), UnityEngine.Random.Range(-.06f, 0.06f));
      }
      bunnyText.text = "Bunnies: " + positions.Length;
    }
  }

  // Helper functions
  Mesh MakeQuad(float width, float height)
  {
    Mesh mesh = new Mesh();

    Vector3[] vertices = new Vector3[4]
    {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(width, height, 0)
    };
    mesh.vertices = vertices;

    int[] tris = new int[6]
    {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
    };
    mesh.triangles = tris;

    Vector3[] normals = new Vector3[4]
    {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
    };
    mesh.normals = normals;

    Vector2[] uv = new Vector2[4]
    {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
    };
    mesh.uv = uv;

    return mesh;
  }

  void OnDisable()
  {
    if (positionBuffer != null)
      positionBuffer.Release();
    positionBuffer = null;
  }
}
