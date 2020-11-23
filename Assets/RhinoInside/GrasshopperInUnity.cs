using System.Collections.Generic;
using UnityEngine;
using Rhino;
using RhinoInside.Unity;

//[ExecuteInEditMode]
public class GrasshopperInUnity : MonoBehaviour
{

    public GameObject geoPrefab;
    private List<GameObject> _gameObjects = new List<GameObject>();

    void FromGrasshopper(object sender, Rhino.Runtime.NamedParametersEventArgs args)
    {
        Rhino.Geometry.GeometryBase[] values;
        if (args.TryGetGeometry("mesh", out values))
        {
            var meshFilters = GetComponentsInChildren<MeshFilter>();

            foreach (var meshFilter in meshFilters)
            {
            if (meshFilter.sharedMesh != null)
            {
                DestroyImmediate(meshFilter.sharedMesh);
            }
            }

            if (values.Length != _gameObjects.Count)
            {
            foreach(var gb in _gameObjects)
            {
                DestroyImmediate(gb);
            }

            _gameObjects.Clear();

            for(int i=0; i<values.Length; i++)
            {
                GameObject instance = (GameObject) Instantiate(geoPrefab);
                instance.transform.SetParent(transform);
                _gameObjects.Add(instance);
            }
            }

            for (int i = 0; i < values.Length; i++)
            {
            _gameObjects[i].GetComponent<MeshFilter>().mesh = (values[i] as Rhino.Geometry.Mesh).ToHost();
            }
        }
    }

    public void ToGrasshopper(Rhino.Runtime.NamedParametersEventArgs args)
    {
        Rhino.Runtime.HostUtils.ExecuteNamedCallback("ToGrasshopper", args);
    }

  // Start is called before the first frame update
  void Start()
  {
        //OpenGH();
        if (!Startup.isLoaded)
        {
            Startup.Init();
        }

        Rhino.Runtime.HostUtils.RegisterNamedCallback("Unity:FromGrasshopper", FromGrasshopper);
    }

  // Update is called once per frame
  void Update()
  {

  }

    public void OpenGH()
    {
        string script = "!_-Grasshopper _W _S ENTER";
        Rhino.RhinoApp.RunScript(script, false);
    }
    
    public void SendRndSeedToGH()
    {
        var val = Random.Range(0, 100000f);
        using (var args = new Rhino.Runtime.NamedParametersEventArgs())
        {
            args.Set("randomSeed", val);
            ToGrasshopper(args);

        }
    }

}

