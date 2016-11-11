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
            /*
            GameObject ball = (GameObject)Instantiate(ballPrefab);
            ball.transform.parent = itemsGroup.transform;
            this.ball = ball.GetComponent<Ball>();
            */
            this.ball = new Ball();
        }
    }

    public void takeBall() {
        
        this.ball = null;
    }

    public void addFood() {
        if (this.food == null) {
            /*
            GameObject food = (GameObject)Instantiate(foodPrefab);
            food.transform.parent = itemsGroup.transform;
            this.food = food.GetComponent<Food>();
            */
            this.food = new Food();
        }
    }

    public void takeFood() {
        
        this.food = null;
    }

    public List<Item> getItems() {
        List<Item> items = new List<Item>();
        items.Add(ball);
        items.Add(food);
        return items;
    }
}
