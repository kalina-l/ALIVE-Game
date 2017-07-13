using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

class Test1 : EditorWindow
{ 
    // TempData
    string[] itemList = { "Item1", "Item2", "Item3", "Item4" };     // list of all Items
    int[] itemrows = { 3, 7, 5, 15 };                               // list of rows Items has

    //todo load items

    // Data
    Rect buttonpos = new Rect(2,40,80,40);  // Pos des ItemSelectButtons
    int buttonValue = 0;                    // Value of the Button, first selection
    Rect[] textAreaPos = new Rect[13];      // Position of the LabelArea

    // Holder
    int[,] selection = new int[15,14];     // 15 Lines with 13 Values for store Line selected Options
   
    // Labels
    string[] textAreaLabels = { "Reward-ID","Value","Status",
                                "minHealth", "maxHealth",
                                "minHunger", "maxHunger",
                                "minSatisfact.", "maxSatisfact.",
                                "minSocial", "maxSocial",
                                "minEnergy", "maxEnergy",
    }; 

    string[] arrayItemOptions = {   "None", "SUICIDAL", "SUPER_BAD", "VERY_BAD", "BAD",
                                    "NEUTRAL", "GOOD", "VERY_GOOD", "SUPER_GOOD" };

    string[] arrayStatusOptions = { "Health", "Hunger", "Satisfaction", "Social", "Energy" };

    // Show Window
    [MenuItem("Window/MyWindows/TestWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Test1));
    }

    // Show GUI
    void OnGUI()
    {
        // Itemauswahl
        GUILayout.Label("Select Item", EditorStyles.boldLabel);
        buttonValue = EditorGUI.Popup(buttonpos, buttonValue, itemList);
        // todo: itemchange need to reload new item probs and save the old

        // Erzeugt die Überschriften der Rewards
        textAreaLine(80);

        // Erzeugt Die (Rewards) OptionLines in Anzahl wie in itemrows vorgegeben, abhänig vom gewählten Index des Popups
        for (int i = 0; i < itemrows[EditorGUI.Popup(buttonpos, buttonValue, itemList)]; i++)
        {
            OptionLine(100 + 40 * i, i);
        }
        
    }

    // *********
    // Functions
    // *********

    // TextDescriptionArea
    void textAreaLine(int ypos)
    {
        // LabelPositionen
        for (int i = 0; i < 13; i++)
        {
            textAreaPos[i] = new Rect(0+100*i, ypos, 100, 20);
        }
   
        // LabelNahmen einsetzen
        for (int i = 0; i < 13; i++)
        {
            EditorGUI.TextArea(textAreaPos[i], textAreaLabels[i], EditorStyles.boldLabel);
        }
    }

    // OptionLine
    void OptionLine(int ypos, int lineNr)
    {
        string placeholder = selection[lineNr, 12].ToString();

        selection[lineNr, 13] = lineNr;
        // ID (selection[13])
        EditorGUI.TextArea(new Rect(0, ypos, 100, 40), selection[lineNr, 13].ToString(), EditorStyles.boldLabel);

        // Value (selection[12])
        //EditorGUI.TextField(new Rect(100, ypos, 100, 40), "0", EditorStyles.boldLabel);
        placeholder = EditorGUI.TextField(new Rect(100, ypos, 100, 40), placeholder, EditorStyles.label);
        selection[lineNr, 12] = Int32.Parse(placeholder);

        // Status Selection (selection[11])
        selection[lineNr,11] = EditorGUI.Popup(new Rect(200, ypos, 100, 40), selection[lineNr, 11], arrayStatusOptions);

        // RewardRange Selection
        for (int i = 0; i < 10; i++)
        {
            // Status Selection
            selection[lineNr,i] = EditorGUI.Popup(new Rect(300 + 100 * i, ypos, 100, 40), selection[lineNr,i], arrayItemOptions);
        }
    }

    void loadSelection(int itemNr)
    {

    }
    void saveSelection(int itemNr)
    {

    }
}


