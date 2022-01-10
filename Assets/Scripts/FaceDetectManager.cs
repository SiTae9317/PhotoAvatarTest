using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DlibFaceLandmarkDetector.UnityUtils;
using DlibFaceLandmarkDetector;

namespace DlibFaceLandmarkDetectorExample
{
    /// <summary>
    /// Texture2D Example
    /// An example of detecting face landmarks in a Texture2D image.
    /// </summary>
    public class FaceDetectManager : MonoBehaviour
    {
        /// <summary>
        /// The texture2D.
        /// </summary>

        public bool isDrawing = false;
        public bool isExport = false;
        public bool isGenPoints = false;

        public GameObject pointObj;

        public Texture2D texture2D;

        Texture2D genTexture;

        public MeshIndexResorting3 mir3;

        public FaceLandmarkMatching flm;

        public DynamicPointGenerator dpg;

        public FaceHairSegmentation fhs;

        public DetectHairPixel dhp;

        /// <summary>
        /// The FPS monitor.
        /// </summary>
        FpsMonitor fpsMonitor;

        /// <summary>
        /// The dlib shape predictor file name.
        /// </summary>
        string dlibShapePredictorFileName = "sp_human_face_68.dat";

        /// <summary>
        /// The dlib shape predictor file path.
        /// </summary>
        string dlibShapePredictorFilePath;

#if UNITY_WEBGL && !UNITY_EDITOR
        IEnumerator getFilePath_Coroutine;
#endif

        // Use this for initialization
        void Start()
        {
            //fpsMonitor = GetComponent<FpsMonitor>();

            dlibShapePredictorFileName = DlibFaceLandmarkDetectorExample.dlibShapePredictorFileName;
#if UNITY_WEBGL && !UNITY_EDITOR
            getFilePath_Coroutine = Utils.getFilePathAsync (dlibShapePredictorFileName, (result) => {
                getFilePath_Coroutine = null;

                dlibShapePredictorFilePath = result;
                Run ();
            });
            StartCoroutine (getFilePath_Coroutine);
#else
            dlibShapePredictorFilePath = Utils.getFilePath(dlibShapePredictorFileName);

            //fhs.runFHS(texture2D.EncodeToJPG(), Run);

            Run();

            //Run ();
#endif
        }

        private void Run()
        {
            if (string.IsNullOrEmpty(dlibShapePredictorFilePath))
            {
                Debug.LogError("shape predictor file does not exist. Please copy from “DlibFaceLandmarkDetector/StreamingAssets/” to “Assets/StreamingAssets/” folder. ");
            }

            //if true, The error log of the Native side Dlib will be displayed on the Unity Editor Console.
            Utils.setDebugMode(true);

            Debug.Log(texture2D.mipmapCount);
            //texture2D = fhs.getCurrentTex();

            Texture2D dstTexture2D = new Texture2D(texture2D.width, texture2D.height, texture2D.format, false);
            //Texture2D dstTexture2D = new Texture2D (texture2D.width, texture2D.height, texture2D.format, false);
            Graphics.CopyTexture(texture2D, dstTexture2D);

            //texture2D = fhs.getCurrentTex();
            //texture2D = gameObject.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

            //gameObject.transform.position = new Vector3(texture2D.width / 2.0f, texture2D.height / 2.0f, 0.0f);

            gameObject.transform.localScale = new Vector3(texture2D.width, 1, texture2D.height);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = gameObject.transform.localScale.x;
            float height = gameObject.transform.localScale.z;

            gameObject.transform.localScale = new Vector3(width / 10.0f, height / 10.0f, 1);

            gameObject.transform.localScale *= 10.0f;


            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }

            FaceLandmarkDetector faceLandmarkDetector = new FaceLandmarkDetector(dlibShapePredictorFilePath);
            faceLandmarkDetector.SetImage(texture2D);

            //detect face rects
            List<Rect> detectResult = faceLandmarkDetector.Detect();

            List<Vector3> basePoints = new List<Vector3>();

