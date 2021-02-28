﻿using System;
using UnityEngine;
public class AssetPath
{
    /// <summary>
    /// We limit this attributes to fields and only allow one. Should
    /// only be applied to string types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Attribute : PropertyAttribute
    {
        private PathType m_PathType;
        private Type m_Type;

        /// <summary>
        /// Gets the type of asset path this attribute is watching.
        /// </summary>
        public PathType pathType
        {
            get { return m_PathType; }
        }

        /// <summary>
        /// Gets the type of asset this attribute is expecting.
        /// </summary>
        public Type type
        {
            get { return m_Type; }
        }

        /// <summary>
        /// Creates the default instance of AssetPathAttribute
        /// </summary>
        public Attribute(Type type)
        {
            m_Type = type;
            m_PathType = PathType.Project;
        }
    }

    /// <summary>
    /// A enum containing all the types of paths we can watch
    /// </summary>
    public enum PathType
    {
        /// <summary>
        /// The path will be contained within the 'Asset/*' directory.
        /// </summary>
        Project,
        /// <summary>
        /// The path will be contained within a resources folder.
        /// </summary>
        Resources,
    }

    private const string RESOURCES_FOLDER_NAME = "/Resources/";

    /// <summary>
    /// Takes the string from the Asset Path Attribute and converts it into
    /// a usable resources path.
    /// </summary>
    /// <param name="assetPath">The project path that AssetPathAttribute serializes</param>
    /// <returns>The resources path if it exists otherwise returns the same path</returns>
    public static string ConvertToResourcesPath(string projectPath)
    {
        // Make sure it's not empty
        if (string.IsNullOrEmpty(projectPath))
        {
            return string.Empty;
        }

        // Get the index of the resources folder
        int folderIndex = projectPath.IndexOf(RESOURCES_FOLDER_NAME);

        // If it's -1 we this asset is not in a resource folder
        if (folderIndex == -1)
        {
            return string.Empty;
        }

        // We don't include the 'Resources' part in our final path
        folderIndex += RESOURCES_FOLDER_NAME.Length;

        // Calculate the full length of our substring
        int length = projectPath.Length - folderIndex;

        // Get extension length
        length -= projectPath.Length - projectPath.LastIndexOf('.');

        // Get the substring
        string resourcesPath = projectPath.Substring(folderIndex, length);

        // Return it.
        return resourcesPath;
    }

    /// <summary>
    /// Loads the asset at the following path. If the asset is contained within a resources folder
    /// this uses <see cref="UnityEngine.Resources.Load(string)"/>. If we are in the Editor this will
    /// use <see cref="UnityEditor.AssetDatabase.LoadAssetAtPath(string, Type)"/> instead. This will
    /// allow you to load any type at any path. Keep in mind at Runtime the asset can only be loaded
    /// if it is inside a resources folder.
    /// </summary>
    /// <typeparam name="T">The type of object you want to load</typeparam>
    /// <param name="projectPath">The full project path of the object you are trying to load.</param>
    /// <returns>The loaded asset or null if it could not be found.</returns>
    public static T Load<T>(string projectPath) where T : UnityEngine.Object
    {
        // Make sure our path is not null
        if (string.IsNullOrEmpty(projectPath))
        {
            return null;
        }

        // Get our resources path
        string resourcesPath = ConvertToResourcesPath(projectPath);

        if (!string.IsNullOrEmpty(resourcesPath))
        {
            // The asset is in a resources folder.
            return Resources.Load<T>(resourcesPath);
        }

#if UNITY_EDITOR
        // We could not find it in resources so we just try the AssetDatabase.
        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(projectPath);
#else
        return null;
#endif
    }
}
