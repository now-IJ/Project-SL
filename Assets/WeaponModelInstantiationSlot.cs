using Unity.Mathematics;
using UnityEngine;

namespace RS
{
    public class WeaponModelInstantiationSlot : MonoBehaviour
    {
        public WeaponModelSlot weaponSlot;
        public GameObject currentWeaponModel;

        public void UnloadWeapon()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeapon(GameObject weaponModel)
        {
            currentWeaponModel = weaponModel;
            weaponModel.transform.parent = transform;
            
            weaponModel.transform.localPosition = Vector3.zero;
            weaponModel.transform.localRotation = quaternion.identity;
            weaponModel.transform.localScale = Vector3.one;
        }
    }
}