            if (isDrawing)
            {
                genTexture = new Texture2D(texture2D.width, texture2D.height);
                genTexture.name = texture2D.name;

                genTexture.SetPixels(texture2D.GetPixels());
                genTexture.Apply();
            }

            pointObj.SetActive(true);

            int curIndex = 0;

            foreach (var rect in detectResult)
            {
                Debug.Log("face : " + rect);

                //detect landmark points
                List<Vector2> points = faceLandmarkDetector.DetectLandmark(rect);

                Debug.Log("face points count : " + points.Count);
                foreach (var point in points)
                {
                    //					Vector3 screenPos = new Vector3 ((point.x / (float)texture2D.width * 1440.0f), (((float)texture2D.height - point.y) / (float)texture2D.height * 2560.0f), 0.0f);
                    //					GameObject go = Instantiate (sphere);
                    //					go.transform.position = cam.ScreenToWorldPoint (screenPos);
                    //                    Debug.Log ("face point : x " + point.x + " y " + point.y);

                    Vector3 newPoint = new Vector3(point.x - width / 2.0f, height - point.y - height / 2.0f, 0.0f);
                    //Vector3 newPoint = new Vector3 (point.x, height - point.y , 0.0f);


                    GameObject newGo = Instantiate(pointObj);
                    newGo.name = curIndex.ToString();
                    curIndex++;
                    newGo.transform.position = newPoint;

                    basePoints.Add(newPoint);

                    if (isDrawing)
                    {
                        int pixelCount = 2;
                        int offset = pixelCount / 2;

                        for (int y = 0; y < pixelCount; y++)
                        {
                            for (int x = 0; x < pixelCount; x++)
                            {
                                genTexture.SetPixel((int)point.x + (x - offset), genTexture.height - (int)point.y + (y - offset), Color.green);
                            }
                        }
                    }
                }
                pointObj.SetActive(false);

                //draw landmark points
                //                faceLandmarkDetector.DrawDetectLandmarkResult (dstTexture2D, 0, 255, 0, 255);

                if (isDrawing)
                {
                    genTexture.Apply();
                }
                if (isExport)
                {
                    System.IO.File.WriteAllBytes(genTexture.name + "_Point.jpg", genTexture.EncodeToJPG());
                }
            }

            //draw face rect
            //            faceLandmarkDetector.DrawDetectResult (dstTexture2D, 255, 0, 0, 255, 2);

            faceLandmarkDetector.Dispose();

            if (flm != null)
            {
                flm.savePoints(basePoints);
                //      		flm.pointMatching (basePoints);
            }

            if (dpg != null)
            {
                dpg.initialize();
                dpg.setValue(basePoints);
                dpg.generateDPS();
                //dpg.showPoints();
            }

            gameObject.GetComponent<Renderer>().material.mainTexture = dstTexture2D;

            Utils.setDebugMode(false);

            mir3.genWire(basePoints);

            //            if (fpsMonitor != null) {                
            //                fpsMonitor.Add ("dlib shape predictor", dlibShapePredictorFileName);
            //                fpsMonitor.Add ("width", width.ToString ());
            //                fpsMonitor.Add ("height", height.ToString ());
            //                fpsMonitor.Add ("orientation", Screen.orientation.ToString ());
            //            }

            //			genTexture.Apply ();

            //			gameObject.GetComponent<HighpassFilter> ().startHPF ();

            //			gameObject.GetComponent<MeshRenderer> ().material.mainTexture = genTexture;

            //gameObject.transform.position -= Vector3.forward * 10.0f;

            //			gameObject.SetActive (false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (dhp != null)
                {
                    dhp.gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Raises the disable event.
        /// </summary>
        void OnDisable()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (getFilePath_Coroutine != null) {
                StopCoroutine (getFilePath_Coroutine);
                ((IDisposable)getFilePath_Coroutine).Dispose ();
            }
#endif
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("DlibFaceLandmarkDetectorExample");
        }
    }
}