using UnityEngine;
using System.Collections.Generic;

public class SceneManager : Singleton<SceneManager> {

    private Ball ball;
    private Food food;

    public void addBall(Ball ball) {
        this.ball = ball;
    }

    public void addFood(Food food) {
        this.food = food;
    }

    public List<Item> getItems() {
        List<Item> items = new List<Item>();
        items.Add(ball);
        items.Add(food);
        return items;
    }
}
