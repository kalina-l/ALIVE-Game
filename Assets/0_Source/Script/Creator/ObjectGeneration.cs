using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public class ObjectGeneration : MonoBehaviour {

    public bool _generatNew;
    public bool _clearAll;

    //set -1 to generate random
    public int _generationSeed = -1;

    public GameObject[] _object;
    public int _number;

    public float _minScaling = 1f;
    public float _maxScaling = 1f;

    void Start()
    {
        Generate();
    }
    // Update is called once per frame
    void Update()
    {
        if (_generatNew)
        {
            Generate();
            _generatNew = false;
        }
        if (_clearAll)
        {
            DestroyAll();
            _clearAll = false;
        }
    }

    // Use this for initialization
    void Generate () {
        if (_generationSeed == -1)
        {
            int random = Random.Range(0, 100000);
            Random.InitState(random);
            Debug.Log("Random Generation Seed: " + random);
        }
        else
        {
            Random.InitState(_generationSeed);
        }

        Bounds meshBounds = GetComponent<MeshCollider>().sharedMesh.bounds;
        DestroyAll();
        
        for (int i = 0; i < _number; i++)
        {
            Vector3 newPosition = new Vector3(Random.Range(meshBounds.min.x, meshBounds.max.x), Random.Range(meshBounds.min.y, meshBounds.max.y), Random.Range(meshBounds.min.z, meshBounds.max.z));
            int objInd = Random.Range(0, _object.Length);
            GameObject obj = Instantiate(_object[objInd], newPosition, Quaternion.identity, transform) as GameObject;
            obj.transform.localScale *= Random.Range(_minScaling, _maxScaling);
        }
	}

    void DestroyAll()
    {
        if (transform.childCount > 0)
        {
            for (int i = transform.childCount-1; i >= 0; i-- )
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
	
}
