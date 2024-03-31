using System.Linq;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class TerrainManager : MonoBehaviour
{

#if UNITY_EDITOR

    [ContextMenu("Add Mesh Colliders")]
    public void AddMeshColliders()
    {
        var children = transform.GetComponentsInChildren<MeshFilter>();

        Undo.RecordObjects(children.ToArray(), "Add MeshCollider");

        foreach (var child in children)
        {
            if (child.TryGetComponent<MeshCollider>(out _) == false)
            {
                child.AddComponent<MeshCollider>();

            }
        };
    }

    [MenuItem("Custom Editors/Add random object")]
    public static void RandomizeObject()
    {
        if (Selection.activeTransform == null)
        {
            return;
        }

        var models = AssetDatabase.FindAssets("t:model");

        var assetPath = AssetDatabase.GUIDToAssetPath(models[Random.Range(0, models.Length)]);

        var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

        var newObject = Instantiate(asset, Selection.activeTransform.position, Quaternion.identity, Selection.activeTransform.parent);

        newObject.AddComponent<MeshCollider>();

        Undo.DestroyObjectImmediate(Selection.activeTransform.gameObject);

        Undo.RegisterCreatedObjectUndo(newObject, "Randomize");

        Selection.activeObject = newObject;
    }
#endif

}
