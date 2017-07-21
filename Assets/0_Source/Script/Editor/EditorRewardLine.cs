using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorRewardLine {

    int[] selection = new int[10];

    public int RewardID;
    public int RewardValue = 0;
    public int Status = 0;

    public int minHealth = 0;
    public int maxHealth = 0;
    public int minHunger = 0;
    public int maxHunger = 0;
    public int minSatisfaction = 0;
    public int maxSatisfaction = 0;
    public int minSocial = 0;
    public int maxSocial = 0;
    public int minEnergy = 0;
    public int maxEnergy = 0;

    static EditorRewardLine()
    {

    }

    public void setRewards(int rid, int rv, int stat)
    {
        this.RewardID = rid;
        this.RewardValue = rv;
        this.Status = stat;

    }


}
