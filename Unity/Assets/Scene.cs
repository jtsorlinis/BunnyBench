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

  float xBound;
  float yBound;

  private ComputeBuffer positionBuffer;

  public Mesh quad;
  Bounds bounds = new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f));

  Vector4[] positions = new Vector4[0];
  Vector2[] velocities = new Vector2[0];

  void Start()
  {
    yBound = Camera.main.orthographicSize - 0.3f;
    xBound = Camera.main.orthographicSize * Camera.main.aspect - 0.2f;

    Array.Resize(ref positions, 10);
    Array.Resize(ref velocities, 10);

    for (int i = 0; i < 10; i++)
    {
      positions[i] = new Vector4(-xBound, yBound, 0, 0);
      velocities[i] = new Vector2(UnityEngine.Random.Range(0, 0.13f), UnityEngine.Random.Range(-.06f, 0.06f));
    }
    positionBuffer = new ComputeBuffer(10, 16);
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

      if (pos.x > xBound)
      {
        vel.x *= -1;
        pos.x = xBound;
      }
      else if (pos.x < -xBound)
      {
        vel.x *= -1;
        pos.x = -xBound;
      }

      if (pos.y < -yBound)
      {
        vel.y *= -0.85f;
        pos.y = -yBound;
        pos.w = UnityEngine.Random.Range(-0.1f, 0.1f); // Passing rotation through w value of position
        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
        {
          vel.y += UnityEngine.Random.Range(0f, .1f);
        }
      }
      else if (pos.y > yBound)
      {
        vel.y = 0;
        pos.y = yBound;
      }

      positions[i] = pos;
      velocities[i] = vel;
    }


    // Add bunnies while over 59fps
    if (1 / Time.smoothDeltaTime > 59)
    {
      Array.Resize(ref positions, positions.Length + 1000);
      Array.Resize(ref velocities, positions.Length + 1000);
      for (int i = 0; i < 1000; i++)
      {
        positions[positions.Length - 1000 + i] = new Vector4(-xBound, yBound, 0, 0);
        velocities[positions.Length - 1000 + i] = new Vector2(UnityEngine.Random.Range(0, 0.13f), UnityEngine.Random.Range(-.06f, 0.06f));
      }

      // Resize buffer if we need to
      if (positionBuffer.count < positions.Length)
      {
        positionBuffer.Dispose();
        positionBuffer = new ComputeBuffer(Mathf.NextPowerOfTwo(positions.Length), 16);
        mat.SetBuffer("positionBuffer", positionBuffer);
      }

      bunnyText.text = "Bunnies: " + positions.Length;
    }

    positionBuffer.SetData(positions);
    Graphics.DrawMeshInstancedProcedural(quad, 0, mat, bounds, positions.Length, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null, UnityEngine.Rendering.LightProbeUsage.Off, null);
  }

  void OnDisable()
  {
    if (positionBuffer != null)
      positionBuffer.Release();
    positionBuffer = null;
  }
}
