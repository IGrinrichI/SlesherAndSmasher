using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slasher : MonoBehaviour
{
    private MyPlane plane;
    public GameObject emptyMesh;
    public GameObject testObject;
    public Vector3 n = Vector3.up;
    public Vector3 m = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Detect when there is a mouse click
        if (Input.GetMouseButtonUp(0))
        {
            plane = new MyPlane(n, m);
            //Debug.Log(plane.GetDot(Vector3.one, new Vector3(1,4,7)));
            Split(plane, testObject);
        }
    }

    public void Split(MyPlane realPlane, GameObject go)
    {
        plane = new MyPlane(go.transform.InverseTransformDirection(realPlane.n), go.transform.InverseTransformPoint(realPlane.m));

        Vector3[] originVertices = go.GetComponent<MeshFilter>().mesh.vertices;
        int[] originTriangles = go.GetComponent<MeshFilter>().mesh.triangles;
        Dictionary<int, int> oldNewRtriangles = new Dictionary<int, int>();
        Dictionary<int, int> oldNewLtriangles = new Dictionary<int, int>();
        Dictionary<int, Vector3> rightVertices = new Dictionary<int, Vector3>();
        Dictionary<int, Vector3> leftVertices = new Dictionary<int, Vector3>();
        List<int> rightTriangles = new List<int>();
        List<int> leftTriangles = new List<int>();
        List<Vector3> newLeftEdgeVertices = new List<Vector3>();
        List<Vector3> newRightEdgeVertices = new List<Vector3>();
        int counter = 0;
        int rightCounter = 0;
        int leftCounter = 0;
        

        foreach (Vector3 vertice in originVertices)
        {
            if (plane.GetSide(vertice))
            {
                rightVertices.Add(counter, vertice);
                oldNewRtriangles.Add(counter, rightCounter);
                rightCounter++;
            }
            else
            {
                leftVertices.Add(counter, vertice);
                oldNewLtriangles.Add(counter, leftCounter);
                leftCounter++;
            }
            counter++;
        }

        //Debug.Log("leftvert = " + leftCounter + "    rightvert = " + rightCounter);
        
        for (int i = 0; i < originTriangles.Length; i += 3)
        {
            //Debug.Log("Select " + i + " and " + (i + 1) + " and " + (i + 2) + " triangles");
            bool[] right = new bool[] { rightVertices.ContainsKey(originTriangles[i]), rightVertices.ContainsKey(originTriangles[i + 1]), rightVertices.ContainsKey(originTriangles[i + 2]) };

            // Issue with ContainsKey method (probably collision), idk why but here we go again...
            for (int j = 0; j < 3; j++)
            {
                if (right[j])
                {
                    right[j] = rightVertices[originTriangles[i + j]] == originVertices[originTriangles[i + j]];
                }
            }
            /*
            if (rightVertices.ContainsKey(originTriangles[16]))
            {
                Debug.Log(rightVertices.ContainsKey(originTriangles[16]) + "   THIS VALUE " + rightVertices[originTriangles[16]]);
            }
            else
            {
                Debug.Log(rightVertices.ContainsKey(originTriangles[16]));
            }
            
            Debug.Log(originVertices[originTriangles[i]] + " " + originVertices[originTriangles[i + 1]] + " " + originVertices[originTriangles[i + 2]] + "       " + right[0] + " " + right[1] + " " + right[2]);
            */

            if (right[0] == right[1]) // + + ?
            {
                if (right[0] == right[2]) // + + +
                {
                    if (right[0] == true) // r r r
                    {
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i]]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 1]]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 2]]);
                    }
                    else // l l l
                    {
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i]]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 1]]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 2]]);
                    }
                }
                else // + + -
                {
                    Vector3 rt12 = (originVertices[originTriangles[i]] + originVertices[originTriangles[i + 1]]) / 2;
                    Vector3 rt23 = plane.GetDotBetween(originVertices[originTriangles[i + 1]], originVertices[originTriangles[i + 2]]);
                    Vector3 rt13 = plane.GetDotBetween(originVertices[originTriangles[i]], originVertices[originTriangles[i + 2]]);
                    
                    if (right[0] == true) // r r l
                    {
                        rightVertices.Add(counter, rt12); // new 1/2
                        oldNewRtriangles.Add(counter, rightCounter);
                        counter++;
                        rightCounter++;
                        rightVertices.Add(counter, rt13); // new 3 for 1
                        oldNewRtriangles.Add(counter, rightCounter);
                        newRightEdgeVertices.Add(/*rightCounter,*/ rt13);
                        counter++;
                        rightCounter++;
                        rightVertices.Add(counter, rt23); // new 3 for 2
                        oldNewRtriangles.Add(counter, rightCounter);
                        newRightEdgeVertices.Add(/*rightCounter,*/ rt23);
                        counter++;
                        rightCounter++;

                        rightTriangles.Add(oldNewRtriangles[originTriangles[i]]);
                        rightTriangles.Add(oldNewRtriangles[counter - 3]);
                        rightTriangles.Add(oldNewRtriangles[counter - 2]);

                        rightTriangles.Add(oldNewRtriangles[counter - 3]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 1]]);
                        rightTriangles.Add(oldNewRtriangles[counter - 1]);

                        rightTriangles.Add(oldNewRtriangles[counter - 3]);
                        rightTriangles.Add(oldNewRtriangles[counter - 1]);
                        rightTriangles.Add(oldNewRtriangles[counter - 2]);


                        if (!leftVertices.ContainsKey(originTriangles[i]))
                        {
                            leftVertices.Add(originTriangles[i], rt13);
                            oldNewLtriangles.Add(originTriangles[i], leftCounter);
                            newLeftEdgeVertices.Add(/*leftCounter,*/ rt13);
                            leftCounter++;
                        }
                        if (!leftVertices.ContainsKey(originTriangles[i + 1]))
                        {
                            leftVertices.Add(originTriangles[i + 1], rt23);
                            oldNewLtriangles.Add(originTriangles[i + 1], leftCounter);
                            newLeftEdgeVertices.Add(/*leftCounter,*/ rt23);
                            leftCounter++;
                        }

                        leftTriangles.Add(oldNewLtriangles[originTriangles[i]]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 1]]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 2]]);
                    }
                    else // l l r
                    {
                        leftVertices.Add(counter, rt12); // new 1/2
                        oldNewLtriangles.Add(counter, leftCounter);
                        counter++;
                        leftCounter++;
                        leftVertices.Add(counter, rt13); // new 3 for 1
                        oldNewLtriangles.Add(counter, leftCounter);
                        newLeftEdgeVertices.Add(/*leftCounter,*/ rt13);
                        counter++;
                        leftCounter++;
                        leftVertices.Add(counter, rt23); // new 3 for 2
                        oldNewLtriangles.Add(counter, leftCounter);
                        newLeftEdgeVertices.Add(/*leftCounter,*/ rt23);
                        counter++;
                        leftCounter++;

                        leftTriangles.Add(oldNewLtriangles[originTriangles[i]]);
                        leftTriangles.Add(oldNewLtriangles[counter - 3]);
                        leftTriangles.Add(oldNewLtriangles[counter - 2]);

                        leftTriangles.Add(oldNewLtriangles[counter - 3]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 1]]);
                        leftTriangles.Add(oldNewLtriangles[counter - 1]);

                        leftTriangles.Add(oldNewLtriangles[counter - 3]);
                        leftTriangles.Add(oldNewLtriangles[counter - 1]);
                        leftTriangles.Add(oldNewLtriangles[counter - 2]);


                        if (!rightVertices.ContainsKey(originTriangles[i]))
                        {
                            rightVertices.Add(originTriangles[i], rt13);
                            oldNewRtriangles.Add(originTriangles[i], rightCounter);
                            newRightEdgeVertices.Add(/*rightCounter,*/ rt13);
                            rightCounter++;
                        }
                        if (!rightVertices.ContainsKey(originTriangles[i + 1]))
                        {
                            rightVertices.Add(originTriangles[i + 1], rt23);
                            oldNewRtriangles.Add(originTriangles[i + 1], rightCounter);
                            newRightEdgeVertices.Add(/*rightCounter,*/ rt23);
                            rightCounter++;
                        }

                        rightTriangles.Add(oldNewRtriangles[originTriangles[i]]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 1]]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 2]]);
                    }
                }
                    
            }
            else // + - ?
            {
                if (right[0] == right[2]) // + - +
                {
                    Vector3 rt12 = plane.GetDotBetween(originVertices[originTriangles[i]], originVertices[originTriangles[i + 1]]);
                    Vector3 rt23 = plane.GetDotBetween(originVertices[originTriangles[i + 1]], originVertices[originTriangles[i + 2]]);
                    Vector3 rt13 = (originVertices[originTriangles[i]] + originVertices[originTriangles[i + 2]]) / 2;

                    if (right[0] == true) // r l r
                    {
                        rightVertices.Add(counter, rt13); // new 1/3
                        oldNewRtriangles.Add(counter, rightCounter);
                        counter++;
                        rightCounter++;
                        rightVertices.Add(counter, rt12); // new 2 for 1
                        oldNewRtriangles.Add(counter, rightCounter);
                        newRightEdgeVertices.Add(/*rightCounter,*/ rt12);
                        counter++;
                        rightCounter++;
                        rightVertices.Add(counter, rt23); // new 2 for 3
                        oldNewRtriangles.Add(counter, rightCounter);
                        newRightEdgeVertices.Add(/*rightCounter,*/ rt23);
                        counter++;
                        rightCounter++;

                        rightTriangles.Add(oldNewRtriangles[originTriangles[i]]);
                        rightTriangles.Add(oldNewRtriangles[counter - 2]);
                        rightTriangles.Add(oldNewRtriangles[counter - 3]);

                        rightTriangles.Add(oldNewRtriangles[counter - 3]);
                        rightTriangles.Add(oldNewRtriangles[counter - 1]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 2]]);

                        rightTriangles.Add(oldNewRtriangles[counter - 3]);
                        rightTriangles.Add(oldNewRtriangles[counter - 2]);
                        rightTriangles.Add(oldNewRtriangles[counter - 1]);


                        if (!leftVertices.ContainsKey(originTriangles[i]))
                        {
                            leftVertices.Add(originTriangles[i], rt12);
                            oldNewLtriangles.Add(originTriangles[i], leftCounter);
                            newLeftEdgeVertices.Add(/*leftCounter,*/ rt12);
                            leftCounter++;
                        }
                        if (!leftVertices.ContainsKey(originTriangles[i + 2]))
                        {
                            leftVertices.Add(originTriangles[i + 2], rt23);
                            oldNewLtriangles.Add(originTriangles[i + 2], leftCounter);
                            newLeftEdgeVertices.Add(/*leftCounter,*/ rt23);
                            leftCounter++;
                        }

                        leftTriangles.Add(oldNewLtriangles[originTriangles[i]]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 1]]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 2]]);
                    }
                    else // l r l
                    {
                        leftVertices.Add(counter, rt13); // new 1/3
                        oldNewLtriangles.Add(counter, leftCounter);
                        counter++;
                        leftCounter++;
                        leftVertices.Add(counter, rt12); // new 2 for 1
                        oldNewLtriangles.Add(counter, leftCounter);
                        newLeftEdgeVertices.Add(/*leftCounter,*/ rt12);
                        counter++;
                        leftCounter++;
                        leftVertices.Add(counter, rt23); // new 2 for 3
                        oldNewLtriangles.Add(counter, leftCounter);
                        newLeftEdgeVertices.Add(/*leftCounter,*/ rt23);
                        counter++;
                        leftCounter++;

                        leftTriangles.Add(oldNewLtriangles[originTriangles[i]]);
                        leftTriangles.Add(oldNewLtriangles[counter - 2]);
                        leftTriangles.Add(oldNewLtriangles[counter - 3]);

                        leftTriangles.Add(oldNewLtriangles[counter - 3]);
                        leftTriangles.Add(oldNewLtriangles[counter - 1]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 2]]);

                        leftTriangles.Add(oldNewLtriangles[counter - 3]);
                        leftTriangles.Add(oldNewLtriangles[counter - 2]);
                        leftTriangles.Add(oldNewLtriangles[counter - 1]);


                        if (!rightVertices.ContainsKey(originTriangles[i]))
                        {
                            rightVertices.Add(originTriangles[i], rt12);
                            oldNewRtriangles.Add(originTriangles[i], rightCounter);
                            newRightEdgeVertices.Add(/*rightCounter,*/ rt12);
                            rightCounter++;
                        }
                        if (!rightVertices.ContainsKey(originTriangles[i + 2]))
                        {
                            rightVertices.Add(originTriangles[i + 2], rt23);
                            oldNewRtriangles.Add(originTriangles[i + 2], rightCounter);
                            newRightEdgeVertices.Add(/*rightCounter,*/ rt23);
                            rightCounter++;
                        }

                        rightTriangles.Add(oldNewRtriangles[originTriangles[i]]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 1]]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 2]]);
                    }
                }
                else // + - -
                {
                    Vector3 rt12 = plane.GetDotBetween(originVertices[originTriangles[i]], originVertices[originTriangles[i + 1]]); 
                    Vector3 rt23 = (originVertices[originTriangles[i + 1]] + originVertices[originTriangles[i + 2]]) / 2;
                    Vector3 rt13 = plane.GetDotBetween(originVertices[originTriangles[i]], originVertices[originTriangles[i + 2]]);

                    if (right[0] == true) // r l l
                    {
                        leftVertices.Add(counter, rt23); // new 2/3
                        oldNewLtriangles.Add(counter, leftCounter);
                        counter++;
                        leftCounter++;
                        leftVertices.Add(counter, rt12); // new 1 for 2
                        oldNewLtriangles.Add(counter, leftCounter);
                        newLeftEdgeVertices.Add(/*leftCounter,*/ rt12);
                        counter++;
                        leftCounter++;
                        leftVertices.Add(counter, rt13); // new 1 for 3
                        oldNewLtriangles.Add(counter, leftCounter);
                        newLeftEdgeVertices.Add(/*leftCounter,*/ rt13);
                        counter++;
                        leftCounter++;

                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 1]]);
                        leftTriangles.Add(oldNewLtriangles[counter - 3]);
                        leftTriangles.Add(oldNewLtriangles[counter - 2]);

                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 2]]); 
                        leftTriangles.Add(oldNewLtriangles[counter - 1]);
                        leftTriangles.Add(oldNewLtriangles[counter - 3]);

                        leftTriangles.Add(oldNewLtriangles[counter - 2]);
                        leftTriangles.Add(oldNewLtriangles[counter - 3]);
                        leftTriangles.Add(oldNewLtriangles[counter - 1]);

                        if (!rightVertices.ContainsKey(originTriangles[i + 1]))
                        {
                            rightVertices.Add(originTriangles[i + 1], rt12);
                            oldNewRtriangles.Add(originTriangles[i + 1], rightCounter);
                            newRightEdgeVertices.Add(/*rightCounter,*/ rt12);
                            rightCounter++;
                        }
                        if (!rightVertices.ContainsKey(originTriangles[i + 2]))
                        {
                            rightVertices.Add(originTriangles[i + 2], rt13);
                            oldNewRtriangles.Add(originTriangles[i + 2], rightCounter);
                            newRightEdgeVertices.Add(/*rightCounter,*/ rt13);
                            rightCounter++;
                        }

                        rightTriangles.Add(oldNewRtriangles[originTriangles[i]]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 1]]);
                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 2]]);
                        //Debug.Log("WHAT THE " + i);
                        //Debug.Log("Okey, lets look at origin triangles " + originTriangles[i] + " " + originTriangles[i + 1] + " " + originTriangles[i + 2]);
                        //Debug.Log("EXPLAIN ME THIS " + rt12 + " " + rt13 + " " + rightVertices[originTriangles[i]]);
                    }
                    else // l r r
                    {
                        rightVertices.Add(counter, rt23); // new 2/3
                        oldNewRtriangles.Add(counter, rightCounter);
                        counter++;
                        rightCounter++;
                        rightVertices.Add(counter, rt12); // new 1 for 2
                        oldNewRtriangles.Add(counter, rightCounter);
                        newRightEdgeVertices.Add(/*rightCounter,*/ rt12);
                        counter++;
                        rightCounter++;
                        rightVertices.Add(counter, rt13); // new 1 for 3
                        oldNewRtriangles.Add(counter, rightCounter);
                        newRightEdgeVertices.Add(/*rightCounter,*/ rt13);
                        counter++;
                        rightCounter++;

                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 1]]);
                        rightTriangles.Add(oldNewRtriangles[counter - 3]);
                        rightTriangles.Add(oldNewRtriangles[counter - 2]);

                        rightTriangles.Add(oldNewRtriangles[originTriangles[i + 2]]);
                        rightTriangles.Add(oldNewRtriangles[counter - 1]);
                        rightTriangles.Add(oldNewRtriangles[counter - 3]);

                        rightTriangles.Add(oldNewRtriangles[counter - 2]);
                        rightTriangles.Add(oldNewRtriangles[counter - 3]);
                        rightTriangles.Add(oldNewRtriangles[counter - 1]);

                        if (!leftVertices.ContainsKey(originTriangles[i + 1]))
                        {
                            leftVertices.Add(originTriangles[i + 1], rt12);
                            oldNewLtriangles.Add(originTriangles[i + 1], leftCounter);
                            newLeftEdgeVertices.Add(/*leftCounter,*/ rt12);
                            leftCounter++;
                        }
                        if (!leftVertices.ContainsKey(originTriangles[i + 2]))
                        {
                            leftVertices.Add(originTriangles[i + 2], rt13);
                            oldNewLtriangles.Add(originTriangles[i + 2], leftCounter);
                            newLeftEdgeVertices.Add(/*leftCounter,*/ rt13);
                            leftCounter++;
                        }

                        leftTriangles.Add(oldNewLtriangles[originTriangles[i]]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 1]]);
                        leftTriangles.Add(oldNewLtriangles[originTriangles[i + 2]]);
                    }
                }
            }
            //Debug.Log("leftvert = " + leftCounter + "    rightvert = " + rightCounter);
        }

        //Cover slice space
        //How to properly order vertices or make triangles?
        //(For case of convex figures)
        List<Vector3> sortedNewRightEdgeVertices = SortVertices(newRightEdgeVertices);
        List<Vector3> sortedNewLeftEdgeVertices = SortVertices(newLeftEdgeVertices);
        List<Vector3> resRightVertices = new List<Vector3>(rightVertices.Values);
        resRightVertices.AddRange(sortedNewRightEdgeVertices);
        List<Vector3> resLeftVertices = new List<Vector3>(leftVertices.Values);
        resLeftVertices.AddRange(sortedNewLeftEdgeVertices);

        Vector3 rightTriangleNormal = Vector3.Cross(resRightVertices[rightVertices.Count + 1] - resRightVertices[rightVertices.Count],
            resRightVertices[rightVertices.Count + sortedNewRightEdgeVertices.Count - 1] - resRightVertices[rightVertices.Count]).normalized;
        bool rightDir = Vector3.Angle(n, rightTriangleNormal) < 90 ? false : true;

        rightTriangles.AddRange(CalculateTriangles(rightVertices.Count, sortedNewRightEdgeVertices.Count, rightDir));
        leftTriangles.AddRange(CalculateTriangles(leftVertices.Count, sortedNewLeftEdgeVertices.Count, !rightDir));

        GameObject rightObject = Instantiate(emptyMesh, go.transform);
        GameObject leftObject = Instantiate(emptyMesh, go.transform);

        //Debug.Log(testObject.GetComponent<MeshFilter>().mesh.triangles.Length);

        SetMesh(rightObject, /*new List<Vector3>(rightVertices.Values)*/resRightVertices.ToArray(), rightTriangles.ToArray());
        //Debug.Log("right obj v = " + rightVertices.Values.Count + "    t = " + rightTriangles.Count);
        SetMesh(leftObject, /*new List<Vector3>(leftVertices.Values)*/resLeftVertices.ToArray(), leftTriangles.ToArray());
        //Debug.Log("left obj v = " + leftVertices.Values.Count + "    t = " + leftTriangles.Count);
    }

    private List<int> SortVertices(Dictionary<int, Vector3> vertices)
    {
        List<int> result = new List<int>();
        List<int> keys = new List<int>(vertices.Keys);

        result.Add(keys[0]);
        int minkey;
        float mindis;
        float tempdis;

        for (int i = 1; i < keys.Count; i++)
        {
            minkey = keys[i];
            mindis = Vector3.Distance(vertices[keys[i - 1]], vertices[keys[i]]);
            for (int j = i + 1; j < keys.Count; j++)
            {
                tempdis = Vector3.Distance(vertices[keys[i - 1]], vertices[keys[j]]);
                if (mindis > tempdis)
                {
                    minkey = keys[j];
                    mindis = tempdis;
                }
            }
            result.Add(minkey);
        }

        return result;
    }

    private List<Vector3> SortVertices(List<Vector3> badVertices)
    {
        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i < badVertices.Count; i++)
        {
            if (!vertices.Contains(badVertices[i]))
            {
                vertices.Add(badVertices[i]);
            }
        }
        List<Vector3> result = new List<Vector3>();
        result.Add(vertices[0]);
        int minind;
        float mindis;
        float tempdis;

        for (int i = 1; i < vertices.Count; i++)
        {
            mindis = int.MaxValue;
            minind = i;
            for (int j = 1; j < vertices.Count; j++)
            {
                tempdis = Vector3.Distance(result[i - 1], vertices[j]);
                if (mindis > tempdis && !result.Contains(vertices[j]))
                {
                    mindis = tempdis;
                    minind = j;
                }
            }
            result.Add(vertices[minind]);
        }

        return result;
    }

    private List<int> CalculateTriangles(List<int> indexes)
    {
        List<int> result = new List<int>();

        int first, second, third;

        first = 0;
        second = indexes.Count - 1;
        third = indexes.Count - 1;

        for (int i = 0; i < indexes.Count - 2; i++)
        {
            if (i % 2 == 0)
            {
                //first = first;
                third = second;
                second = first + 1;
            }
            else
            {
                first = second;
                second = third - 1;
                //third = third;
            }

            result.Add(indexes[first]);
            result.Add(indexes[second]);
            result.Add(indexes[third]);
            /*
            result.Add(indexes[second]);
            result.Add(indexes[first]);
            result.Add(indexes[third]);
            */
        }

        return result;
    }

    private List<int> CalculateTriangles(int start, int count, bool right)
    {
        List<int> result = new List<int>();

        int first, second, third;

        first = start;
        second = start + count - 1;
        third = start + count - 1;

        for (int i = 0; i < count - 2; i++)
        {
            if (i % 2 == 0)
            {
                //first = first;
                third = second;
                second = first + 1;
            }
            else
            {
                first = second;
                second = third - 1;
                //third = third;
            }

            if (right)
            {
                result.Add(first);
                result.Add(second);
                result.Add(third);
            }
            else
            {
                result.Add(second);
                result.Add(first);
                result.Add(third);
            }
            
            /*
            result.Add(second);
            result.Add(first);
            result.Add(third);
            */
        }

        return result;
    }

    private void SetMesh(GameObject go, Vector3[] vertices, int[] triangles)
    {
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

}
