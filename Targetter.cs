using UnityEngine;
using System;
using RPG.Characters;
using RPG.AI;

#pragma warning disable 0649
namespace RPG.Control
{
    public sealed class Targetter : MonoBehaviour
    {
        /************************************************************************************************************************/
        #region Declaration
        public IMoveController TargetMoveController { get; private set; }
        public float TargetRadius { get; private set; }
        public Vector3 TargetPos { get => Target.transform.position; }
        public Vector3 TargetVelocity { get => TargetMoveController.Velocity; }
        public Character Target { get; private set; }
        public Character LastTarget { get; private set; }

        //interactibles
        public Interactible Interactible{ get; private set; }

        //Event
        public Action onTargetChanged;
        #endregion
        /************************************************************************************************************************/
        #region Targetting
        public void SetTarget(Character newTarget)
        {
            if(newTarget == null) { return; }

            LastTarget = Target;
	        Target = newTarget;
            TargetRadius = 0.5f;

            if (Tags.IsPlayer(newTarget.gameObject))
            {
                TargetRadius = newTarget.GetCached<CharacterController>().radius;
                TargetMoveController = newTarget.GetCached<PlayerMoveController>();
            }
            else
            {
                TargetRadius = newTarget.GetCached<CapsuleCollider>().radius;
                TargetMoveController = newTarget.GetCached<AIMoveController>();
            }

            onTargetChanged?.Invoke();
        }
        public void ClearTarget()
        {
            if(Target == null) { return; }
            LastTarget = Target;
	        Target = null;
	        TargetRadius = 0;

	        onTargetChanged?.Invoke();
        }
        #endregion
        /************************************************************************************************************************/
        #region Interactible
        /************************************************************************************************************************/
        public void Interact()
        {
            if(Interactible == null) { return; }
            Interactible.HandleInteraction(gameObject);
        }
        public bool TrySetInteractible(Interactible interactible)
        {
            if(Interactible != null && Interactible != interactible) { return false; }
            
            Interactible = interactible;
            return true;
        }
        public bool TryClearInteractible(Interactible interactible)
        {
            if(Interactible != interactible) { return false; }
            
            Interactible = null;
            return true;
        }
        public void ClearInteractible()
        {
            Interactible = null;
        }
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}