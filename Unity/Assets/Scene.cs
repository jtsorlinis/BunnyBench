using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Scene : MonoBehaviour
{
  float gravity = 0.5f;

  public Text fpsText;
  public Text bunnyText;

  public ParticleSystem ps;
  ParticleSystem.Particle[] m_Particles;

  float minX = 0;
  float maxX;
  float minY = 0;
  float maxY;

  GameObject bunnyPrefab;

  // Start is called before the first frame update
  void Start()
  {
    maxY = -(2f * Camera.main.orthographicSize);
    maxX = 2f * Camera.main.orthographicSize * Camera.main.aspect;
  }

  // Update is called once per frame
  void Update()
  {
    fpsText.text = "FPS: " + ((int)(1 / Time.smoothDeltaTime));

    if (m_Particles == null || m_Particles.Length < ps.main.maxParticles)
    {
      m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    int numParticles = ps.GetParticles(m_Particles);

    for (int i = 0; i < numParticles; i++)
    {
      m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y - gravity);

      if (m_Particles[i].position.x > maxX)
      {
        m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, m_Particles[i].velocity.y);
        m_Particles[i].position = new Vector2(maxX, m_Particles[i].position.y);
      }
      else if (m_Particles[i].position.x < minX)
      {
        m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, m_Particles[i].velocity.y);
        m_Particles[i].position = new Vector2(minX, m_Particles[i].position.y);
      }

      if (m_Particles[i].position.y < this.maxY)
      {
        m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y * -0.85f);
        m_Particles[i].position = new Vector2(m_Particles[i].position.x, maxY);
        m_Particles[i].rotation = Random.Range(-6f, 6f);
        if (Random.Range(0f, 1f) > 0.5f)
        {
          m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y + Random.Range(0f, 6f));
        }
      }
      else if (m_Particles[i].position.y > this.minY)
      {
        m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, minY);
        m_Particles[i].position = new Vector2(m_Particles[i].position.x, minY);
      }

    }
    ps.SetParticles(m_Particles, numParticles);

    // Add bunnies while over 59fps
    if (1 / Time.smoothDeltaTime > 59)
    {
      for (int i = 0; i < 100; i++)
      {
        ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
        ep.position = new Vector3(0, 0);
        ep.velocity = new Vector3(Random.Range(0f, 10f), Random.Range(-5f, 5f));
        ps.Emit(ep, 1);
      }
      bunnyText.text = "Bunnies: " + ps.particleCount;
    }
  }
}
