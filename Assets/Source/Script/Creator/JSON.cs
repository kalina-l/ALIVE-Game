﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using FullSerializer;

public class JSON {

    private fsSerializer serializer;

    public Personality personality;
    public List<Reward> rewardList;
    public List<Item> itemList;

    public JSON(Personality personality, List<Reward> rewardList, List<Item> itemList)
    {
        serializer = new fsSerializer();
        if(personality != null)
        {
            this.personality = personality;
        }
        //else
        //{
        //    Debug.LogError("No Personality found");
        //}
        if (rewardList != null)
        {
            this.rewardList = rewardList;
        }
        if (itemList != null)
        {
            this.itemList = itemList;
        }
    }

    public bool writeJSON(JSON json)
    {
        StringBuilder sb = new StringBuilder();

        fsData data;

        //write whole Personality
        serializer.TrySerialize(json.personality, out data);
        sb.Append(fsJsonPrinter.CompressedJson(data));
        PlayerPrefs.SetString("personality", sb.ToString());

        sb.Length = 0;

        //write itemList
        serializer.TrySerialize(json.itemList, out data);
        sb.Append(fsJsonPrinter.CompressedJson(data));
        PlayerPrefs.SetString("itemList", sb.ToString());

        sb.Length = 0;

        //write rewardList
        serializer.TrySerialize(json.rewardList, out data);
        sb.Append(fsJsonPrinter.CompressedJson(data));
        PlayerPrefs.SetString("rewardList", sb.ToString());

        PlayerPrefs.Save();

        return true;
    }

    public bool readJSON(JSON json)
    {
        json.personality = new Personality();
        json.rewardList = new List<Reward>();
        json.itemList = new List<Item>();

        if (!PlayerPrefs.HasKey("personality"))
        {
            Debug.LogError("Savefile not found, please save a state before trying to load!");
        }

        //read rewardList
        string jsonText = PlayerPrefs.GetString("rewardList");
        fsData data = fsJsonParser.Parse(jsonText);
        serializer.TryDeserialize(data, ref json.rewardList);

        //read itemList
        jsonText = PlayerPrefs.GetString("itemList");
        data = fsJsonParser.Parse(jsonText);
        serializer.TryDeserialize(data, ref json.itemList);

        //read Personality
        jsonText = PlayerPrefs.GetString("personality");
        data = fsJsonParser.Parse(jsonText);
        serializer.TryDeserialize(data, ref json.personality);

        return true;
    }



}