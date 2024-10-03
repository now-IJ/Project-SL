using System;
using UnityEngine;

namespace RS
{
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float timeUntilDestroy = 5;

        private void Awake()
        {
            Destroy(gameObject, timeUntilDestroy);
        }
    }
}