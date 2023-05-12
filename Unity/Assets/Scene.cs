using UnityEngine;
using UnityEngine.UI;

public struct Bunny
{
  public Vector4 pos;
  public Vector2 vel;
  float pad0;
  float pad1;
}

public class Scene : MonoBehaviour
{
  public int maxBunnies = 4000000;

  [Header("Prefabs")]
  public Text fpsText;
  public Text bunnyText;
  public Material mat;

  float xBound;
  float yBound;

  float gravity = 0.007f;

  public ComputeShader bunniesComputeShader;

  private ComputeBuffer bunniesBuffer;

  int count = 0;

  Mesh quad;
  Bounds bounds = new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f));

  void Start()
  {
    quad = MakeQuad();
    yBound = Camera.main.orthographicSize - 0.07f;
    xBound = Camera.main.orthographicSize * Camera.main.aspect - 0.05f;
    bunniesBuffer = new ComputeBuffer(maxBunnies, 32);

    var _bunniesArray = new Bunny[maxBunnies];

    for (int i = 0; i < maxBunnies; i++)
    {
      _bunniesArray[i].pos = new Vector4(-xBound, yBound, 0, 0);
      _bunniesArray[i].vel = new Vector2(UnityEngine.Random.Range(0, 0.13f), UnityEngine.Random.Range(-.06f, 0.06f));
    }
    count += 10;

    bunniesBuffer.SetData(_bunniesArray);

    bunniesComputeShader.SetBuffer(0, "bunnies", bunniesBuffer);
    bunniesComputeShader.SetFloat("gravity", gravity);
    bunniesComputeShader.SetFloat("xBound", xBound);
    bunniesComputeShader.SetFloat("yBound", yBound);
    mat.SetBuffer("bunnies", bunniesBuffer);
  }

  void Update()
  {
    fpsText.text = "FPS: " + ((int)(1 / Time.smoothDeltaTime));

    bunniesComputeShader.SetInt("rngSeed", Random.Range(0, int.MaxValue));
    bunniesComputeShader.Dispatch(0, Mathf.CeilToInt(count / 256f), 1, 1);
    Graphics.DrawMeshInstancedProcedural(quad, 0, mat, bounds, count, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null, UnityEngine.Rendering.LightProbeUsage.Off, null);

    //Add bunnies while over 59fps
    if (1 / Time.smoothDeltaTime > 59)
    {
      if (count < maxBunnies)
      {
        count += 1000;
      }

      bunnyText.text = "Bunnies: " + count;
    }
  }

  void OnDisable()
  {
    bunniesBuffer?.Release();
  }

  Mesh MakeQuad()
  {
    float width = .3f;
    float height = .5f;
    Mesh mesh = new Mesh();

    Vector3[] vertices = new Vector3[4]
    {
      new Vector3(-width/2, -height/2, 0),
      new Vector3(width/2, -height/2, 0),
      new Vector3(-width/2, height/2, 0),
      new Vector3(width/2, height/2, 0)
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
}
