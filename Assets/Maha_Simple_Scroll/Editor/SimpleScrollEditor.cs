using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleScroll))]

public class SimpleScrollEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SimpleScroll simpleScroll = (SimpleScroll)target;

        HandleTargetManager(simpleScroll, "_useNavigationButtons", "Remove SlideButtonManager", typeof(SliderButtonManager));
        HandleTargetManager(simpleScroll, "_useParallaxEffect", "Remove ParallaxLayer", typeof(ParallaxEffectManager));
    }

    /// <summary>
    /// Dynamically attaches or removes a component based on a serialized boolean property.
    /// </summary>
    private void HandleTargetManager(SimpleScroll simpleScroll, string parameterName, string removeParameterName, Type managerType)
    {
        SerializedProperty property = serializedObject.FindProperty(parameterName);
        if (property == null)
        {
            Debug.LogError($"Property '{parameterName}' not found in SimpleScroll.");
            return;
        }

        bool isEnabled = property.boolValue;

        if (isEnabled)
        {
            if (!simpleScroll.GetComponent(managerType))
            {
                simpleScroll.gameObject.AddComponent(managerType);
                Debug.Log($"{managerType.Name} attached in the Editor.");
            }
        }
        else
        {
            Component manager = simpleScroll.GetComponent(managerType);
            if (manager != null)
            {
                EditorApplication.delayCall += () =>
                {
                    if (manager != null)
                    {
                        Undo.RegisterCompleteObjectUndo(simpleScroll.gameObject, removeParameterName);
                        DestroyImmediate(manager);
                        Debug.Log($"{managerType.Name} removed from the GameObject.");
                    }
                };
            }
        }
    }
}
