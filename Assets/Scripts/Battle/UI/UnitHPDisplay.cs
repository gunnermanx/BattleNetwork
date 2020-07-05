using BattleNetwork.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace BattleNetwork.Battle.UI
{
    public class UnitHPDisplay : MonoBehaviour
    {

        [SerializeField]
        private Text hpLabel;

        private Damageable cachedDamageable = null;
        private Transform cachedDamaegableTransform = null;
        private Vector2 offset = Vector2.zero;

        public void AttachToIDamageable(Damageable target, Vector2 _offset)
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
                Vector3 position = Camera.main.WorldToScreenPoint(cachedDamaegableTransform.position);
                transform.position = position + new Vector3(offset.x, offset.y, 0f);
            }
        }

        private void HandleDamageTaken(int amount, int remaining)
        {
            hpLabel.text = remaining.ToString();
        }
    }
}
