using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGen : MonoBehaviour
{
    //public GameObject leftPoint;
    //public GameObject rightPoint;

    //public GameObject point;

    //private Vector3 center;
    //private Vector3 cYvec;

    //public int rotValue;

    private Vector3 leftP;
    private Vector3 rightP;

    //public float sumTime = 0.0f;

    public void setPoints(Vector3 lp, Vector3 rp)
    {
        leftP = lp;
        rightP = rp;
    }

    public List<Vector3> foreHeadGenerate(int pointCount, int layer = 1)
    {
        //List<Vector3> returnPoint = new List<Vector3>();

        //HashSet<Vector3> points = new HashSet<Vector3>();

        List<Vector3> points = new List<Vector3>();

        Vector3 center = leftP + rightP;
        center /= 2.0f;

        Vector3 axisVec = rightP - leftP;
        axisVec.Normalize();

        float distance = Vector3.Distance(leftP, rightP) / 2.0f;

        float rotValue = 240.0f;

        float divValue = 7.5f;

        if (layer < 10)
        {
            rotValue = 90.0f;
            divValue = 4.0f;
        }

        float splitValue = rotValue / (float)layer;

        for(; rotValue > splitValue; rotValue -= splitValue)
        {
            Quaternion currentRot = Quaternion.AngleAxis(rotValue, axisVec);
            //Quaternion currentRot = Quaternion.AngleAxis(Mathf.Deg2Rad * rotValue, axisVec);

            //float horOffset = 1.0f + (Mathf.Sin(Mathf.Deg2Rad * 2.0f * rotValue) / 10.0f);
            float horOffset = Mathf.Cos(Mathf.Deg2Rad * (rotValue - 15.0f));

            horOffset = horOffset > 0.0f ? 0.0f : horOffset;
            horOffset /= -3.0f;
            horOffset += 1.0f;

            horOffset = Mathf.Min(1.2f, horOffset);

            if (rotValue < 90.0f)
            {
                horOffset = 1.0f;
            }

            //if ( rotValue < 135.0f)
            //{
            //    horOffset = 1.0f;
            //}
            //else if (rotValue < 225.0f)
            //{
            //    horOffset = 1.2f;
            //}
            //else
            //{
            //    horOffset = 1.0f;
            //}

            Matrix4x4 currentMat = Matrix4x4.TRS(center, currentRot, Vector3.one);

            float angleValue = 180.0f / (float)pointCount;
            float len = 180.0f / angleValue;

            for (int i = 1; i < len; i++)
            {
                float currentAngle = angleValue * i;

                float verOffset = 1.0f + (Mathf.Sin(Mathf.Deg2Rad * currentAngle) / divValue);

                float overValue = 1.0f;

                if(i < 4 || i > len - 4)
                {
                    float calcRotValue = Mathf.Min(rotValue, 90.0f);

                    if (i == 3 || i == len - 3)
                    {
                        //verOffset *= 1.0f - Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (90.0f - rotValue))) / 20.0f;
                        verOffset *= 1.0f - (Mathf.Sin(Mathf.Deg2Rad * (90.0f - calcRotValue))) / 20.0f;
                    }
                    else
                    {
                        //verOffset *= 1.0f - Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (90.0f - rotValue))) / 10.0f;
                        verOffset *= 1.0f - (Mathf.Sin(Mathf.Deg2Rad * (90.0f - calcRotValue))) / 10.0f;
                    }

                    if (rotValue > 90.0f)
                    {
                        // a
                        overValue = 1.0f / horOffset;
                        overValue *= (1.0f + Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * currentAngle)) / 10.0f);

                        //// b
                        //overValue = 0.9f / horOffset;
                        //overValue *= (1.0f + Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * currentAngle)) / 5.0f);

                        //overValue *= Mathf.Cos(Mathf.Deg2Rad * (rotValue - 90.0f));
                    }
                }

                //float verOffset = 1.0f + (Mathf.Sin(Mathf.Deg2Rad * currentAngle) / 7.5f);

                //verOffset = Mathf.Exp(Mathf.Pow(Mathf.Sin(Mathf.Deg2Rad * currentAngle) , 2.0f) / 10.0f);

                Vector3 crossBefore = Vector3.RotateTowards(axisVec, -axisVec, Mathf.Deg2Rad * currentAngle, 0.0f);
                crossBefore.Normalize();

                crossBefore = currentMat.MultiplyVector(crossBefore);

                Vector3 tPoint = center + crossBefore * distance * verOffset * horOffset * overValue;

                points.Add(tPoint);
                if (rotValue >= 90.0f && currentAngle == 90.0f)
                {
                    points.Add(tPoint);
                }
            }
        }

        //HashSet<Vector3>.Enumerator pointEnum = points.GetEnumerator();

        //while (pointEnum.MoveNext())
        //{
        //    returnPoint.Add(pointEnum.Current);
        //}

        return points;
    }

    // Start is called before the first frame update
    void Start()
    {
        return;

        //List<int> a = new List<int>();
        //List<int> b = new List<int>();

        List<int> lineAllData = new List<int>();
        List<List<int>> allData = new List<List<int>>();

        for (int i = 245; i > 93; i--)
        {
            lineAllData.Add(i);
            if (i == 240 || i == 229 || i == 218 || i == 207)
            {
                lineAllData.Add(i);
            }
        }

        //int[,] testAllData = new int[12, 13];
        int[][] testAllData = new int[12][];

        for(int i = 0; i < 12; i++)
        {
            testAllData[i] = new int[13];
        }

        int x = 0;
        int y = 0;

        for(int i = 0; i < lineAllData.Count; i++)
        {
            testAllData[x][y] = lineAllData[i];
            x++;
            if(x == 12)
            {
                x = 0;
                y++;
            }
        }

        string sum = "";

        for(int i = 0; i < testAllData.Length - 1; i++)
        {
            int[] a = testAllData[i];
            int[] b = testAllData[i+1];

            if(i == 5)
            {
                continue;
            }

            for (int j = 0; j < a.Length - 1; j++)
            {
                sum += a[0 + j] + " " + a[1 + j] + " " + b[0 + j];
                sum += "\r\n";
                sum += a[1 + j] + " " + b[1 + j] + " " + b[0 + j];
                sum += "\r\n";
            }
        }

        for(int i = 0; i < testAllData[0].Length - 1; i++)
        {
            int[] a = testAllData[0];

            sum += a[i] + " 92 " + a[i + 1];
            sum += "\r\n";
        }

        for (int i = 0; i < testAllData[11].Length - 1; i++)
        {
            int[] a = testAllData[11];

            sum += a[i] + " " + a[i + 1] + " 93";
            sum += "\r\n";
        }

        Debug.Log(sum);

        //bool isOnemore = false;

        //for (int i = 245; i > 234; i--)
        //{
        //    List<int> lineData = new List<int>();

        //    int keepData = i;

        //    for(int j = 0; j < 13; j++)
        //    {
        //        lineData.Add(keepData);

        //        Debug.Log(keepData);

        //        int otherValue = 0;

        //        if(j > 3 && isOnemore)
        //        {
        //            otherValue = 1;
        //        }

        //        keepData -= (11 + otherValue);
        //    }

        //    if(i == 240 && !isOnemore)
        //    {
        //        isOnemore = true;
        //        i++;
        //    }

        //    allData.Add(lineData);
        //}

        //for(int i = 0; i < allData.Count - 1; i++)
        //{
        //    //if(i == allData.Count / 2)
        //    //{
        //    //    i++;
        //    //}
        //    List<int> a = allData[i];
        //    List<int> b = allData[i + 1];

        //    for (int j = 0; j < a.Count - 1; j++)
        //    {
        //        sum += a[0 + j] + " " + a[1 + j] + " " + b[0 + j];
        //        sum += "\r\n";
        //        sum += a[1 + j] + " " + b[1 + j] + " " + b[0 + j];
        //        sum += "\r\n";
        //    }
        //}

        //Debug.Log(sum);

        //for (int i = 236; i > 226; i--)
        //{
        //    a.Clear();
        //    b.Clear();

        //    int keepA = i;
        //    int keepB = i - 1;

        //    for (int j = 0; j < 13; j++)
        //    {
        //        a.Add(keepA);
        //        b.Add(keepB);

        //        keepA -= 11;
        //        keepB -= 11;
        //    }
        //    //for (int j = 0; j < a.Count - 1; j++)
        //    //{
        //    //    sum += a[0 + j] + " " + a[1 + j] + " " + b[0 + j];
        //    //    sum += "\r\n";
        //    //    sum += a[1 + j] + " " + b[1 + j] + " " + b[0 + j];
        //    //    sum += "\r\n";
        //    //}
        //}
        //Debug.Log(sum);
        //Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 45.0f));
        //Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 90.0f));
        //Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 135.0f));
        //Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 180.0f));
        //Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 225.0f));
        //Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 270.0f));
        //Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 315.0f));
        //Debug.Log(Mathf.Cos(Mathf.Deg2Rad * 360.0f));
        //int[] a = { 125, 115, 105, 95 };
        //int[] b = { 124, 114, 104, 94 };

        //string sum = "";
        //for(int i = 0; i < a.Length - 1; i++)
        //{
        //    sum += a[0 + i] + " " + a[1 + i] + " " + b[0 + i];
        //    sum += "\r\n";
        //    sum += a[1 + i] + " " + b[1 + i] + " " + b[0 + i];
        //    sum += "\r\n";
        //}
        //Debug.Log(sum);

        //    setPoints(leftPoint.transform.position, rightPoint.transform.position);
        //    foreHeadGenerate(11, 4);

        //center = leftPoint.transform.position + rightPoint.transform.position;

        //center /= 2.0f;

        //Vector3 cXvec = rightPoint.transform.position - leftPoint.transform.position;
        //cXvec.Normalize();

        //cYvec = Vector3.Cross(cXvec, Vector3.back);
        //cYvec.Normalize();

        //Debug.DrawRay(center, cXvec, Color.red);
        //Debug.DrawRay(center, cYvec, Color.green);

        ////180.0f / 11.0f;
        //for (int i = 0; i < 18; i++)
        //{
        //    GameObject newObj = Instantiate(point);
        //    Vector3 crossBefore = Vector3.RotateTowards(cXvec, -cXvec, Mathf.Deg2Rad * (10.0f * (i)), 0.0f);

        //    //Vector3 crossAfter = Vector3.Cross(crossBefore, -crossBefore);

        //    //crossAfter = new Vector3(crossBefore.x, -crossBefore.z, crossBefore.y);
        //    //crossAfter = Vector3.Cross(crossBefore, crossAfter);

        //    newObj.transform.position = center + crossBefore;

        //    points.Add(newObj);
        //}
        ////for (int i = 0; i < 10; i++)
        ////{
        ////    GameObject newObj = Instantiate(point);

        ////    newObj.transform.position = center + Vector3.RotateTowards(cYvec, -cXvec, Mathf.Deg2Rad * (180.0f / 11.0f * (i)), 0.0f);
        ////}
    }

    // Update is called once per frame
    void Update()
    {
        //sumTime += Time.deltaTime;
        //Debug.Log(Mathf.Exp(sumTime));
        //center = leftPoint.transform.position + rightPoint.transform.position;

        //center /= 2.0f;

        //Vector3 cXvec = rightPoint.transform.position - leftPoint.transform.position;
        //cXvec.Normalize();

        //cYvec = Vector3.Cross(cXvec, Vector3.back);
        //cYvec.Normalize();

        //Debug.DrawRay(center, cXvec, Color.red);
        //Debug.DrawRay(center, cYvec, Color.green);

        //Matrix4x4 mat = new Matrix4x4();
        
        //mat = Matrix4x4.TRS(center, Quaternion.AxisAngle(cXvec, Mathf.Deg2Rad * rotValue), Vector3.one);

        ////180.0f / 11.0f;
        //for (int i = 0; i < 18; i++)
        //{
        //    Vector3 crossBefore = Vector3.RotateTowards(cXvec, -cXvec, Mathf.Deg2Rad * (10.0f * (i)), 0.0f);
        //    crossBefore.Normalize();
            
        //    crossBefore = mat.MultiplyVector(crossBefore);
        //    //crossBefore.Normalize();
        //    //Vector3 crossAfter = Vector3.Cross(crossBefore, -crossBefore);

        //    //crossAfter = new Vector3(crossBefore.x, -crossBefore.z, crossBefore.y);
        //    //crossAfter = Vector3.Cross(crossBefore, crossAfter);

        //    points[i].transform.position = center + crossBefore;
        //}
        ////center -= Vector3.up * 0.1f;

        ////for (int i = 1; i < 9; i++)
        ////{
        ////    Debug.DrawRay(center, Vector3.RotateTowards(cXvec, cYvec, Mathf.Deg2Rad * (10.0f * (i + 1)), 0.0f), Color.blue);
        ////}
        ////for (int i = 0; i < 8; i++)
        ////{
        ////    Debug.DrawRay(center, Vector3.RotateTowards(cYvec, -cXvec, Mathf.Deg2Rad * (10.0f * (i + 1)), 0.0f), Color.blue);
        ////}
    }
}
