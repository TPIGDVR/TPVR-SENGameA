using System;

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
                micProvider.CalculatedVolume < _data.maxDBThreshold
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

    public struct InhaleTester
    {
        MicProvider micProvider;
        InhaleData _data;
        int counter;

        public InhaleTester(MicProvider micProvider , InhaleData template)
        {
            counter = 0;
            this.micProvider = micProvider;

            _data = template;
            //fixed the min and max amplitude. that way u can get more frequent readings
        }

        public void Run()
        {
            counter++;
            _data.maxDBThreshold += micProvider.avgVolume;

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

    public struct ExhaleTester
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