using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Dalgona
{
    Triangle,
    Star,
    Umbrella,
    Circle,
    DalgonaCount
}

public class DalgonaData
{
    public static float[][] g_ClearDalPS = new float[4][];
    public static float[][] g_FailDalPS = new float[4][];
    public static float[] g_TriPhase = new float[7];
    public static float[] g_StarPhase = new float[6];
    public static float[] g_UmbPhase = new float[6];
    public static float[] g_CirPhase = new float[5];

    public static Dalgona g_DalShape = Dalgona.DalgonaCount;

    public static void SetPhase()
	{
        g_TriPhase = new float[7] { 8.7f, 33.3f, 50.3f, 66.6f, 81.0f, 100.0f, 101.0f };
        g_StarPhase = new float[6] { 20.2f, 39.5f, 59.1f, 79.8f, 100.0f, 101.0f };
        g_UmbPhase = new float[6] { 15.9f, 44.3f, 60.3f, 82.8f, 100.0f, 101.0f };
        g_CirPhase = new float[5] { 25.1f, 53.3f, 74.6f, 100.0f, 101.0f };
    }
}
