using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Scene : MonoBehaviour {

  public Text fpsText;
  public Text bunnyText;

  List<Bunny> bunnies = new List<Bunny>();

  int startBunnyCount = 10;
  int count;

  float minX;
  float maxX;
  float minY;
  float maxY;

  float gravity = 0.008f;

  Sprite bunnySprite;

  // Start is called before the first frame update
  void Start() {
    count = startBunnyCount;

    // get bounds
    minX = .13f;
    minY = -.19f;

    maxY = -(2f * Camera.main.orthographicSize + minY);
    maxX = (-maxY * Camera.main.aspect) - minX;

    // Load sprite
    bunnySprite = Resources.Load<Sprite>("bunny");

    // Draw sprites
    for (int i = 0; i < startBunnyCount; i++) {
      Bunny bunny = new Bunny();
      bunny.go = new GameObject();
      bunny.go.name = "bunny" + i;
      SpriteRenderer sp = bunny.go.AddComponent<SpriteRenderer>();
      sp.sprite = bunnySprite;

      bunny.speedX = Random.Range(0f, 10f) / 60;
      bunny.speedY = Random.Range(-5f, 5f) / 60;
      bunny.go.transform.position = new Vector2(0.1f, -.2f);
      bunnies.Add(bunny);
    }

  }

  // Update is called once per frame
  void Update() {
    fpsText.text = "FPS: " + ((int)(1 / Time.deltaTime));

    // Add bunnies while over 60fps
    if (1 / Time.deltaTime > 60) {
      if (this.count < 200000) {
        for (int i = 0; i < 100; i++) {
          Bunny bunny = new Bunny();
          bunny.go = new GameObject();
          bunny.go.name = "bunny" + count;
          SpriteRenderer sp = bunny.go.AddComponent<SpriteRenderer>();
          sp.sprite = bunnySprite;

          bunny.speedX = Random.Range(0f, 10f) / 60;
          bunny.speedY = Random.Range(-5f, 5f) / 60;
          bunny.go.transform.position = new Vector2(0.1f, -.2f);
          bunnies.Add(bunny);

          count++;
        }
      }
      bunnyText.text = "Bunnies: " + count;
    }


    // Move bunnies
    for (int i = 0; i < bunnies.Count; i++) {
      Bunny bunny = bunnies[i];

      bunny.go.transform.position += new Vector3(bunny.speedX, bunny.speedY, 0);
      bunny.speedY -= gravity;

      if (bunny.go.transform.position.x > maxX) {
        bunny.speedX *= -1;
        bunny.go.transform.position = new Vector2(maxX, bunny.go.transform.position.y);
      } else if (bunny.go.transform.position.x < minX) {
        bunny.speedX *= -1;
        bunny.go.transform.position = new Vector2(minX, bunny.go.transform.position.y);
      }

      if (bunny.go.transform.position.y < this.maxY) {
        bunny.speedY *= -0.85f;
        bunny.go.transform.position = new Vector2(bunny.go.transform.position.x, maxY);
        bunny.go.transform.eulerAngles = new Vector3(0, 0, Random.Range(-6f, 6f));
        if (Random.Range(0f, 1f) > 0.5f) {
          bunny.speedY += Random.Range(0f, .1f);
        }
      } else if (bunny.go.transform.position.y > this.minY) {
        bunny.speedY = 0;
        bunny.go.transform.position = new Vector2(bunny.go.transform.position.x, minY);
      }
    }
  }
}
