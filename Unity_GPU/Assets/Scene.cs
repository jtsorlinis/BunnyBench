using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public struct Bunny
{
  public Vector4 pos;
  public Vector2 vel;
  float pad0;
  float pad1;
}


public class Scene : MonoBehaviour
{
  public Text fpsText;
  public Text bunnyText;
  public Material mat;

  float minX = -6.7f;
  float minY = 4.4f;

  public ComputeShader bunniesComputeShader;

  private ComputeBuffer bunniesBuffer;

  int count = 0;
  int max = 2000000;

  Mesh mesh;
  Bounds bounds = new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f));


  void Start()
  {
    mesh = MakeQuad(.43f, .625f);

    bunniesBuffer = new ComputeBuffer(max, 32);

    var _bunniesArray = new Bunny[max];


    for (int i = 0; i < max; i++)
    {
      _bunniesArray[i].pos = new Vector4(minX, minY, 0, 0);
      _bunniesArray[i].vel = new Vector2(UnityEngine.Random.Range(0, 0.13f), UnityEngine.Random.Range(-.06f, 0.06f));
    }
    count += 10;

    bunniesBuffer.SetData(_bunniesArray);

    bunniesComputeShader.SetBuffer(0, "bunnies", bunniesBuffer);
    mat.SetBuffer("bunnies", bunniesBuffer);
  }

  void Update()
  {
    fpsText.text = "FPS: " + ((int)(1 / Time.smoothDeltaTime));

    bunniesComputeShader.Dispatch(0, Mathf.CeilToInt(count / 64f), 1, 1);
    Graphics.DrawMeshInstancedProcedural(mesh, 0, mat, bounds, count, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null, UnityEngine.Rendering.LightProbeUsage.Off, null);

    //Add bunnies while over 59fps
    if (1 / Time.smoothDeltaTime > 59)
    {
      if (count < max)
      {
        count += 1000;
      }

      bunnyText.text = "Bunnies: " + count;
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
    if (bunniesBuffer != null)
      bunniesBuffer.Release();
  }
}
