using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{

    private List<GameObject> builds;

    public void Initialize()
    {
        builds = new List<GameObject>();
    }

    public GameObject CreateBuild(float x, float y, BuildEntity.Type type)
    {
        GameObject build = Game.World.CreateEntity(x, y, Entity.Type.BUILD, (int)type);
        builds.Add(build);
        return build;
    }

    public List<GameObject> GetNeighbors(float x, float y)
    {
        List<GameObject> neighbors = new List<GameObject>();
        for (int i = 0; i < builds.Count; i++)
        {
            GameObject build = builds[i];
            if ((build.transform.position - new Vector3(x, y)).magnitude <= 2.0f)
            {
                neighbors.Add(build);
            }
        }
        return neighbors;
    }

}
