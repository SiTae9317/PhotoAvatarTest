using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EculideanModel
{
    GRAY,
    RGB,
    HSV,
    HSV_HUE,
    HSV_CONIC,
    HSV_CYLINDRIC,
    CIE_LAB
}

public class LAB
{
    public float l;
    public float a;
    public float b;
}

public class HSV
{
    public float h;     // Hue: 0 ~ 255 (red:0, gree: 85, blue: 171)
    public float s;     // Saturation: 0 ~ 255
    public float v;     // Value: 0 ~ 255
}

public class ColorUtils
{
    public const float maxRGBDistance = 1.732051f;
    //public Color inputColor;
    //public Color outputColor;


    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //HSV hsv = RGBToHSV(inputColor);
    //    //outputColor = HSVToRGB(hsv);
    //}

    public static float minColorValue(Color curColor)
    {
        return minColorValue(curColor.r, curColor.g, curColor.b);
    }

    public static float maxColorValue(Color curColor)
    {
        return maxColorValue(curColor.r, curColor.g, curColor.b);
    }

    public static float minColorValue(float r, float g, float b)
    {
        return ((g) <= (r) ? ((b) <= (g) ? (b) : (g)) : ((b) <= (r) ? (b) : (r)));
    }

    public static float maxColorValue(float r, float g, float b)
    {
        return ((g) >= (r) ? ((b) >= (g) ? (b) : (g)) : ((b) >= (r) ? (b) : (r)));
    }

    public static float euclidean(Color left, Color right, EculideanModel model)
    {
        float d = 0.0f;

        switch(model)
        {
            case EculideanModel.GRAY:
                {
                    float lGray = left.grayscale;
                    float rGray = right.grayscale;

                    d = Mathf.Abs(lGray - rGray);

                    break;
                }
            case EculideanModel.RGB:
                {
                    d = euclidean(left.r, left.g, left.b, right.r, right.g, right.b);

                    break;
                }
            case EculideanModel.HSV:
                {
                    HSV leftHSV = RGBToHSV(left);
                    HSV rightHSV = RGBToHSV(right);

                    d = euclidean(distanceHUE(leftHSV.h, rightHSV.h), leftHSV.s, leftHSV.v, 0.0f, rightHSV.s, rightHSV.v);

                    break;
                }
            case EculideanModel.HSV_HUE:
                {
                    HSV leftHSV = RGBToHSV(left);
                    HSV rightHSV = RGBToHSV(right);

                    float lH = leftHSV.h;
                    float rH = rightHSV.h;

                    d = distanceHUE(lH, rH);

                    break;
                }
            case EculideanModel.HSV_CONIC:
                {
                    HSV leftHSV = RGBToHSV(left);
                    HSV rightHSV = RGBToHSV(right);

                    float lx = leftHSV.s * Mathf.Cos((2.0f * Mathf.PI * leftHSV.h) / 255.0f) * leftHSV.v / 255.0f;
                    float ly = leftHSV.s * Mathf.Sin((2.0f * Mathf.PI * leftHSV.h) / 255.0f) * leftHSV.v / 255.0f;
                    float lz = leftHSV.v;

                    float rx = rightHSV.s * Mathf.Cos((2.0f * Mathf.PI * rightHSV.h) / 255.0f) * rightHSV.v / 255.0f;
                    float ry = rightHSV.s * Mathf.Sin((2.0f * Mathf.PI * rightHSV.h) / 255.0f) * rightHSV.v / 255.0f;
                    float rz = rightHSV.v;

                    d = euclidean(lx, ly, lz, rx, ry, rz);

                    break;
                }
            case EculideanModel.HSV_CYLINDRIC:
                {
                    HSV leftHSV = RGBToHSV(left);
                    HSV rightHSV = RGBToHSV(right);

                    float lx = leftHSV.s * Mathf.Cos((2.0f * Mathf.PI * leftHSV.h) / 255.0f);
                    float ly = leftHSV.s * Mathf.Sin((2.0f * Mathf.PI * leftHSV.h) / 255.0f);
                    float lz = leftHSV.v;

                    float rx = rightHSV.s * Mathf.Cos((2.0f * Mathf.PI * rightHSV.h) / 255.0f);
                    float ry = rightHSV.s * Mathf.Sin((2.0f * Mathf.PI * rightHSV.h) / 255.0f);
                    float rz = rightHSV.v;

                    d = euclidean(lx, ly, lz, rx, ry, rz);

                    break;
                }
            case EculideanModel.CIE_LAB:
                {
                    LAB leftLab = RGBToLAB(left);
                    LAB rightLab = RGBToLAB(right);

                    d = euclidean(leftLab.l, leftLab.a, leftLab.b, rightLab.l, rightLab.a, rightLab.b);

                    break;
                }
        }

        return d;
    }

