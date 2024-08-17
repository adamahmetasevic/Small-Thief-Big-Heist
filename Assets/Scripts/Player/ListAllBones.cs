using UnityEngine;
using System.Collections.Generic;

public class ListAllBones : MonoBehaviour
{
    private void Start()
    {

        /*
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            List<string> boneNames = new List<string>();
            foreach (Transform bone in renderer.bones)
            {
                if (bone != null)
                {
                    boneNames.Add(bone.name);
                }
            }
            
            Debug.Log($"Bone names for SkinnedMeshRenderer on {renderer.gameObject.name}:");
            foreach (string name in boneNames)
            {
                Debug.Log(name);
            }

            
        }

        */
    }
}