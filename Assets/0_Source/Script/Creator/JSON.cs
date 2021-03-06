﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using FullSerializer;

public class JSON {

    private fsSerializer serializer;

    public Personality personality;
    public List<Item> itemList;

    public JSON(Personality personality, List<Item> itemList)
    {
        serializer = new fsSerializer();
        if(personality != null)
        {
            this.personality = personality;
        }
        if (itemList != null)
        {
            this.itemList = itemList;
        }

    }

    public bool writeJSON(JSON json, string SaveFile)
    {
        string path = Path.Combine(Application.persistentDataPath, "savestates");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path = Path.Combine(path, SaveFile);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }


        string pathToFile;
        StringBuilder sb = new StringBuilder();
        fsData data;

        //write whole Personality
        serializer.TrySerialize(json.personality, out data);
        sb.Append(fsJsonPrinter.CompressedJson(data));
        pathToFile = Path.Combine(path, SaveFile + "_personality.json");
        File.WriteAllText(pathToFile, sb.ToString());
        
        sb.Length = 0;

        //write itemList
        serializer.TrySerialize(json.itemList, out data);
        sb.Append(fsJsonPrinter.CompressedJson(data));
        pathToFile = Path.Combine(path, SaveFile + "_itemList.json");
        File.WriteAllText(pathToFile, sb.ToString());

        sb.Length = 0;
        

        DebugController.Instance.Log("Saved in " + Application.persistentDataPath, DebugController.DebugType.System);
        return true;
    }


    public bool readJSON(JSON json, string LoadFile)
    {
        json.personality = new Personality();
        json.itemList = new List<Item>();

        string path = Path.Combine(Application.persistentDataPath, "savestates");
        if (!Directory.Exists(path))
        {
            //Directory.CreateDirectory(path);
            Debug.LogError("The savestates folder does not exist!");
        }
        path = Path.Combine(path, LoadFile);
        if (!Directory.Exists(path)) {
            //Directory.CreateDirectory(path);
            Debug.LogError("There are no saved stats under this name!");
        }

        string pathToFile;
        string jsonText;

        //read itemList
        pathToFile = Path.Combine(path, LoadFile + "_itemList.json");
        if (!File.Exists(pathToFile))
        {
            Debug.LogError("There is no itemList_savefile with that name!");
        }
        jsonText = File.ReadAllText(pathToFile);
        fsData data = fsJsonParser.Parse(jsonText);
        serializer.TryDeserialize(data, ref json.itemList);

        //read Personality
        pathToFile = Path.Combine(path, LoadFile + "_personality.json");
        if (!File.Exists(pathToFile))
        {
            Debug.LogError("There is no personality_savefile with that name!");
        }
        jsonText = File.ReadAllText(pathToFile);
        data = fsJsonParser.Parse(jsonText);
        serializer.TryDeserialize(data, ref json.personality);



        return true;
    }
    


}
