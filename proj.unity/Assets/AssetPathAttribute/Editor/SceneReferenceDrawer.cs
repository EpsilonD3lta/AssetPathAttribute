using System;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : AssetPathDrawer
{
    private SerializedProperty _name;
    private SerializedProperty _buildIndex;

    protected override SerializedProperty GetProperty(SerializedProperty rootProperty)
    {
        _name = rootProperty.FindPropertyRelative("_name");
        _buildIndex = rootProperty.FindPropertyRelative("_buildIndex");
        return rootProperty.FindPropertyRelative("_path");
    }

    protected override Type ObjectType()
    {
        return typeof(SceneAsset);
    }

    protected override void OnSelectionMade(UnityEngine.Object newSelection, SerializedProperty property)
    {
        if (newSelection == null)
        {
            _name.stringValue = "";
            _buildIndex.intValue = -1;
        }
        else
        {
            string assetPath = AssetDatabase.GetAssetPath(newSelection);
            Scene scene = SceneManager.GetSceneByPath(assetPath);
            _name.stringValue = scene.name;
            _buildIndex.intValue = scene.buildIndex;
        }
        base.OnSelectionMade(newSelection, property);
    }
}
