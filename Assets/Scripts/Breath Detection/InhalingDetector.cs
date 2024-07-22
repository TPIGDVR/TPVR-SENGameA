using System;
using System.Collections.Generic;
using UnityEngine;

namespace BreathDetection
{
    public class InhalingDetector : IBreathResult
    {
        MicProvider micProvider;
        InhaleData _data;
        int inhaleCounter = 0;

        float minValue => inhaleCounter < _data.inhaleCounterThreshold ?
            _data.minNumberOfCommonPoint :
            Math.Min(_data.minNumberOfCommonPoint - _data.reductionOfMinCounter, 1);

        public InhalingDetector(MicProvider micProvider, InhaleData data)
        {
            this.micProvider = micProvider;
            _data = data;
        }

        public bool Result()
        {
            var spectrumData = micProvider._dataSpectrumContainer;
            int lowCutOff = (int)(_data.lowPassFilter / micProvider.pitchIncrementor);
            int highCutOff = (int)(_data.highPassFilter / micProvider.pitchIncrementor);

            int counter = 0;

            for (int i = lowCutOff; i < highCutOff; i++)
            {
                var val = spectrumData[i];
                if (val > _data.minAmplitude && val < _data.maxAmplitude)
                {
                    counter++;
                }
            }

            if (counter > minValue &&
                counter < _data.maxNumberOfCommonPoint &&
                micProvider.CalculatedVolume > _data.maxDBThreshold
                )
            {
                if(inhaleCounter < _data.inhaleCounterThreshold)
                {
                    inhaleCounter++;
                    return false;
                }
                return true;
            }
            else
            {
                inhaleCounter = 0;
                return false;
            }
        }
    }

    public class ExhalingDetector : IBreathResult
    {
        MicProvider micProvider;
        ExhaleData _data;

        public ExhalingDetector(MicProvider micProvider, ExhaleData data)
        {
            this.micProvider = micProvider;
            _data = data;
        }

        public bool Result()
        {
            return micProvider.volume > _data.volumeThreshold &&
                micProvider.pitch > _data.minPitchThreshold;
        }
    }

    public struct SpectrumTester : ITestable<InhaleData>
    {
        MicProvider micProvider;
        InhaleData _data;
        int counter;

        public SpectrumTester(MicProvider micProvider , InhaleData template)
        {
            counter = 0;
            this.micProvider = micProvider;

            _data = template;
            //fixed the min and max amplitude. that way u can get more frequent readings
        }

        public void Run()
        {
            counter++;
            _data.maxDBThreshold += micProvider.maxVolume;


            var spectrumData = micProvider._dataSpectrumContainer;
            int lowCutOff = (int)(_data.lowPassFilter / micProvider.pitchIncrementor);
            int highCutOff = (int)(_data.highPassFilter / micProvider.pitchIncrementor);

            int pointCounter = 0;

            for (int i = lowCutOff; i < highCutOff; i++)
            {
                var val = spectrumData[i];
                if (val > _data.minAmplitude && val < _data.maxAmplitude)
                {
                    pointCounter++;
                }
            }

            Debug.Log($"Common point {pointCounter}");

            if(pointCounter > _data.maxNumberOfCommonPoint)
            {
                _data.maxNumberOfCommonPoint = pointCounter;
            }
            _data.minNumberOfCommonPoint += pointCounter;
            
        }

        public void Reset(InhaleData templateData)
        {
            _data = templateData;
            counter = 0;
        }

        public InhaleData Calculate()
        {
            _data.minNumberOfCommonPoint /= counter; //take average
            _data.maxDBThreshold /= counter;
            return _data;
        }
    }

    public struct SpectrumMinMaxTester : ITestable<InhaleData>
    {
        public InhaleData _data;
        private MicProvider micProvider;
        float maxAmp;
        float avgAmp;
        int counter;
        public SpectrumMinMaxTester(MicProvider micProvider, InhaleData data)
        {
            _data = data;
            this.micProvider = micProvider;
            maxAmp = 0;
            avgAmp = 0;
            counter = 0;
        }

        public InhaleData Calculate()
        {
            _data.maxAmplitude = maxAmp/ counter;
            _data.minAmplitude = avgAmp/ counter;
            return _data;
        }

        public void Reset(InhaleData templateData)
        {
            counter = 0;
            maxAmp = 0;
            avgAmp = 0;
        }

        public void Run()
        {
            var spectrumData = micProvider._dataSpectrumContainer;
            int lowCutOff = (int)(_data.lowPassFilter / micProvider.pitchIncrementor);
            int highCutOff = (int)(_data.highPassFilter / micProvider.pitchIncrementor);

            float curMaxFloat = 0;
            List<float> maxBuffer = new();

            for (int i = lowCutOff; i < highCutOff; i++)
            {
                var amp = spectrumData[i];
                if(maxBuffer.Count == 0) maxBuffer.Add(amp);
                else
                {
                    bool hasInsert = false;
                    for(int j = 0; j < maxBuffer.Count; j++)
                    {
                        if (amp > maxBuffer[j] )
                        {
                            maxBuffer.Insert(j, amp);
                            hasInsert = true;
                            break;
                        }
                    }
                    if (!hasInsert && maxBuffer.Count < _data.numberOfPointsToStop)
                    {
                        maxBuffer.Add(amp);
                    }
                    //remove the last buffer if it overflow
                    if(maxBuffer.Count > _data.numberOfPointsToStop)
                    {
                        maxBuffer.RemoveAt(maxBuffer.Count - 1);
                    }
                }

            }

            maxAmp += maxBuffer[0];
            avgAmp += maxBuffer[maxBuffer.Count - 1];
            counter++;
        }
    }

    public struct ExhaleTester : ITestable<ExhaleData>
    {
        ExhaleData _data;
        MicProvider micProvider;
        int counter;

        public ExhaleTester(ExhaleData data, MicProvider micProvider) : this()
        {
            _data = data;
            this.micProvider = micProvider;
            counter = 0;
        }

        public void Run()
        {
            counter++;
            _data.volumeThreshold += micProvider.CalculatedVolume;
            _data.maxPitchThreshold += micProvider.maxPitch;
            _data.minPitchThreshold += micProvider.minPitch;
        }

        public void Reset(ExhaleData templateData)
        {
            _data = templateData;
            counter = 0;
        }

        public ExhaleData Calculate()
        {
            _data.volumeThreshold /= counter;
            _data.maxPitchThreshold /= counter;
            _data.minPitchThreshold /= counter;
            return _data;
        }
    }

    
}