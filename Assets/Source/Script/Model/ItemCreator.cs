using UnityEngine;
using System.Collections.Generic;
using System;

public class ItemCreator {

    public static readonly string ItemIdentifier = "ItemActionsRewards";

    private List<Item> _items;
    public List<Item> items { get; set; }

    private string[][] _itemsCSV;

    public ItemCreator(string itemCSVPath)
    {
        _itemsCSV = CSV.read(Application.dataPath + itemCSVPath);
        getAllItems(_itemsCSV);
    }

    private void getAllItems(string[][] itemsCSV)
    {
        items = new List<Item>();
        Item item = new Item();
        Activity activity;
        string[] itemAction;
        bool itemExists = false;

        int start = -1;
        string identifier = null;

        for (int i = 0; (i < itemsCSV.GetLength(0)) && (start == -1); i++)
        {
            if (Array.IndexOf(itemsCSV[i], ItemIdentifier) != -1)
            {
                start = i;
                identifier = "item";
            }

            if (start != -1)
            {
                for (int k = start + 1; k < itemsCSV.GetLength(0) && !String.IsNullOrEmpty(itemsCSV[k][0]); k++)
                {
                    switch (identifier)
                    {
                        case "item":
                            itemAction = itemsCSV[k][0].Split(':');
                            foreach(Item itemName in items)
                            {
                                if (itemName.name.Equals(itemAction[0]))
                                {
                                    itemExists = true;
                                    item = itemName;
                                }
                            }
                            if (itemExists == false)
                            {
                                item = new Item();
                                item.name = itemAction[0];
                            }

                            activity = new Activity("");

                            for (int j = 1; j < itemsCSV[k].Length; j++)
                                {
                                    if (itemsCSV[start][j] != "feedBackString")
                                    {
                                        activity.AddReward(itemsCSV[start][j], Int32.Parse(itemsCSV[k][j]));
                                    }
                                    else
                                    {
                                        activity.feedBackString = itemsCSV[k][j];
                                    }
                                }
                            item.AddActivity(itemAction[1], activity);
                            if(itemExists == false)
                            {
                                items.Add(item);
                            }
                            else
                            {
                                itemExists = false;
                            }
                            break;
                        default:
                            break;
                    }
                    i = k;
                }
            }
            start = -1;
        }
    }

}
