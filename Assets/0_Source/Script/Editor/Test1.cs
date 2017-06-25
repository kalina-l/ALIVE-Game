using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class Test1 : EditorWindow
{
    Rect buttonpos = new Rect(2,40,80,40);

    int buttonValue = 0;

    int[] selection = new int[10];
    //int selection = 0;
    //Rect optionPos = new Rect(0, 160, 100, 40);

    Rect[] optionPos = new Rect[10];
    Rect[] textAreaPos = new Rect[10];

    string[] itemList = { "Item1", "Item2", "Item3" };

    string[] textAreaLabels = { "minHealth", "maxHealth",
                                "minHunger", "maxHunger",
                                "minSatisfact.", "maxSatisfact.",
                                "minSocial", "maxSocial",
                                "minEnergy", "maxEnergy",
    }; 

    string[] arrayItemOptions = {   "None", "SUICIDAL", "SUPER_BAD", "VERY_BAD", "BAD",
                                    "NEUTRAL", "GOOD", "VERY_GOOD", "SUPER_GOOD" };

    //string[] moreLabels = { "Reward ID", "Value", "Type" };



    [MenuItem("Window/MyWindows/TestWindow")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Test1));
    }

    void OnGUI()
    {

        //Rect contextRect = new Rect(10, 10, 100, 100);
        //EditorGUI.DrawRect(contextRect, Color.red);
        //GUILayout.Button(buttonpos, ItelmList[0]);

        GUILayout.Label("Select Item", EditorStyles.boldLabel);
        buttonValue = EditorGUI.Popup(buttonpos, buttonValue, itemList);
        //GUILayout.Label("Item Properties", EditorStyles.boldLabel);

        textAreaLine(140);


        //selection = EditorGUI.Popup(optionPos, selection, arrayItemOptions);
        OptionLine(160);
        OptionLine(200);
        OptionLine(240);

        // GUILayout.TextField("Test");

    }

    void textAreaLine(int ypos)
    {
        textAreaPos[0] = new Rect(0, ypos, 100, 20);
        textAreaPos[1] = new Rect(100, ypos, 100, 20);
        textAreaPos[2] = new Rect(200, ypos, 100, 20);
        textAreaPos[3] = new Rect(300, ypos, 100, 20);
        textAreaPos[4] = new Rect(400, ypos, 100, 20);
        textAreaPos[5] = new Rect(500, ypos, 100, 20);
        textAreaPos[6] = new Rect(600, ypos, 100, 20);
        textAreaPos[7] = new Rect(700, ypos, 100, 20);
        textAreaPos[8] = new Rect(800, ypos, 100, 20);
        textAreaPos[9] = new Rect(900, ypos, 100, 20);

        for (int i = 0; i < 10; i++)
        {
            EditorGUI.TextArea(textAreaPos[i], textAreaLabels[i], EditorStyles.boldLabel);
        }
    }

    void OptionLine(int ypos)
    {
        //selection[0] = EditorGUI.Popup(optionPos[0], selection[0], arrayItemOptions);
        //selection[1] = EditorGUI.Popup(optionPos[1], selection[1], arrayItemOptions);
        /*
        for (int i = 0; i < 10; i++)
        {
            selection[i] = 0;
        }*/

        optionPos[0] = new Rect(0, ypos, 100, 40);
        optionPos[1] = new Rect(100, ypos, 100, 40);
        optionPos[2] = new Rect(200, ypos, 100, 40);
        optionPos[3] = new Rect(300, ypos, 100, 40);
        optionPos[4] = new Rect(400, ypos, 100, 40);
        optionPos[5] = new Rect(500, ypos, 100, 40);
        optionPos[6] = new Rect(600, ypos, 100, 40);
        optionPos[7] = new Rect(700, ypos, 100, 40);
        optionPos[8] = new Rect(800, ypos, 100, 40);
        optionPos[9] = new Rect(900, ypos, 100, 40);

        for (int i = 0; i < 10; i++)
        {
            selection[i] = EditorGUI.Popup(optionPos[i], selection[i], arrayItemOptions);
        }

    }
}


