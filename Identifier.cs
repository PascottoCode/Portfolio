using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#pragma warning disable 0649

namespace RPG.Characters
{
    [ExecuteAlways]
    public sealed class Identifier : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";

        //a static lookup table to check for uniqueness
        static Dictionary<string, Identifier> globalLookup = new Dictionary<string, Identifier>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	    static void InitStaticFields()
	    {
		    globalLookup = new Dictionary<string, Identifier>();
	    }
	    
        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        #if UNITY_EDITOR
        private void Update()
        {
            GenerateIdEditor();
        }

        //generates a GUID on the editor, if the object is present at the scene
        private void GenerateIdEditor()
        {
            //check if the game is Playing or if no scene is loaded
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            //access the serialized field for the GUID
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }
        #endif

        //generates a GUID at runtime
        private void GenerateIDRuntime()
        {
            if (string.IsNullOrEmpty(uniqueIdentifier) || !IsUnique(uniqueIdentifier))
            {
                uniqueIdentifier = Guid.NewGuid().ToString();
            }

            globalLookup[uniqueIdentifier] = this;
        }

        //check lookup table to see if it is unique
        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;

            if (globalLookup[candidate] == this) return true;

            if (globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}