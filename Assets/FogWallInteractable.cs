using NUnit.Framework.Internal;
using UnityEngine;
using Unity.Netcode;

namespace RS
{
    public class FogWallInteractable : NetworkBehaviour
    {
        [Header("Fog")] 
        [SerializeField] private GameObject[] fogGameObjects;

        [Header("ID")] 
        public int fogWallID;
        
        [Header("Active")]
        public NetworkVariable<bool> isActive = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            OnIsActiveChanged(false, isActive.Value);
            isActive.OnValueChanged += OnIsActiveChanged;
            WorldObjectManager.instance.AddFogWallToList(this);
        }
    
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            isActive.OnValueChanged -= OnIsActiveChanged;
        }

        private void OnIsActiveChanged(bool oldStatus, bool newStatus)
        {
            if (isActive.Value)
            {
                foreach (GameObject fogObject in fogGameObjects)
                {
                    fogObject.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject fogObject in fogGameObjects)
                {
                    fogObject.SetActive(false);
                }
            }
        }
        
    }
}