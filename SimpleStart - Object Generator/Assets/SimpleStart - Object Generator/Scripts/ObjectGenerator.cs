using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/*
    Asset Title: SimpleStart - Object Generator 
    Version: Version: 1.0
    Author: Andrew Wilkinson
    
    Description: A simple solution for generating objects that have a default state and/or multiple special types 
    (eg. walls & tiles) via a prefab with inactive nested objects within it, activating them based on set probability.
    Once desired objects are generated in scene they can be placed and randomized again without affecting their scene
    positions, alternatively there is functionality to assemble all the objects into a grid based on user parameters 
    assigned in the editor.
*/ 

namespace Scripts
{
    public class ObjectGenerator : MonoBehaviour
    {
        // The child objects in the current scene that we have generated
        public List<GameObject> ObjectsInScene = new List<GameObject>();
        // A collection of unique object types for use in the editor
        public List<EditorObject> EditorObjects = new List<EditorObject>();
        // This is the prefab that is used by the script for generation
        public GameObject ObjectToGenerate;
        // Width of our gameobject (via our transform)
        public float SizeX { get; set; }
        // Length of our object (via our trasform)
        public float SizeZ { get; set; }    
        // Object heights in scene (via our trasform)
        public float HeightY { get; set; }    
        // The width of our grid
        public int GridWidth { get; set; }
        // Property allow for setting a custom quantity of objects to generate
        public int ObjectCount { get; set; }
        // Property to show how many unique objects we are using within our prefab
        public int NumberOfNestedObjects { get; set; }

        private float _cumulativePercentages = 0;
        
        public void GenerateDefaultObjects(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                GameObject obj = Instantiate<GameObject>(ObjectToGenerate, transform);
                obj.transform.name = "GenericObject";
                ObjectsInScene.Add(obj);
            }
        }
      
        public void GenerateObjects()
        {
            foreach (var obj in ObjectsInScene)
            {
                for (int i = 0; i < NumberOfNestedObjects; i++)
                {
                    GameObject child = obj.transform.Find(EditorObjects[i].Name).gameObject;
                    child.SetActive(false);
                }
            }
            
            Debug.Log("Generating objects");
            
            _cumulativePercentages = 0;
            
            foreach (var obj in ObjectsInScene)
            {
                int randomNumber = Random.Range(1, 101);

                string defName = EditorObjects[0].Name;

                if (randomNumber <= EditorObjects[0].Percentage)
                {
                    GameObject child = obj.transform.Find(defName).gameObject;
                    child.GetComponentInParent<Transform>().name = defName;
                    child.SetActive(true);
                }
                else
                {
                    for (int i = 1; i <= EditorObjects.Count; i++)
                    {
                        for (int j = 0; j <= i; j++)
                        {
                            _cumulativePercentages = _cumulativePercentages + EditorObjects[j].Percentage;
                        }

                        if (randomNumber <= _cumulativePercentages)
                        {
                            GameObject child = obj.transform.Find(EditorObjects[i].Name).gameObject;
                            child.GetComponentInParent<Transform>().name = EditorObjects[i].Name;
                            child.SetActive(true);
                            break;
                        }
                        _cumulativePercentages = 0;
                    }
                    _cumulativePercentages = 0;
                } 
            }
        }

        public void SetObjectsToDefault()
        {
            Debug.Log("Setting objects to default state");

            foreach (var obj in ObjectsInScene)
            {
                foreach (var editorObj in EditorObjects)
                {
                    obj.transform.Find(editorObj.Name).gameObject.SetActive(false);
                }
                obj.transform.name = EditorObjects[0].Name;
                obj.transform.Find(EditorObjects[0].Name).gameObject.SetActive(true);
            }
        }
        public void ResetFilters()
        {
            Debug.Log("Resetting filters");
            
                EditorObjects.Clear();
                ObjectsInScene.Clear();
        }

        public void GenerateUniqueObjects(int count)
        {
            Debug.Log("Setting up unique objects");
            
            for (int i = 0; i < count; i++)
            {
                EditorObjects.Add(new EditorObject());
            }
        }

        public void ProcessExistingObjects()
        {
            Debug.Log("Processing existing objects in scene");
            
            foreach (Transform child in transform)
            {
                ObjectsInScene.Add(child.gameObject);
            }
        }
        
        public void TransformToGrid()
        {
            Debug.Log("Creating 3D grid of generated objects");
            
            float zAxis = 0;
            float xAxis = 0;
            int count = 0;
            foreach (var obj in ObjectsInScene)
            {
                obj.transform.position = new Vector3(SizeX * xAxis, HeightY, zAxis * SizeZ);
                xAxis++;
                count++;

                if (count == (GridWidth))
                {
                    zAxis++;
                    xAxis = 0;
                    count = 0;
                }
            }
        }
        
        public class EditorObject
        {
            public string Name { get; set; }
            public float Percentage { get; set; }
        }
    }
}