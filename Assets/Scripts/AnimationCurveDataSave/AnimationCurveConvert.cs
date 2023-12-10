using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class AnimationCurveConvert : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private byte[] serializeCurveData;
    [SerializeField] private AnimationCurve result;
    [SerializeField] private string byteData;
    [SerializeField] private byte[] serializeCurveDat2;
    [SerializeField] private AnimationCurve result2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            serializeCurveData = CurveSerializer.SerializeCurve(curve);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            result = CurveSerializer.DeserializeCurve(serializeCurveData);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            if (0 != serializeCurveData.Length)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < serializeCurveData.Length; i++)
                {
                    if (i == serializeCurveData.Length - 1)
                    {
                        sb.Append($"{serializeCurveData[i]}");
                        continue;
                    }
                    sb.Append($"{serializeCurveData[i]},");
                }
                byteData = sb.ToString();
                Debug.Log(sb.ToString());
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            if (false == string.IsNullOrEmpty(byteData))
            {
                List<byte> bytes = new();
                string[] ss = byteData.Split(',');
                Debug.Log(ss.Length);
                foreach (var item in ss)
                {
                    byte byteValue;
                    if (byte.TryParse(item, out byteValue))
                    {
                        bytes.Add(byteValue);
                    }
                    else
                    {
                        Console.WriteLine($"'{item}' is out of range for a Byte.");
                    }
                }
                serializeCurveDat2 = bytes.ToArray();

                result2 = CurveSerializer.DeserializeCurve(serializeCurveDat2);
            }
        }
    }
}


public static class CurveSerializer
{
    public static byte[] SerializeCurve(AnimationCurve curve)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        formatter.Serialize(stream, new SerializableCurve(curve));

        return stream.ToArray();
    }

    public static AnimationCurve DeserializeCurve(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(data);

        SerializableCurve serializableCurve = (SerializableCurve)formatter.Deserialize(stream);

        return serializableCurve.ToCurve();
    }
}

[System.Serializable]
public class SerializableKeyframe
{
    public float time, value, inTangent, outTangent;

    public SerializableKeyframe(Keyframe key)
    {
        time = key.time;
        value = key.value;
        inTangent = key.inTangent;
        outTangent = key.outTangent;
    }

    public Keyframe ToKeyframe()
    {
        return new Keyframe(time, value, inTangent, outTangent);
    }
}

[System.Serializable]
public class SerializableCurve
{
    public SerializableKeyframe[] keys;

    public SerializableCurve(AnimationCurve curve)
    {
        keys = new SerializableKeyframe[curve.keys.Length];

        for (int i = 0; i < curve.keys.Length; i++)
        {
            keys[i] = new SerializableKeyframe(curve.keys[i]);
        }
    }

    public AnimationCurve ToCurve()
    {
        AnimationCurve curve = new AnimationCurve();

        foreach (SerializableKeyframe key in keys)
        {
            curve.AddKey(key.ToKeyframe());
        }

        return curve;
    }
}