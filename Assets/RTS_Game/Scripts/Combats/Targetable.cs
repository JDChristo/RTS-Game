using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace RTS.Combat
{
    public class Targetable : NetworkBehaviour
    {

        [SerializeField]
        private Transform aimPoint;

        public Transform AimPoint => aimPoint;
    }
}
