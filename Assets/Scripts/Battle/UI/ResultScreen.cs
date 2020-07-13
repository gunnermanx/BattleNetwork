using UnityEngine;
using UnityEngine.UI;

namespace BattleNetwork.Battle.UI
{
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField] private Text tempResultText;

        public void SetWon()
        {
            gameObject.SetActive(true);
            tempResultText.text = "Victory";
        }

        public void SetLost()
        {
            gameObject.SetActive(true);
            tempResultText.text = "Defeated";
        }
    }
}
