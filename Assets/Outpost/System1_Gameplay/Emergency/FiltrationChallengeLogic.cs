using System;
using UnityEngine;

namespace Outpost.System1.Gameplay.Emergency
{
    [Serializable]
    public class FiltrationChallengeLogic
    {
        public bool HasCanister { get; private set; }
        public bool HasCardboardGuide { get; private set; }
        public bool HasSuitHose { get; private set; }
        public bool HasDuctTape { get; private set; }

        public bool IsAdapterCompleted => HasCanister && HasCardboardGuide && HasSuitHose && HasDuctTape;

        public void Reset()
        {
            HasCanister = false;
            HasCardboardGuide = false;
            HasSuitHose = false;
            HasDuctTape = false;
        }

        public bool TogglePart(string partName)
        {
            switch (partName.ToLowerInvariant())
            {
                case "canister":
                    HasCanister = !HasCanister;
                    return HasCanister;
                case "cardboard":
                    HasCardboardGuide = !HasCardboardGuide;
                    return HasCardboardGuide;
                case "hose":
                    HasSuitHose = !HasSuitHose;
                    return HasSuitHose;
                case "tape":
                    HasDuctTape = !HasDuctTape;
                    return HasDuctTape;
                default:
                    return false;
            }
        }
    }
}
