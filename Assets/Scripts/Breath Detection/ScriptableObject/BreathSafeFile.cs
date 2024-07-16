using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreathDetection
{
    [CreateAssetMenu(fileName ="Breathing save file", menuName = "Breathing/Safe file")]
    public class BreathSafeFile : ScriptableObject
    {
        public InhaleData inhaleCalculatedData;
        public InhaleData exhaleCalculatedData;
        public ExhaleData exhaleLoudnessData;
    }
}