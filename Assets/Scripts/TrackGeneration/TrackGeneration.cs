using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackGeneration : MonoBehaviour {

    public float mTrackWidth;
    public float mTrackHeight;
    public float mTrackSectionLength;

    public int mTrackPieceLength;

    public AnimationCurve mTrackCrossSection;

	// Use this for initialization
	void Start ()
    {
        MeshFilter mMeshFilter = GetComponent<MeshFilter>();
        Mesh mMesh = new Mesh();
        mMeshFilter.mesh = mMesh;

        Keyframe[] tempTrackPoints = mTrackCrossSection.keys;


        Vector3[] mVertices = new Vector3[tempTrackPoints.GetLength(0) * 2];
        int[] mTriangles = new int[((tempTrackPoints.GetLength(0) -1)*2)*3];
        Vector3[] mNormals = new Vector3[tempTrackPoints.GetLength(0) * 2];
        Vector2[] mUVs = new Vector2[tempTrackPoints.GetLength(0) * 2];

        for (int i = 0; i < tempTrackPoints.GetLength(0); i++)
        {
            //Vertices
            mVertices[i * 2] = new Vector3((tempTrackPoints[i].time * mTrackWidth) - (mTrackWidth/2), (tempTrackPoints[i].value * mTrackHeight), 0.0f);
            mVertices[(i * 2) + 1] = new Vector3((tempTrackPoints[i].time * mTrackWidth) - (mTrackWidth / 2), (tempTrackPoints[i].value * mTrackHeight), mTrackSectionLength);


            //Normals//TODO
            mNormals[i] = Vector3.up;
            //UVs
            mUVs[i*2] = new Vector2(0, tempTrackPoints[i].time);
            mUVs[(i * 2)+1] = new Vector2(1, tempTrackPoints[i].time);
        }

        for(int i = 0;i<tempTrackPoints.GetLength(0)-1;i++)
        {
            mTriangles[(i * 6) + 0] = (i * 2);
            mTriangles[(i * 6) + 1] = (i * 2) + 1;
            mTriangles[(i * 6) + 2] = (i * 2) + 2;

            mTriangles[(i * 6) + 3] = (i * 2) + 2;
            mTriangles[(i * 6) + 4] = (i * 2) + 1;
            mTriangles[(i * 6) + 5] = (i * 2) + 3;
        }


        mMesh.vertices = mVertices;
        mMesh.triangles = mTriangles;
        mMesh.normals = mNormals;
        mMesh.uv = mUVs;

        mMesh.RecalculateNormals();

        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
