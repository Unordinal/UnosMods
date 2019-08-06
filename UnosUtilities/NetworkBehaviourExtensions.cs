using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace UnosUtilities
{
    public static class NetworkBehaviourExtensions
    {
        /// <summary>
        /// Checks whether the specified interactable is completely used up.
        /// </summary>
        /// <param name="NB">The NetworkBehaviour to check.</param>
        /// <returns></returns>
        public static bool InteractableIsUsedUp(this NetworkBehaviour NB) // I _could_ put these all in one if statement... but why?
        {
            if (NB.IsShrine() && NB.IsShrineUsedUp())
                return true;
            else if (NB.IsContainer() && NB.IsContainerOpened())
                return true;
            else if (!NB.IsShrine() && (!NB.GetComponent<PurchaseInteraction>()?.available ?? false))
                return true;
            return false;
        }

        /// <summary>
        /// Checks whether the specified NetworkBehaviour is a Shrine.
        /// </summary>
        /// <param name="NB">The NetworkBehaviour to check.</param>
        /// <returns></returns>
        public static bool IsShrine(this NetworkBehaviour NB)
        {
            return 
                NB.GetComponent<ShrineChanceBehavior>()     ||
                NB.GetComponent<ShrineBloodBehavior>()      ||
                NB.GetComponent<ShrineBossBehavior>()       ||
                NB.GetComponent<ShrineCombatBehavior>()     ||
                NB.GetComponent<ShrineHealingBehavior>()    ||
                NB.GetComponent<ShrineRestackBehavior>();
        }

        /// <summary>
        /// Checks whether the NetworkBehaviour is a Shrine that has been completely used up.
        /// </summary>
        /// <param name="NB">The NetworkBehaviour to check.</param>
        /// <returns></returns>
        public static bool IsShrineUsedUp(this NetworkBehaviour NB)
        {
            if (NB.IsShrine()) // Since purchaseCount is private, we'll do it your way...
            {
                return
                    (!NB.GetComponent<ShrineChanceBehavior>()?.symbolTransform.gameObject.activeSelf ?? false) ||
                    (!NB.GetComponent<ShrineBloodBehavior>()?.symbolTransform.gameObject.activeSelf ?? false) ||
                    (!NB.GetComponent<ShrineBossBehavior>()?.symbolTransform.gameObject.activeSelf ?? false) ||
                    (!NB.GetComponent<ShrineCombatBehavior>()?.symbolTransform.gameObject.activeSelf ?? false) ||
                    (!NB.GetComponent<ShrineHealingBehavior>()?.symbolTransform.gameObject.activeSelf ?? false) ||
                    (!NB.GetComponent<ShrineRestackBehavior>()?.symbolTransform.gameObject.activeSelf ?? false);
            }
            else
            {
                Debug.LogWarning("NetworkBehaviour passed to IsShrineUsedUp() is not a valid shrine");
                return false;
            }
        }

        /// <summary>
        /// Gets if the NetworkBehaviour is a Chest or Barrel.
        /// </summary>
        /// <param name="NB">The NetworkBehaviour to check.</param>
        /// <returns></returns>
        public static bool IsContainer(this NetworkBehaviour NB)
        {
            return NB.GetComponent<ChestBehavior>() || NB.GetComponent<BarrelInteraction>();
        }

        /// <summary>
        /// Gets if the container has been opened.
        /// </summary>
        /// <param name="NB">The NetworkBehaviour to check.</param>
        /// <returns></returns>
        public static bool IsContainerOpened(this NetworkBehaviour NB)
        {
            if (NB.IsContainer())
                return (NB.GetComponent<BarrelInteraction>()?.Networkopened ?? false) || (!NB.GetComponent<PurchaseInteraction>()?.available ?? false);
            Debug.LogWarning("NetworkBehaviour passed to IsContainerOpened() is not a valid container");
            return false;
        }
    }
}
