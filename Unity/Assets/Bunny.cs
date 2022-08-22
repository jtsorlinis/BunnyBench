using UnityEngine;


public class Bunny : MonoBehaviour
{
  float speedY;
  float speedX;

  float minX;
  float maxX;
  float minY;
  float maxY;

  float gravity = 0.008f;

  private void Start()
  {

    speedY = Random.Range(-5f, 5f) / 60;
    speedX = Random.Range(0f, 10f) / 60;

    // get bounds
    minX = 0;
    minY = 0;

    maxY = -(2f * Camera.main.orthographicSize);
    maxX = 2f * Camera.main.orthographicSize * Camera.main.aspect;
  }

  private void Update()
  {
    // Move
    transform.position += new Vector3(speedX, speedY, 0);
    speedY -= gravity;

    if (transform.position.x > maxX)
    {
      speedX *= -1;
      transform.position = new Vector2(maxX, transform.position.y);
    }
    else if (transform.position.x < minX)
    {
      speedX *= -1;
      transform.position = new Vector2(minX, transform.position.y);
    }

    if (transform.position.y < this.maxY)
    {
      speedY *= -0.85f;
      transform.position = new Vector2(transform.position.x, maxY);
      transform.eulerAngles = new Vector3(0, 0, Random.Range(-6f, 6f));
      if (Random.Range(0f, 1f) > 0.5f)
      {
        speedY += Random.Range(0f, .1f);
      }
    }
    else if (transform.position.y > this.minY)
    {
      speedY = 0;
      transform.position = new Vector2(transform.position.x, minY);
    }
  }
}
