using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameData {

    private string personalityCSVPath = "Data\\";
    private string saveFile;
    private string loadFile;

    public Personality Person { get; set; }
    public ArtificialIntelligence Intelligence { get; set; }
    public List<Item> Items { get; set; }

    public GameData (LoadStates LoadFrom) {

        saveFile = "savefile";
        loadFile = "savefile";

        Items = new List<Item>();
        loadPersonality(LoadFrom);
        Intelligence = new ArtificialIntelligence();
    }

    public void SaveData() {
        JSON writeJSON = new JSON(Person, Items);
        writeJSON.writeJSON(writeJSON, saveFile);
    }

    private void loadPersonality(LoadStates LoadFrom)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Path.Combine(Application.persistentDataPath, "savestates");
            if (!Directory.Exists(path))
            {
                LoadFrom = LoadStates.CSV;
            }
            path = Path.Combine(path, saveFile);
            if (!Directory.Exists(path))
            {
                LoadFrom = LoadStates.CSV;
            }
            else
            {
                LoadFrom = LoadStates.SavedState;
            }
        }

        switch (LoadFrom)
        {
            case LoadStates.Dummy:
                DummyCreator creator = new DummyCreator();
                Person = creator.CreateDummyPerson();
                Items = creator.CreateDummyItems();
                break;

            case LoadStates.CSV:
                PersonalityCreator creatorCSV = new PersonalityCreator(personalityCSVPath);
                Person = creatorCSV.Personality;
                Items = creatorCSV.ItemList;
                foreach(KeyValuePair<EmotionType, Emotion> kvp in Person.Emotions)
                {
                    kvp.Value.Items = Items;
                }
                Dictionary<TraitType, Trait> traitList = creatorCSV.TraitList;

                //Person.AddTrait(traitList[TraitType.WILD], Items);
                //foreach (KeyValuePair<TraitType, Trait> kvp in traitList)
                //{
                //    if (Random.value <= 0.33)
                //    {
                //        Person.AddTrait(kvp.Value, Items);
                //    }
                //}
                break;

            case LoadStates.SavedState:
                JSON readJSON = new JSON(Person, Items);
                readJSON.readJSON(readJSON, loadFile);
                Person = readJSON.personality;
                Items = readJSON.itemList;
                break;

            default:
                Debug.LogError("No Load State");
                break;
        }
        string traits = "Traits in Personality: ";
        foreach (Trait trait in Person.Traits)
        {
            traits += trait.Identifier + " | ";
        }
        DebugController.Instance.Log(traits, DebugController.DebugType.System);

    }
}
