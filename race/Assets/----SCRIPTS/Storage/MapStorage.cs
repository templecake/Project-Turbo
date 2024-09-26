using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStorage : MonoBehaviour
{
    public string MapName;
    public Sprite MapDisplayImage;

    public GameObject Checkpoints;
    public GameObject StartPositions;
    public GameObject Powerups;

    public List<AIRoute> AIRoutes = new List<AIRoute>();
    public AIRoute generalRoute;

    public List<GameObject> CameraPanPositions = new List<GameObject>();

    public Material SkyboxMaterial;
    public Color FogColour;
    public float FogDensity;
    public Color GlobalLightColour;
    public float GlobalLightIntensity;

    public List<GameObject> GetCheckpoints()
    {
        List<GameObject> checks = new List<GameObject>();

        for (int i = 0; i < Checkpoints.transform.childCount; i++)
        {
            checks.Add(Checkpoints.transform.GetChild(i).gameObject);
        }

        return checks;  
    }

    public List<GameObject> GetStartPositions()
    {
        List<GameObject> starts = new List<GameObject>();

        for (int i = 0; i < StartPositions.transform.childCount; i++)
        {
            starts.Add(StartPositions.transform.GetChild(i).gameObject);
        }

        return starts;
    }

    public List<AIRoute> GetAIRoute()
    {
        return AIRoutes;
    }

    public AIRoute GetGeneralRoute()
    {
        return generalRoute;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        for (int i = 0; i < AIRoutes.Count; i++)
        {
            if (AIRoutes[i].Displaying)
            {
                for (int x = 0; x < AIRoutes[i].positions.Count; x++)
                {
                    Gizmos.color = AIRoutes[i].RouteColour * 0.9f;
                    if (x > 0) { Gizmos.DrawLine(AIRoutes[i].positions[x - 1], AIRoutes[i].positions[x]); }

                    Gizmos.color = AIRoutes[i].RouteColour;
                    Gizmos.DrawSphere(AIRoutes[i].positions[x], 1f);
                    if (AIRoutes[i].ShowOverlay)
                    {
                        Color c = Color.green;
                        c.a = 0.2f;
                        Gizmos.color = c;
                        Gizmos.DrawSphere(AIRoutes[i].positions[x], AIRoutes[i].CompleteRadius[x]);
                    }
                }
            }
        }

        if (generalRoute.Displaying)
        {
            for (int x = 0; x < generalRoute.positions.Count; x++)
            {
                Gizmos.color = generalRoute.RouteColour * 0.9f;
                if (x > 0) { Gizmos.DrawLine(generalRoute.positions[x - 1], generalRoute.positions[x]); }

                Gizmos.color = generalRoute.RouteColour;
                Gizmos.DrawSphere(generalRoute.positions[x], 1f);
                if (generalRoute.ShowOverlay)
                {
                    Color c = Color.green;
                    c.a = 0.2f;
                    Gizmos.color = c;
                    Gizmos.DrawSphere(generalRoute.positions[x], 20f);
                }
            }
        }
    }
    #endregion

}
