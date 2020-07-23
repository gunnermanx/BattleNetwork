using BattleNetwork.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace BattleNetwork.Battle.UI
{
    public class UnitHPDisplay : MonoBehaviour
    {

        [SerializeField] private Text hpLabel;
        private Vector3 offset = Vector2.zero;

        private Damageable cachedDamageable = null;
        private Transform cachedDamaegableTransform = null;


        public void AttachToIDamageable(Damageable target, Vector3 _offset)
        {
            cachedDamageable = target;
            cachedDamaegableTransform = target.gameObject.transform;
            cachedDamageable.damageTaken += HandleDamageTaken;

            offset = _offset;

            hpLabel.text = cachedDamageable.Current().ToString();
        }

        private void Update()
        {
            if (cachedDamageable != null)
            {
                Vector3 worldPos = cachedDamaegableTransform.position + offset;
                Vector3 position = Camera.main.WorldToScreenPoint(worldPos);
                transform.position = position;
            }
        }

        private void HandleDamageTaken(int amount, int remaining)
        {
            hpLabel.text = remaining.ToString();
        }
    }
}
