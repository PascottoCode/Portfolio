using System;
using RPG.Characters;
using RPG.Localization;
using RPG.UI;
using UnityEngine;

#pragma warning disable 0649

namespace RPG.Control
{
    public abstract class Interactible : MonoBehaviour
    {
        [SerializeField] protected InputBindNameDisplay activateText;
        protected bool canInteract = true;
        protected bool isActivated;
        protected bool PlayerInRange { get; private set; }

        private SphereCollider _sphereCollider;
        
        private static Action<Interactible> _OnExitStatic;
        private static Action<Interactible> _OnEnterStatic;
        protected virtual void Awake()
        {
            activateText.gameObject.SetActive(false);
            _sphereCollider = GetComponent<SphereCollider>();
        }
        private void OnEnable()
        {
            _OnExitStatic += TryEnter;
            _OnEnterStatic += TryEnter;
        }
        private void OnDisable()
        {
            _OnExitStatic -= TryEnter;
            _OnEnterStatic -= TryEnter;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Tags.IsPlayer(other.gameObject)) { return; }
            Enter();
        }
        private void OnTriggerExit(Collider other)
        {
            if (!Tags.IsPlayer(other.gameObject)) { return; }
            Exit();
        }        
        protected virtual bool Enter()
        {
            if (!canInteract) { return false; }

            var targetter = Player.Inst.GetCached<Targetter>();

            if (targetter.Interactible != null && targetter.Interactible != this && !targetter.Interactible.CheckDistance())
            {
                targetter.Interactible.Exit();
            }
            
            PlayerInRange = true;
                
            return targetter.TrySetInteractible(this);
        }
        protected virtual void Exit()
        {
            PlayerInRange = false;
            
            if (Player.Inst.TryGetCached<Targetter>(out var playerTargetter) && playerTargetter.TryClearInteractible(this))
            {
                _OnExitStatic?.Invoke(this);
            }
        }
        private void TryEnter(Interactible interactible)
        {
            if(interactible == this || !PlayerInRange) { return; }

            Enter();
        }
        private bool CheckDistance()
        {
            var largestSize = transform.localScale.x;
            if (transform.localScale.y > largestSize)
            {
                largestSize = transform.localScale.y;
            }
            if (transform.localScale.z > largestSize)
            {
                largestSize = transform.localScale.z;
            }

            var playerRadius = Player.Inst.GetCached<CharacterController>().radius;

            return MyMath.InRange(transform.position, Player.Inst.Pos, playerRadius + _sphereCollider.radius * largestSize);
        }

        protected void ResetActivateText()
        {
            SetActivateText(GameSession.Inst.Language == Language.English ? "Activate" : "Ativar");
        }

        protected void SetActivateText(string preText)
        {
            activateText.gameObject.SetActive(false);
            activateText.preText = $"{preText}: [";
            activateText.gameObject.SetActive(true);
        }

        public virtual bool HandleInteraction(GameObject interactor)
        {
            return CheckDistance();
        }
    }
}