using System;
using UnityEngine;

[Serializable]
public struct SceneReference
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private string _path;
    [SerializeField]
    private int _buildIndex;

    /// <summary>
    /// The name of the scene asset itself.
    /// </summary>
    public string name { get { return _name; } private set { _name = value; } }

    /// <summary>
    /// The asset path to the scene.
    /// </summary>
    public string path { get { return _path; } private set { _path = value; } }

    /// <summary>
    /// The index of the scene in build settings
    /// </summary>
    public int buildIndex { get { return _buildIndex; } private set { _buildIndex = value; } }

    /// <summary>
    /// Returns back if the scene is in the build or not.
    /// </summary>
    public bool isInBuild { get { return buildIndex >= 0; } }
}
