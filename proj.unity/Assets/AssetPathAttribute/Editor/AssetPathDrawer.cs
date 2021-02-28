using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(AssetPath.Attribute))]
public class AssetPathDrawer : PropertyDrawer
{
    // A helper warning label when the user puts the attribute above a non string type.
    private const string invalidTypeLabel = "Attribute invalid for type ";

    // A reference to the object we have loaded
    private Object loadedAssetReference;

    /// <summary>
    /// Invoked when we want to try our property.
    /// </summary>
    /// <param name="position">The position we have allocated on screen</param>
    /// <param name="property">The field our attribute is over</param>
    /// <param name="label">The nice display label it has</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property = GetProperty(property);
        if (property.propertyType != SerializedPropertyType.String)
        {
            // Create a rect for our label
            Rect labelPosition = position;
            // Set it's width
            labelPosition.width = EditorGUIUtility.labelWidth;
            // Draw it
            GUI.Label(labelPosition, label);
            // Create a rect for our content
            Rect contentPosition = position;
            // Move it over by the x
            contentPosition.x += labelPosition.width;
            // Shrink it in width since we moved it over
            contentPosition.width -= labelPosition.width;
            // Draw our content warning;
            EditorGUI.HelpBox(contentPosition, invalidTypeLabel + this.fieldInfo.FieldType.Name, MessageType.Error);
        }
        else
        {
            HandleObjectReference(position, property, label);
        }
    }

    protected virtual SerializedProperty GetProperty(SerializedProperty rootProperty)
    {
        return rootProperty;
    }


    protected virtual Type ObjectType()
    {
        // Get our attribute
        AssetPath.Attribute attribute = this.attribute as AssetPath.Attribute;
        // Return back the type.
        return attribute.type;
    }

    private void HandleObjectReference(Rect position, SerializedProperty property, GUIContent label)
    {
        Type objectType = ObjectType();
        // First get our value
        Object propertyValue = null;
        // Save our path
        string assetPath = property.stringValue;
        label.tooltip = assetPath;
        CopyPathMenu(position, property, assetPath);
        // Have a label to say it's missing
        bool isMissing = false;
        // Check if we have a key
        if (loadedAssetReference != null)
        {
            // Get the value.
            propertyValue = loadedAssetReference;
        }

        // Now if its null we try to load it
        if (propertyValue == null && !string.IsNullOrEmpty(assetPath))
        {

            // Try to load our asset
            propertyValue = AssetDatabase.LoadAssetAtPath(assetPath, objectType);

            if (propertyValue == null)
            {
                isMissing = true;
            }
            else
            {
                loadedAssetReference = propertyValue;
            }
        }

        EditorGUI.BeginChangeCheck();
        {
            // Draw our object field.
            Color oldColor = GUI.color;
            GUI.color = isMissing ? new Color(1f, 0.8f, 0.8f, 1f) : new Color(0.9f, 1f, 0.9f, 1f);
            propertyValue = EditorGUI.ObjectField(position, label, propertyValue, objectType, false);
            GUI.color = oldColor;
        }
        if (EditorGUI.EndChangeCheck())
        {
            OnSelectionMade(propertyValue, property);
        }
    }

    protected virtual void OnSelectionMade(Object newSelection, SerializedProperty property)
    {
        string assetPath = string.Empty;

        if (newSelection != null)
        {
            // Get our path
            assetPath = AssetDatabase.GetAssetPath(newSelection);
        }

        // Save our value.
        loadedAssetReference = newSelection;
        // Save it back
        property.stringValue = assetPath;
    }

    private void CopyPathMenu(Rect position, SerializedProperty property, string assetPath)
    {
        Event e = Event.current;
        Rect contextRect = position;
        if (e.type == EventType.ContextClick && contextRect.Contains(e.mousePosition))
        {
            GenericMenu context = new GenericMenu();
            context.AddItem(new GUIContent("Copy Path"), property.isExpanded, CopyPath, assetPath);
            context.ShowAsContext();
            e.Use();
        }
        else if (e.type == EventType.MouseDown && e.button == 0 && position.Contains(e.mousePosition))
        {
            if (Event.current.modifiers == (EventModifiers.Alt | EventModifiers.Control)) // Command == Windows key
            {
                CopyPath(assetPath);
                e.Use();
            }
        }
    }

    private void CopyPath(object assetPath)
    {
        GUIUtility.systemCopyBuffer = (string)assetPath;
        UnityEngine.Debug.Log("assetPath copied to clipboard: " + assetPath);
    }
}
