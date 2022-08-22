using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Scene : MonoBehaviour {

  public Text fpsText;
  public Text bunnyText;

  int startBunnyCount = 10;
  int count;

  GameObject bunnyPrefab;

  // Start is called before the first frame update
  void Start() {
    count = startBunnyCount;

    // Load
    bunnyPrefab = Resources.Load<GameObject>("BunnyPrefab");

    // Draw
    for (int i = 0; i < startBunnyCount; i++) {
      Instantiate(bunnyPrefab, new Vector2(0, -.2f), Quaternion.identity);
    }
  }

  // Update is called once per frame
  void Update() {
    fpsText.text = "FPS: " + ((int)(1 / Time.smoothDeltaTime));

    // Add bunnies while over 59fps
    if (1 / Time.deltaTime > 59) {
      if (this.count < 200000) {
        for (int i = 0; i < 100; i++) {
          Instantiate(bunnyPrefab, new Vector2(0, -.2f), Quaternion.identity);
          count++;
        }
      }
      bunnyText.text = "Bunnies: " + count;
    }
  }
}
