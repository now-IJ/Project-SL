using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace RS
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        private CharacterManager character;
        
        [Header("Position")]
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>
            (Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>
            (Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.1f;
        public float networkRotationSmoothTime = 0.1f;

        [Header("Animator")] 
        public NetworkVariable<float> horizontalMovement =
            new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [FormerlySerializedAs("animatorVerticalParameter")] public NetworkVariable<float> verticalMovement =
            new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        [FormerlySerializedAs("moveAmountParameter")] public NetworkVariable<float> moveAmount =
            new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        [ServerRpc]
        public void NotifyServerOfActionAnimationServerRPC(ulong ClientID, string animationID, bool applyRootMotion)
        {
            if (IsServer)
            {
                PlayActionAnimationOnAllClientsClientRPC(ClientID, animationID, applyRootMotion);
            }
        }

        [ClientRpc]
        public void PlayActionAnimationOnAllClientsClientRPC(ulong ClientID, string animationID, bool applyRootMotion)
        {
            if (ClientID != NetworkManager.Singleton.LocalClientId)
            {PerformActionAnimationFromServer(animationID, applyRootMotion);
                
            }
        }

        private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }
    }
}