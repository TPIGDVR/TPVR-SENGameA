using UnityEngine;
using UnityEngine.UI;
using Breathing3;
using TMPro;

namespace Assets.Scripts.Experiement__Voice_Recognition_
{
    public class AttController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI mainText;
        [SerializeField] Text attText;
        [SerializeField] NoiseReducerInserter inserter;
        [SerializeField] Button incBtn;
        [SerializeField] Button decBtn;
        [SerializeField] float increaseRate = 1f;

        private void OnEnable()
        {
            incBtn.onClick.AddListener(IncreaseAtt);
            decBtn.onClick.AddListener(DecreaseAtt);
        }

        private void OnDisable()
        {
            incBtn.onClick.RemoveListener(IncreaseAtt);
            decBtn.onClick.RemoveListener(DecreaseAtt);
        }

        void Start ()
        {
            mainText.text = $"{inserter.gameObject.name} Att";
            attText.text = inserter.Attuniation.ToString();
        }

        void IncreaseAtt()
        {
            inserter.Attuniation += increaseRate;
            inserter.SetAtt();
            attText.text = inserter.Attuniation.ToString();
        }

        void DecreaseAtt()
        {
            inserter.Attuniation -= increaseRate;
            inserter.SetAtt();

            attText.text = inserter.Attuniation.ToString();
        }
    }
}