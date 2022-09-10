using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

  private ComputeBuffer positionBuffer;
  private ComputeBuffer argsBuffer;
  private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

  Mesh mesh;
  Bounds bounds = new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f));

  List<Vector4> positions = new List<Vector4>();
  List<Vector4> velocities = new List<Vector4>();

  void Start()
  {
    argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
    mesh = MakeQuad(.7f, .7f);

    for (int i = 0; i < 10; i++)
    {
      positions.Add(new Vector4(minX, minY, 0, 0));
      velocities.Add(new Vector4(Random.Range(0, 0.13f), Random.Range(-.06f, 0.06f)));
    }
    positionBuffer = new ComputeBuffer(positions.Count, 16);

  }

  void Update()
  {

    fpsText.text = "FPS: " + ((int)(1 / Time.smoothDeltaTime));

    if (positionBuffer.count < positions.Count)
    {
      positionBuffer.Release();
      positionBuffer = new ComputeBuffer(positions.Count, 16);
    }

    for (int i = 0; i < positions.Count; i++)
    {
      Vector4 pos = positions[i];
      Vector4 vel = velocities[i];

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
        pos.w = Random.Range(-0.1f, 0.1f); // Passing rotation through w value of position
        if (Random.Range(0f, 1f) > 0.5f)
        {
          vel.y += Random.Range(0f, .1f);
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

    // Draw
    positionBuffer.SetData(positions.ToArray());
    mat.SetBuffer("positionBuffer", positionBuffer);

    // Indirect args
    if (mesh != null)
    {
      args[0] = (uint)mesh.GetIndexCount(0);
      args[1] = (uint)positions.Count;
      args[2] = (uint)mesh.GetIndexStart(0);
      args[3] = (uint)mesh.GetBaseVertex(0);
    }
    else
    {
      args[0] = args[1] = args[2] = args[3] = 0;
    }
    argsBuffer.SetData(args);

    Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, bounds, argsBuffer, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null, UnityEngine.Rendering.LightProbeUsage.Off, null);

    // Add bunnies while over 59fps
    if (1 / Time.smoothDeltaTime > 59)
    {
      for (int i = 0; i < 100; i++)
      {
        positions.Add(new Vector4(minX, minY, 0, 0));
        velocities.Add(new Vector4(Random.Range(0, 0.13f), Random.Range(-.06f, 0.06f)));
      }
      bunnyText.text = "Bunnies: " + positions.Count;
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

    if (argsBuffer != null)
      argsBuffer.Release();
    argsBuffer = null;
  }
}
