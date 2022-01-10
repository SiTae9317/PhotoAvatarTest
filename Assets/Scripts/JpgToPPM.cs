using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class JpgToPPM : MonoBehaviour
{
    public GameObject sourceObj;
    public GameObject maskObj;

    public Texture2D source;
    public Texture2D mask;

    bool init = false;

    int beforeIndex = -1;
    int checkIndex = 0;
    List<string> checkList;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(jpgToPPMProess());

        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = readPPM("\\\\RAVATAR\\public\\곽희태\\FaceHairSeg\\샘플데이터\\1001 ~\\1020_m.ppm");

        //byte a = (byte)((int)(0.5f * 255.0f));
        //Debug.Log(a);

        //jpgToPPM("ppmtest.jpg");
        //jpgToPPM("1002_m.jpg");
        //jpgToPPM("1_m.jpg");

        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = readPPM("genPPM.ppm");
        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = readPPM("ppmtest.ppm");


        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = readPPM("ppmtest3.ppm");

        ////byte[] readData = File.ReadAllBytes("ppmtest.ppm");

        ////string datas = Encoding.UTF8.GetString(readData);
        ////string[] allD = datas.Split(' ');
        ////for(int i = 0; i < allD.Length; i++)
        ////{
        ////    string ts = allD[i];
        ////    Debug.Log(ts.Length + " " + ts);
        ////}

        //int width = 250;
        //int height = 250;

        //Texture2D tex = new Texture2D(width, height);

        ////using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("ppmtest3.ppm")))
        ////using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("ppmtest2.ppm")))
        //using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("ppmtest.ppm")))
        //{
        //    using (BinaryReader br = new BinaryReader(ms))
        //    {
        //        string header = "";
        //        List<byte> imageData = new List<byte>();

        //        bool isBody = false;

        //        while (ms.Position < ms.Length)
        //        {
        //            byte c = br.ReadByte();

        //            if(c == 10)
        //            {
        //                //isBody = true;

        //                header += Encoding.UTF8.GetString(imageData.ToArray());
        //                imageData.Clear();

        //                continue;
        //            }

        //            imageData.Add(c);

        //            //if(!isBody)
        //            //{
        //            //    header += (char)c;
        //            //}
        //            //else
        //            //{
        //            //    imageData.Add(c);
        //            //}

        //            //if (c == 32)
        //            //{
        //            //    header += (char)c;
        //            //}
        //            //else if(c >= 48 && c < 58)
        //            //{
        //            //    header += (char)c;
        //            //}
        //            //else if(c >= 65 && c < 91)
        //            //{
        //            //    header += (char)c;
        //            //}
        //            //else if(c >= 97 && c < 123)
        //            //{
        //            //    header += (char)c;
        //            //}
        //            //else
        //            //{
        //            //    imageData.Add(c);
        //            //}
        //        }

        //        Debug.Log(imageData[0]);

        //        Debug.Log(header.Length + " " + header);
        //        Debug.Log(imageData.Count);

        //        int idIndex = 0;

        //        for(int y = height - 1; y >= 0; y--)
        //        {
        //            for(int x = 0; x < width; x++)
        //            {
        //                float r = (float)imageData[idIndex + 0] / 255.0f;
        //                float g = (float)imageData[idIndex + 1] / 255.0f;
        //                float b = (float)imageData[idIndex + 2] / 255.0f;

        //                idIndex += 3;

        //                Color curColor = new Color(r, g, b);

        //                tex.SetPixel(x, y, curColor);
        //            }
        //        }
        //        tex.Apply();

        //        gameObject.GetComponent<MeshRenderer>().material.mainTexture = tex;

        //        //string ts = br.ReadString();
        //        //Debug.Log(ts.Length + " " + ts);
        //    }
        //}

        ////File.ReadAllBytes("ppmtest.jpg");
    }

    IEnumerator jpgToPPMProess()
    {
        DirectoryInfo di = new DirectoryInfo("\\\\RAVATAR\\public\\곽희태\\FaceHairSeg\\샘플데이터\\");

        DirectoryInfo[] dis = di.GetDirectories();

        for (int i = 0; i < dis.Length; i++)
        {
            Dictionary<string, string> workList = new Dictionary<string, string>();

            //HashSet<string> workList = new HashSet<string>();
            HashSet<string> workedList = new HashSet<string>();

            DirectoryInfo curDi = dis[i];

            curDi = curDi.GetDirectories()[0];

            Debug.Log(curDi.FullName);
            Debug.Log(curDi.Name);

            FileInfo[] fis = curDi.GetFiles();

            Debug.Log(fis.Length);

            for (int j = 0; j < fis.Length; j++)
            {
                string[] nameNExtension = fis[j].Name.Split('.');
                string curName = nameNExtension[0].ToLower();
                string extension = nameNExtension[1].ToLower();

                if (curName.Contains("_m"))
                {
                    if (extension == "jpg")
                    {
                        if (!workList.ContainsKey(curName))
                        {
                            workList.Add(curName, extension);
                        }
                    }
                    else if (extension == "ppm")
                    {
                        if (workList.ContainsKey(curName))
                        {
                            workList.Remove(curName);
                        }
                        workList.Add(curName, extension);
                    }
                }
                else
                {
                    if (extension == "ppm")
                    {
                        workedList.Add(curName + "_m");
                    }
                }
            }

            HashSet<string>.Enumerator workedListEnum = workedList.GetEnumerator();

            while (workedListEnum.MoveNext())
            {
                string curName = workedListEnum.Current;

                if (workList.ContainsKey(curName))
                {
                    workList.Remove(curName);
                }
            }

            Debug.Log(workList.Count);

            int curCount = 0;

            Dictionary<string, string>.Enumerator worklistEnum = workList.GetEnumerator();

            while (worklistEnum.MoveNext())
            {
                string fullName = curDi.FullName + "\\" + worklistEnum.Current.Key + "." + worklistEnum.Current.Value;

                jpgToPPM(curDi.FullName, worklistEnum.Current.Key, worklistEnum.Current.Value);

                Debug.Log(curCount + "/" + workList.Count);

                yield return null;

                curCount++;
            }
        }

        init = true;
        checkList = checkImagePixel();
    }

    void jpgToPPM(string path, string filename, string extension)
    {
        Texture2D tex = new Texture2D(1, 1);
        if(extension == "jpg")
        {
            tex.LoadImage(File.ReadAllBytes(path + "\\" + filename + "." + extension));
        }
        else if(extension == "ppm")
        {
            tex = readPPM(path + "\\" + filename + "." + extension);
        }

        int width = tex.width;
        int height = tex.height;

        Color[] rgbColors = new Color[3];

        rgbColors[0] = Color.red;
        rgbColors[1] = Color.green;
        rgbColors[2] = Color.blue;

        List<byte> outputData = new List<byte>();

        outputData.AddRange(Encoding.UTF8.GetBytes("P6 250 250 255"));
        outputData.Add((byte)10);

        for (int y = height - 1; y >= 0; y--)
        {
            for(int x = 0; x < width; x++)
            {
                Color keepColor = tex.GetPixel(x, y);

                float minVal = float.MaxValue;

                Color curColor = Color.white;

                for(int i = 0; i < 3; i++)
                {
                    float curVal = ColorUtils.euclidean(keepColor, rgbColors[i], EculideanModel.RGB);

                    if(curVal < minVal)
                    {
                        minVal = curVal;
                        curColor = rgbColors[i];
                    }
                }

                byte r = (byte)((int)(curColor.r * 255.0f));
                byte g = (byte)((int)(curColor.g * 255.0f));
                byte b = (byte)((int)(curColor.b * 255.0f));

                outputData.Add(r);
                outputData.Add(g);
                outputData.Add(b);
            }
        }

        using (FileStream fs = new FileStream(path + "\\" + filename.Replace("_m", "") + ".ppm", FileMode.OpenOrCreate))
        {
            Debug.Log(path + "\\" + filename.Replace("_m", "") + ".ppm");
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                //bw.Write("P6 250 250 255");
                //byte endline = 10;
                //bw.Write((char)endline);
                bw.Write(outputData.ToArray());
            }
        }
    }

    Texture2D readPPM(string filename)
    {
        string header = "";
        List<byte> imageData = new List<byte>();

        //using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("ppmtest3.ppm")))
        //using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("ppmtest2.ppm")))
        //using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("ppmtest.ppm")))

        int index = 0;

        using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(filename)))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                while (ms.Position < ms.Length)
                {
                    byte c = br.ReadByte();

                    if (c == 10 && index < 4)
                    {
                        index++;
                        header += Encoding.UTF8.GetString(imageData.ToArray());
                        imageData.Clear();

                        continue;
                    }

                    imageData.Add(c);
                    //index++;
                }

                //Debug.Log(imageData[0]);

                Debug.Log(header.Length + " " + header);
                Debug.Log(imageData.Count);
            }
        }

        return ppmToTexure2D(250, 250, imageData.ToArray());
    }

    Texture2D ppmToTexure2D(int width, int height, byte[] datas)
    {
        Texture2D tex = new Texture2D(width, height);

        int idIndex = 0;

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                float r = (float)datas[idIndex + 0] / 255.0f;
                float g = (float)datas[idIndex + 1] / 255.0f;
                float b = (float)datas[idIndex + 2] / 255.0f;

                idIndex += 3;

                Color curColor = new Color(r, g, b, 0.25f);

                tex.SetPixel(x, y, curColor);
                //tex.SetPixel(x, y, Color.red);
            }
            //break;
        }
        tex.Apply();

        Debug.Log(idIndex);

        return tex;
    }

    List<string> checkImagePixel()
    {
        DirectoryInfo di = new DirectoryInfo("\\\\RAVATAR\\public\\곽희태\\FaceHairSeg\\샘플데이터\\");

        DirectoryInfo[] dis = di.GetDirectories();

        List<string> workList = new List<string>();

        for (int i = 0; i < dis.Length; i++)
        {
            DirectoryInfo curDi = dis[i];

            FileInfo[] fis = curDi.GetFiles();

            for (int j = 0; j < fis.Length; j++)
            {
                string[] nameNExtension = fis[j].Name.Split('.');
                string curName = nameNExtension[0].ToLower();
                string extension = nameNExtension[1].ToLower();

                if (!curName.Contains("_m"))
                {
                    if (extension == "jpg")
                    {
                        workList.Add(curDi.FullName + "\\" + curName);
                    }
                }
            }
        }

        return workList;
    }

    // Update is called once per frame
    void Update()
    {
        if(!init)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            checkIndex = Mathf.Min(++checkIndex, checkList.Count - 1);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            checkIndex = Mathf.Max(--checkIndex, 0);
        }

        if (beforeIndex != checkIndex)
        {
            beforeIndex = checkIndex;

            if(source != null)
            {
                Destroy(source);
            }

            source = new Texture2D(1, 1);
            source.LoadImage(File.ReadAllBytes(checkList[checkIndex] + ".jpg"));
            source.Apply();

            sourceObj.GetComponent<MeshRenderer>().material.mainTexture = source;

            if (mask != null)
            {
                Destroy(mask);
            }

            mask = readPPM(checkList[checkIndex] + ".ppm");

            maskObj.GetComponent<MeshRenderer>().material.mainTexture = mask;
        }
    }
}
