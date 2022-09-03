using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.UI;
using Unity.Burst;


public class Scene : MonoBehaviour
{
  static readonly float gravity = 0.5f;
  static Unity.Mathematics.Random rng;

  public Text fpsText;
  public Text bunnyText;

  public ParticleSystem ps;
  private UpdateParticlesJob job = new UpdateParticlesJob();

  static float minX = 0;
  static float maxX;
  static float minY = 0;
  static float maxY;

  GameObject bunnyPrefab;

  // Start is called before the first frame update
  void Start()
  {
    rng = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
    maxY = -(2f * Camera.main.orthographicSize);
    maxX = 2f * Camera.main.orthographicSize * Camera.main.aspect;
    job.minX = minX;
    job.maxX = maxX;
    job.minY = minY;
    job.maxY = maxY;
    job.rng = rng;
  }

  // Update is called once per frame
  void Update()
  {
    fpsText.text = "FPS: " + ((int)(1 / Time.smoothDeltaTime));

    // Add bunnies while over 59fps
    if (1 / Time.smoothDeltaTime > 59)
    {
      for (int i = 0; i < 100; i++)
      {
        ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
        ep.position = new Vector3(0, 0);
        ep.velocity = new Vector3((float)rng.NextDouble() * 10f, (float)rng.NextDouble() * 10f - 5f);
        ps.Emit(ep, 1);
      }
      bunnyText.text = "Bunnies: " + ps.particleCount;
    }
    job.ScheduleBatch(ps, 2048);
  }

  [BurstCompile]
  struct UpdateParticlesJob : IJobParticleSystemParallelForBatch
  {
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public Unity.Mathematics.Random rng;

    public void Execute(ParticleSystemJobData particles, int start, int count)
    {
      var positionsX = particles.positions.x;
      var positionsY = particles.positions.y;
      var velocitiesX = particles.velocities.x;
      var velocitiesY = particles.velocities.y;
      var rotationsZ = particles.rotations.z;

      int end = start + count;
      for (int i = start; i < end; i++)
      {

        velocitiesY[i] -= gravity;

        if (positionsX[i] > maxX)
        {
          velocitiesX[i] *= -1;
          positionsX[i] = maxX;
        }
        else if (positionsX[i] < minX)
        {
          velocitiesX[i] *= -1;
          positionsX[i] = minX;
        }

        if (positionsY[i] < maxY)
        {
          velocitiesY[i] *= -0.85f;
          positionsY[i] = maxY;
          rotationsZ[i] = (float)rng.NextDouble() * 0.2f - 0.1f;
          if ((float)rng.NextDouble() > 0.5f)
          {
            velocitiesY[i] += (float)rng.NextDouble() * 6f;
          }
        }
        else if (positionsY[i] > minY)
        {
          velocitiesY[i] = 0;
          positionsY[i] = minY;
        }
      }
    }
  }
}
