using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thesis
{
    public class CachedSpatialMapping : SpatialMappingSource
    {
        public void Load(IList<Mesh> storedMeshes)
        {
            Cleanup();
            for (int iMesh = 0; iMesh < storedMeshes.Count; iMesh++)
            {
                AddSurfaceObject(CreateSurfaceObject
                (
                    mesh: storedMeshes[iMesh],
                    objectName: "storedmesh-" + iMesh,
                    parentObject: SpatialMappingManager.Instance.gameObject.transform,
                    meshID: iMesh
                ));
            }
        }
    }
}