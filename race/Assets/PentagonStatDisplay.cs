using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class PentagonStatDisplay : MonoBehaviour
{
    [SerializeField] MeshRenderer pentagonRenderer;
    [SerializeField] MeshFilter pentagonMesh;
    [SerializeField] Material pentagonMaterial;


    [Range(0,1)]public float Weight;
    [Range(0, 1)] public float Speed;
    [Range(0, 1)] public float Acceleration;
    [Range(0, 1)] public float Steering;
    [Range(0, 1)] public float Boost;

    [SerializeField] int numPoints = 5;
    [SerializeField] float radius = 1;

    float weightLerp = 0;
    float speedLerp = 0;
    float accelerationLerp = 0;
    float steeringLerp = 0;
    float boostLerp;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Weight = Random.Range(0.1f, 1f);
            Speed = Random.Range(0.1f, 1f);
            Acceleration = Random.Range(0.1f, 1f);
            Steering = Random.Range(0.1f, 1f);
            Boost = Random.Range(0.1f, 1f);
        }

        weightLerp = Mathf.Lerp(weightLerp, Weight, Time.deltaTime*5);
        speedLerp = Mathf.Lerp(speedLerp, Speed, Time.deltaTime * 5);
        accelerationLerp = Mathf.Lerp(accelerationLerp, Acceleration, Time.deltaTime * 5);
        steeringLerp = Mathf.Lerp(steeringLerp, Steering, Time.deltaTime * 5);
        boostLerp = Mathf.Lerp(boostLerp, Boost, Time.deltaTime * 5);

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[numPoints];
        int[] tris = new int[(numPoints - 2) * 3];
        Vector3[] normals = new Vector3[numPoints];
        Vector2[] uvs = new Vector2[numPoints];

        // Calculate angle increment
        float angleIncrement = 360f / numPoints;

        float[] stats = new float[5]
        {
            speedLerp, accelerationLerp, weightLerp, steeringLerp, boostLerp
        };

        // Generate vertices
        for (int i = 0; i < numPoints; i++)
        {
            float angle = i * angleIncrement * Mathf.Deg2Rad;
            vertices[i] = new Vector3(Mathf.Sin(angle) * stats[i]*radius, 0f, Mathf.Cos(angle) * stats[i] * radius);
            normals[i] = Vector3.up; // All normals point up (assuming the chart is on the XZ plane)
            uvs[i] = new Vector2((vertices[i].x + (stats[i] * radius)) / (2 * stats[i] * radius), (vertices[i].z + (stats[i] * radius)) / (2 * stats[i] * radius));
        }

        // Generate triangles
        for (int i = 0; i < numPoints - 2; i++)
        {
            tris[i * 3] = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = i + 2;
        }

        // Assign data to mesh
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uvs;

        pentagonMesh.mesh = mesh;
        pentagonRenderer.material = pentagonMaterial;
    }

    public void StatsUpdate()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3];
        Vector2[] uv = new Vector2[3];
        int[] triangles = new int[3];

        vertices[0] = Vector3.zero; //origin;
        vertices[1] = new Vector3(0, 100);
        vertices[2] = new Vector3(100, 100);

        triangles[0] = 0; //origin;
        triangles[0] = 1; 
        triangles[0] = 2;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        pentagonMesh.mesh = mesh;
        pentagonRenderer.material = pentagonMaterial;
        pentagonMesh.mesh.RecalculateNormals();
    }
}
