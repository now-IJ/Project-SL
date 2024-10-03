using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RS
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        private CharacterManager character;

        private int vertical;
        private int horizontal;

        [Header("Damage Animations")] 
        public string lastDamageAnimationPlayed;
        
        [SerializeField] private string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
        [SerializeField] private string hit_Forward_Medium_02 = "hit_Forward_Medium_02";
        
        [SerializeField] private string hit_Back_Medium_01 = "hit_Back_Medium_01";
        [SerializeField] private string hit_Back_Medium_02 = "hit_Back_Medium_02";
        
        [SerializeField] private string hit_Left_Medium_01 = "hit_Left_Medium_01";
        [SerializeField] private string hit_Left_Medium_02 = "hit_Left_Medium_02";
        
        [SerializeField] private string hit_Right_Medium_01 = "hit_Right_Medium_01";
        [SerializeField] private string hit_Right_Medium_02 = "hit_Right_Medium_02";

        public List<string> forward_Medium_Damage = new List<string>();
        public List<string> back_Medium_Damage = new List<string>();
        public List<string> left_Medium_Damage = new List<string>();
        public List<string> right_Medium_Damage = new List<string>();
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        protected virtual void Start()
        {
            forward_Medium_Damage.Add(hit_Forward_Medium_01);
            forward_Medium_Damage.Add(hit_Forward_Medium_02);
            
            back_Medium_Damage.Add(hit_Back_Medium_01);
            back_Medium_Damage.Add(hit_Back_Medium_02);
            
            left_Medium_Damage.Add(hit_Left_Medium_01);
            left_Medium_Damage.Add(hit_Left_Medium_02);
            
            right_Medium_Damage.Add(hit_Right_Medium_01);
            right_Medium_Damage.Add(hit_Right_Medium_02);
        }

        public string GetRandomAnimationFromList(List<string> animationList)
        {
            List<string> finalList = new List<string>();

            foreach (string item in animationList)
            {
                finalList.Add(item);
            }

            // Check if animation was played before, stops repeating animations
            finalList.Remove(lastDamageAnimationPlayed);

            // Check list for null entries and removes them
            for (int i = finalList.Count - 1; i > -1; i--)
            {
                if (finalList[i] == null)
                {
                    finalList.RemoveAt(i);
                }
            }
            
            int randomValue = Random.Range(0, finalList.Count);

            return finalList[randomValue];
        }
        
        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
        {
            float horizontalAmount = horizontalValue;
            float verticalAmount = verticalValue;
            if (isSprinting)
            {
                verticalAmount = 2;
            }
            character.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
            character.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
            
        }

        public virtual void PlayTargetActionAnimation(
            string targetAnimation, 
            bool isPerformingAction,
            bool applyRootMotion = true, 
            bool canRotate = false, 
            bool canMove = false)
        {
            Debug.Log("PLAYING ANIMATION: " + targetAnimation);
            
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.isPerformingAction = isPerformingAction;
            character.applyRootMotion = applyRootMotion;
            character.canRotate = canRotate;
            character.canMove = canMove;

            character.characterNetworkManager.NotifyServerOfActionAnimationServerRPC
            (NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }

        public virtual void PlayTargetAttackActionAnimation(
            AttackType attackType, 
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true, 
            bool canRotate = false, 
            bool canMove = false)
        {
            character.characterCombatManager.currentAttackType = attackType;
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.isPerformingAction = isPerformingAction;
            character.applyRootMotion = applyRootMotion;
            character.canRotate = canRotate;
            character.canMove = canMove;

            character.characterNetworkManager.NotifyServerOfAttackActionAnimationServerRPC
                (NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
    }
}