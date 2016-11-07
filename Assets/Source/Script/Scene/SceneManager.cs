using UnityEngine;
using System.Collections.Generic;

public class SceneManager : Singleton<SceneManager> {

    public GameObject ballPrefab;
    public GameObject foodPrefab;

    public GameObject itemsGroup;

    private Ball ball;
    private Food food;

    public void addBall() {
        if (this.ball == null) {
            GameObject ball = (GameObject)Instantiate(ballPrefab);
            ball.transform.parent = itemsGroup.transform;
            this.ball = ball.GetComponent<Ball>();
        }
    }

    public void takeBall() {
        if (ball != null) {
            Destroy(ball.gameObject);
        }
        ball = null;
    }

    public void addFood() {
        if (this.food == null) {
            GameObject food = (GameObject)Instantiate(foodPrefab);
            food.transform.parent = itemsGroup.transform;
            this.food = food.GetComponent<Food>();
        }
    }

    public void takeFood() {
        if (food != null) {
            Destroy(food.gameObject);
        }
        food = null;
    }

    public List<Item> getItems() {
        List<Item> items = new List<Item>();
        items.Add(ball);
        items.Add(food);
        return items;
    }
}