    public static float euclidean(float lx, float ly, float lz, float rx, float ry, float rz)
    {
        return Mathf.Sqrt(Mathf.Pow((lx - rx), 2.0f) + Mathf.Pow((ly - ry), 2.0f) + Mathf.Pow((lz - rz), 2.0f));
    }

    //public static float euclideanFromRGB(Color left, Color right)
    //{
    //    return euclideanFromRGB(left.r, left.g, left.b, right.r, right.g, right.b);
    //}

    //public static float euclideanFromRGB(float r1, float g1, float b1, float r2, float g2, float b2)
    //{
    //    return Mathf.Sqrt(Mathf.Pow((r1 - r2), 2.0f) + Mathf.Pow((g1 - g2), 2.0f) + Mathf.Pow((b1 - b2), 2.0f));
    //}

    //public static float euclideanFromHSV(HSV left, HSV right)
    //{
    //    return euclideanFromHSV(left.h, left.s, left.v, right.h, right.s, right.v);
    //}

    //public static float euclideanFromHSV(float h1, float s1, float v1, float h2, float s2, float v2)
    //{
    //    return Mathf.Sqrt(Mathf.Pow((h1 - h2), 2.0f) + Mathf.Pow((s1 - s2), 2.0f) + Mathf.Pow((v1 - v2), 2.0f));
    //}

    public static float distanceHUE(float lH, float rH)
    {
        float minVal = Mathf.Min(lH, rH);
        float maxVal = Mathf.Max(lH, rH);

        float dis0 = maxVal - minVal;
        float dis1 = (360 - maxVal) + minVal;

        return Mathf.Min(dis0, dis1);// / 360.0f;
    }

    public static HSV RGBToHSV(Color curColor)
    {
        HSV hsv = new HSV();
        float r = curColor.r;
        float g = curColor.g;
        float b = curColor.b;

        float maxColor;
        float minColor;
        float delta;

        minColor = minColorValue(r, g, b);
        maxColor = maxColorValue(r, g, b);

        hsv.v = maxColor;

        delta = maxColor - minColor;

        if (hsv.v == 0.0f)
        {
            hsv.h = 0.0f;
            hsv.s = 0.0f;

            return hsv;
        }

        hsv.s = delta / hsv.v;

        if (hsv.s == 0.0f)
        {
            hsv.h = 0.0f;

            return hsv;
        }

        if (maxColor == r)
        {
            hsv.h = (g - b) / delta;
        }
        else if (maxColor == g)
        {
            hsv.h = 2 + (b - r) / delta;
        }
        else if (maxColor == b)
        {
            hsv.h = 4 + (r - g) / delta;
        }

        hsv.h = hsv.h * 60.0f;

        if (hsv.h < 0.0f)
        {
            hsv.h += 360.0f;
        }

        return hsv;
    }

