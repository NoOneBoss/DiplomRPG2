using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class CombatSystem : NetworkBehaviour
    {
        private Controls _controls;
        public Animator _animator;
        
        private bool isPullingWeapon = false;
        private int currentWeapon = 0;
    }
}