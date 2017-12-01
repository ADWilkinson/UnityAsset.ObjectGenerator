using UnityEditor;
using UnityEngine;

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

namespace Scripts.Editor
{
    [CustomEditor(typeof(ObjectGenerator))]
    [CanEditMultipleObjects]
    public class ObjectGeneratorEditor : UnityEditor.Editor
    {
        public ObjectGenerator Generator;

        private bool _goToStep2;
        private bool _goToStep3;

        private void OnEnable()
        {
            Generator = (ObjectGenerator) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // How many instances of the the object do we want to generate 
            Generator.ObjectCount = EditorGUILayout.IntField("# Prefabs to Generate: ", Generator.ObjectCount);

            EditorGUILayout.Space();
            
            // Grid Parameters for basic grid generation
            EditorGUILayout.LabelField("Grid Params (GPrm): (Only required for grid generation)");
            Generator.SizeX = EditorGUILayout.FloatField("GPrm: Our Object Size (X)", Generator.SizeX);
            Generator.SizeZ = EditorGUILayout.FloatField("GPrm: Our Object Size (Z)", Generator.SizeZ);
            Generator.HeightY = EditorGUILayout.FloatField("GPrm: Object Y Axis Position: ", Generator.HeightY);
            Generator.GridWidth = EditorGUILayout.IntField("GPrm: Grid Width (In prefab #)", Generator.GridWidth);
            
            // How many unique game objects are nested within our prefabs  that we generate
            Generator.NumberOfNestedObjects = Generator.ObjectToGenerate.transform.childCount;

            EditorGUILayout.Space();

            // Here we set up the instatiate the unique object templates into a list for later use
            if (GUILayout.Button("Set Prefab Properties Up"))
            {
                Generator.GenerateUniqueObjects(Generator.NumberOfNestedObjects);
            }
            
            if (Generator.EditorObjects.Count > 0) 
            {
                EditorGUILayout.LabelField($"Done: There are {Generator.EditorObjects.Count} nested objects within your prefab");
                EditorGUILayout.Space();
            }
            
            // Here we generate the desired quantity of prefab objects set to their default state
            if (GUILayout.Button("Generate Generic Objects"))
            {
                Generator.GenerateDefaultObjects(Generator.ObjectCount);
            }  
            
            // If we already have generated objects in the scene under the GeneratorObject then we can process them here
            if (GUILayout.Button("Process Existing Objects"))
            {
                Generator.ProcessExistingObjects();
            }    
            
            if (Generator.ObjectsInScene.Count > 0) 
            {
                EditorGUILayout.LabelField($"Done: There are {Generator.ObjectsInScene.Count} generated objects in the scene");
                EditorGUILayout.Space();
            }
            
            EditorGUILayout.Space();
            
            _goToStep2 = EditorGUILayout.Toggle("Proceed: Step 2", _goToStep2);

            EditorGUILayout.Space();
            
            if (_goToStep2)
            {
                // User input for the name of our default object within the prefab
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Default Object Name: ", GUILayout.Width(175));
                Generator.EditorObjects[0].Name = GUILayout.TextArea(Generator.EditorObjects[0].Name);
                EditorGUILayout.EndHorizontal();

                // User input for the name of our special objects within the prefab
                for (int i = 1; i < Generator.EditorObjects.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(i + "- Special Object Name: ", GUILayout.Width(175));
                    Generator.EditorObjects[i].Name = GUILayout.TextArea(Generator.EditorObjects[i].Name);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

                // User input for the percentage chance of a generated object being our default type
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Default Object %: ", GUILayout.Width(150));
                Generator.EditorObjects[0].Percentage =
                    EditorGUILayout.Slider(Generator.EditorObjects[0].Percentage, 0, 100);
                EditorGUILayout.EndHorizontal();

                /* User input for the percentage chance of a generated object being our special types */
                for (int i = 1; i < Generator.EditorObjects.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Special Obj #{i} %: ", GUILayout.Width(150));
                    Generator.EditorObjects[i].Percentage =
                        EditorGUILayout.Slider(Generator.EditorObjects[i].Percentage, 0, 100);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

                _goToStep3 = EditorGUILayout.Toggle("Proceed: Step 3", _goToStep3);

                if (_goToStep3)
                {
                    int fieldsValidated = 0;

                    foreach (var obj in Generator.EditorObjects)
                    {
                        if (obj.Name != null)
                        {
                            fieldsValidated++;
                        }
                    }

                    if (fieldsValidated == Generator.EditorObjects.Count)
                    {
                        _goToStep2 = true;
                        _goToStep3 = true;
                        EditorGUILayout.Space();

                        // Here we use the objects generated in the previous method and start set their types based on probability.
                        if (GUILayout.Button("Set Objects: Randomize Object Types"))
                        {
                            Generator.GenerateObjects();
                        }

                        // This method allows us to reset all the objects back to the default state without any regeneration neeeded.
                        if (GUILayout.Button("Set Objects: Default Object Type"))
                        {
                            if (EditorUtility.DisplayDialog("Confirm Default",
                                "Are you sure you want to set all " + Generator.ObjectsInScene.Count +
                                " objects to their default state?",
                                "Yes", "No"))
                            {
                                Generator.SetObjectsToDefault();
                            }
                        }
                        
                        // Gives the user the option to create a basic grid layout with the generated objects
                        if (GUILayout.Button("Transform Objects: To Grid"))
                        {
                            if (EditorUtility.DisplayDialog("Confirm Transform",
                                "Are you sure you want to move all " + Generator.ObjectsInScene.Count +
                                " objects in the current scene?",
                                "Yes", "No"))
                            {
                                Generator.TransformToGrid();
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space();

            /* This button allows us to clear all filters from the GUI to allowing to restart the process, the user will
               have to delete the gameobjects from the scene manually before continuing after. */
            if (GUILayout.Button("Clear Filters"))
            {
                if (EditorUtility.DisplayDialog("Warning",
                    "Do you want to clear all current filters? You will need to set the prefab properties up again",
                    "Yes", "No"))
                {
                    Generator.ResetFilters();
                }
            }

            EditorGUILayout.Space();
        }
    }
}