    public static Color HSVToRGB(HSV hsv)
    {
        Color returnColor = Color.black;

        float hh, p, q, t, ff;
        int i;

        if(hsv.s <= 0.0f)
        {
            returnColor.r = hsv.v;
            returnColor.g = hsv.v;
            returnColor.b = hsv.v;

            return returnColor;
        }

        hh = hsv.h;

        if (hh >= 360.0f)
            hh = 0.0f;

        hh /= 60.0f;

        i = (int)hh;

        ff = hh - (float)i;
        p = hsv.v * (1.0f - hsv.s);
        q = hsv.v * (1.0f - (hsv.s * ff));
        t = hsv.v * (1.0f - (hsv.s * (1.0f - ff)));

        switch (i)
        {
            case 0:
                returnColor.r = hsv.v;
                returnColor.g = t;
                returnColor.b = p;
                break;
            case 1:
                returnColor.r = q;
                returnColor.g = hsv.v;
                returnColor.b = p;
                break;
            case 2:
                returnColor.r = p;
                returnColor.g = hsv.v;
                returnColor.b = t;
                break;
            case 3:
                returnColor.r = p;
                returnColor.g = q;
                returnColor.b = hsv.v;
                break;
            case 4:
                returnColor.r = t;
                returnColor.g = p;
                returnColor.b = hsv.v;
                break;
            case 5:
            default:
                returnColor.r = hsv.v;
                returnColor.g = p;
                returnColor.b = q;
                break;
        }

        return returnColor;
    }

    public static LAB RGBToLAB(Color curColor)
    {
        LAB lab = new LAB();

        double[] rgb = new double[3];

        rgb[0] = (double)curColor.r;
        rgb[1] = (double)curColor.g;
        rgb[2] = (double)curColor.b;

        for (int i = 0; i < 3; i++)
        {
            double value = rgb[i]; 

            if(value > 0.04045)
            {
                value = System.Math.Pow(((value + 0.055) / 1.055), 2.4);
            }
            else
            {
                value = value / 12.92;
            }

            rgb[i] = value * 100.0;
        }

        double[] xyz = new double[3];

        double x = rgb[0] * 0.4124f + rgb[1] * 0.3576f + rgb[2] * 0.1805f;
        double y = rgb[0] * 0.2126f + rgb[1] * 0.7152f + rgb[2] * 0.0722f;
        double z = rgb[0] * 0.0193f + rgb[1] * 0.1192f + rgb[2] * 0.9505f;

        xyz[0] = round(x, 4);
        xyz[1] = round(y, 4);
        xyz[2] = round(z, 4);

        xyz[0] = (float)xyz[0] / 95.047f;
        xyz[1] = (float)xyz[1] / 100.0f;
        xyz[2] = (float)xyz[2] / 108.883f;

        for(int i = 0; i < 3; i++)
        {
            double value = xyz[i];

            if(value > 0.008856)
            {
                value = System.Math.Pow(value, 1.0 / 3.0);
            }
            else
            {
                value = (7.787 * value) + (16.0 / 116.0);
            }

            xyz[i] = value;
        }

        double l = (116.0 * xyz[1]) - 16.0;
        double a = 500.0 * (xyz[0] - xyz[1]);
        double b = 200.0 * (xyz[1] - xyz[2]);

        l = round(l, 4);
        a = round(a, 4);
        b = round(b, 4);

        lab.l = (float)l;
        lab.a = (float)a;
        lab.b = (float)b;

        return lab;
    }

    public static double round(double value, int point)
    {
        double returnValue = 0.0;

        double highVal = (double)((int)value);
        double lowVal = value % 1.0;

        //if (lowVal >= 0.5)
        //{
        //    highVal -= 1.0;
        //}

        double pVal = System.Math.Pow(10.0, point);

        lowVal *= pVal;

        double lowHighVal = (double)((int)lowVal);
        double lowLowVal = lowVal % 1.0;

        if (lowLowVal >= 0.499999)
        {
            lowHighVal += 1.0;
        }

        lowHighVal /= pVal;

        returnValue = highVal + lowHighVal;

        return returnValue;
    }
}
