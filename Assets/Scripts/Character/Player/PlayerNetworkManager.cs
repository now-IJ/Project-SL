using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace RS
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        public NetworkVariable<FixedString64Bytes> characterName =
            new NetworkVariable<FixedString64Bytes>("Character", 
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Owner);
    }
}