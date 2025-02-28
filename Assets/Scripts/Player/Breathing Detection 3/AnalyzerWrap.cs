using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnalyzerWrap : MonoBehaviour
{
    public List<AudioClip> InhaleClips;
    public List<AudioClip> ExhaleClips;
    public List<AudioClip> SpeechClips;
    string SystemInformation;
    public LineRenderer lineRenderer;

    public void Start()
    {
        var a = AudioSettings.GetConfiguration();
        SystemInformation = "Audio Settings\n";
        SystemInformation += "Sample Rate: " + a.sampleRate + "\n";
        SystemInformation += "DSP Buffer Size: " + a.dspBufferSize + "\n";
        SystemInformation += "Speaker Mode: " + a.speakerMode + "\n\n\n";

        var b = InhaleClips[0];
        print($"Sample Rate: {b.frequency}, {b.samples / b.length},{AudioSettings.outputSampleRate}");

        AudioAnalyzer.Initialize(InhaleClips);
        var res = AudioAnalyzer.AnalyzeData();

        WriteResultsToFile("Inhale", ResultToString(res));

        AudioAnalyzer.Initialize(ExhaleClips);
        var res2 = AudioAnalyzer.AnalyzeData();

        WriteResultsToFile("Exhale", ResultToString(res2));

        AudioAnalyzer.Initialize(SpeechClips);
        var res3 = AudioAnalyzer.AnalyzeData();

        WriteResultsToFile("Speech", ResultToString(res3));
    }


    string ResultToString(AudioAnalysisResult r)
    {
        return $"Avg RMS : {r.AvgRMS * 100}\nAvg Derivative : {r.AvgDerivative * 100000}\nAvg ZCR : {r.AvgZCR}\nAvg Spec Centroid : {r.AvgSpecCentroid}";
    }

    public void WriteResultsToFile(string fileName, string result)
    {
        string path = Path.Combine(Application.dataPath,"Scripts/Player/Breathing Detection 3", fileName + ".txt");
        // string path = "C:/Users/yc/Documents/unityprojects/TPVR-SENGameA/Assets/Scripts/Player/Breathing Detection 3";
        if (!File.Exists(path))
        {
            // Directory.CreateDirectory(path);
            File.CreateText(path);
        }

        string content = SystemInformation + result;
        File.WriteAllText(path, content);
        print($"{fileName}.txt Written");
    }
}
