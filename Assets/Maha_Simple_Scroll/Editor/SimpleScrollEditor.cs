using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(SimpleScroll))]

public class SimpleScrollEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SimpleScroll simpleScroll = (SimpleScroll)target;

        HandleTargetManager(simpleScroll, "_useNavigationButtons", "Remove SlideButtonManager", typeof(SliderButtonManager));
        HandleParallaxEffect(simpleScroll);
        HandleTargetManager(simpleScroll, "_useToggleNavigation", "Remove ToggleGroupManager", typeof(ToggleGroupManager));
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

    /// <summary>
    /// Ensures ParallaxEffectManager is applied to each child of the content container, not the ScrollView itself.
    /// </summary>
    private void HandleParallaxEffect(SimpleScroll simpleScroll)
    {
        SerializedProperty property = serializedObject.FindProperty("_useParallaxEffect");
        if (property == null)
        {
            Debug.LogError("Property '_useParallaxEffect' not found in SimpleScroll.");
            return;
        }

        bool isEnabled = property.boolValue;
        ScrollRect scrollRect = simpleScroll.GetComponent<ScrollRect>();

        if (scrollRect == null || scrollRect.content == null)
        {
            Debug.LogError("ScrollRect or its content container is missing.");
            return;
        }

        Transform contentTransform = scrollRect.content;

        foreach (Transform child in contentTransform)
        {
            if (isEnabled)
            {
                if (!child.gameObject.GetComponent<ParallaxEffectManager>())
                {
                    Undo.RegisterCompleteObjectUndo(child.gameObject, "Add ParallaxEffectManager");
                    child.gameObject.AddComponent<ParallaxEffectManager>();
                    Debug.Log($"ParallaxEffectManager added to {child.name}.");
                }
            }
            else
            {
                ParallaxEffectManager parallaxEffect = child.gameObject.GetComponent<ParallaxEffectManager>();
                if (parallaxEffect != null)
                {
                    Undo.RegisterCompleteObjectUndo(child.gameObject, "Remove ParallaxEffectManager");
                    DestroyImmediate(parallaxEffect);
                    Debug.Log($"ParallaxEffectManager removed from {child.name}.");
                }
            }
        }
    }
}
