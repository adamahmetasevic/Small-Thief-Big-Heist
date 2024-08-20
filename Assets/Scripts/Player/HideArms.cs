using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class HideArms : MonoBehaviour
{
    public Material hideMaterial;

    private string[] armBoneNames = new string[] {
        "bip R UpperArm",
        "bip R Forearm",
        "bip R Hand",
        "bip R Finger0",
        "bip R Finger1",
        "bip R Finger11",
        "bip R Finger01",
        "bip L UpperArm",
        "bip L Forearm",
        "bip L Hand",
        "bip L Finger0",
        "bip L Finger1",
        "bip L Finger11",
        "bip L Finger01"
    };

    private SkinnedMeshRenderer[] renderers;
    private Dictionary<SkinnedMeshRenderer, Material[]> originalMaterials = new Dictionary<SkinnedMeshRenderer, Material[]>();

    private void Start()
    {
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        // Store original materials
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            originalMaterials[renderer] = renderer.materials;
        }
    }

    public void ApplyHideMaterial()
    {

        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            Transform[] bones = renderer.bones;
            int[] armBoneIndices = bones
                .Select((b, i) => new { Bone = b, Index = i })
                .Where(item => armBoneNames.Contains(item.Bone.name))
                .Select(item => item.Index)
                .ToArray();

            if (armBoneIndices.Length > 0)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = hideMaterial;
                }
                renderer.materials = materials;
            }
        }
    }

    public void RestoreOriginalMaterials()
    {


        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            if (originalMaterials.ContainsKey(renderer))
            {
                renderer.materials = originalMaterials[renderer];
            }
        }
    }
}
