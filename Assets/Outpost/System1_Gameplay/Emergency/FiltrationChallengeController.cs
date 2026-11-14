using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;
using Outpost.Core.Interfaces;
using Outpost.System1.Gameplay.Interactions;

namespace Outpost.System1.Gameplay.Emergency
{
    [Serializable]
    public class FiltrationComponentItem
    {
        public string Id;
        public string Name;
        public string Description;
        public bool IsRequired;
        public bool IsSelected;

        public FiltrationComponentItem(string id, string name, string description, bool isRequired)
        {
            Id = id;
            Name = name;
            Description = description;
            IsRequired = isRequired;
            IsSelected = false;
        }
    }

    /// <summary>
    /// Implements the fictional Apollo 13-inspired Emergency Filtration Challenge.
    /// Requires assembling a square LiOH canister into a round scrubber intake using
    /// cardboard manuals, suit hoses, and duct tape.
    /// </summary>
    public class FiltrationChallengeController : InteractableBase
    {
        [Header("Diagnostic Information")]
        [TextArea(3, 6)]
        [SerializeField] private string diagnosticReport =
            "DIAGNOSTIC FAULT: Primary habitat CO2 scrubber canister breached. " +
            "Storage only contains square Command Module LiOH canisters, which cannot lock into the habitat's round socket (15cm). " +
            "Airflow bypass is causing toxic carbon dioxide accumulation! " +
            "Crew must improvise an airtight funnel adapter using available cabin equipment.";

        [Header("Component Inventory")]
        [SerializeField] private List<FiltrationComponentItem> availableComponents = new List<FiltrationComponentItem>();

        public bool IsChallengeResolved { get; private set; } = false;
        public string DiagnosticReport => diagnosticReport;

        public event Action<bool, string> OnSolutionSubmitted;

        public static FiltrationChallengeController Instance { get; private set; }

        public override string PromptText => "Scrubber Intake: Press [E] to Inspect Filtration Challenge";
        public string ContextDescription => IsChallengeResolved ? "Status: Emergency Adapter Active (CO2 Scrubbing)" : "Status: CRITICAL AIR LEAK (Canister Mismatch)";

        protected override void Awake()
        {
            base.Awake();
            if (Instance == null) Instance = this;
            else if (Instance != this)
            {
                if (Application.isPlaying) Destroy(gameObject);
                else DestroyImmediate(gameObject);
                return;
            }

            objectName = "CO2 Scrubber Console";
            promptAction = "Inspect Challenge";

            InitializeComponents();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void InitializeComponents()
        {
            availableComponents.Clear();

            // Compatible required components
            availableComponents.Add(new FiltrationComponentItem("canister", "Square LiOH Canister", "Chemical CO2 absorption filter containing lithium hydroxide pellets.", true));
            availableComponents.Add(new FiltrationComponentItem("cardboard", "Flight Manual Cardboard Cover", "Rigid laminated cardboard rolled into a square-to-round geometric funnel.", true));
            availableComponents.Add(new FiltrationComponentItem("hose", "Suit Umbilical Hose", "Flexible airtight breathing tube channeling high-velocity blower air.", true));
            availableComponents.Add(new FiltrationComponentItem("tape", "Heavy-Duty Space Duct Tape", "Pressure-rated grey adhesive for hermetic sealing of cardboard seams.", true));

            // Incompatible red herring components (educational challenge)
            availableComponents.Add(new FiltrationComponentItem("copper_wire", "Spool of Copper Wire", "Electrical conductor. Cannot provide an airtight pneumatic seal.", false));
            availableComponents.Add(new FiltrationComponentItem("glass_pipette", "Glass Chemical Pipette", "Fragile laboratory tool. Incompatible with air filtration pressure.", false));
            availableComponents.Add(new FiltrationComponentItem("regolith_sample", "Lunar Regolith Sample", "Abrasive moon dust. Would contaminate the air blower loop.", false));
        }

        public void EnsureInitialized()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            if (availableComponents == null || availableComponents.Count == 0)
            {
                InitializeComponents();
            }
        }

        public string InspectProblem()
        {
            EnsureInitialized();
            OutpostEventChannel.RaisePlaySfx("terminal_beep");
            return diagnosticReport;
        }

        public IReadOnlyList<FiltrationComponentItem> GetAvailableComponents()
        {
            EnsureInitialized();
            return availableComponents;
        }

        public bool ToggleComponent(string componentId)
        {
            var item = availableComponents.Find(c => c.Id.Equals(componentId, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                item.IsSelected = !item.IsSelected;
                OutpostEventChannel.RaisePlaySfx("ui_click");
                return item.IsSelected;
            }
            return false;
        }

        public void SelectComponent(string componentId, bool selected)
        {
            var item = availableComponents.Find(c => c.Id.Equals(componentId, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                item.IsSelected = selected;
            }
        }

        public bool IsComponentSelected(string componentId)
        {
            var item = availableComponents.Find(c => c.Id.Equals(componentId, StringComparison.OrdinalIgnoreCase));
            return item != null && item.IsSelected;
        }

        public bool SubmitSolution(out string feedbackMessage)
        {
            bool hasCanister = IsComponentSelected("canister");
            bool hasCardboard = IsComponentSelected("cardboard");
            bool hasHose = IsComponentSelected("hose");
            bool hasTape = IsComponentSelected("tape");

            bool hasWire = IsComponentSelected("copper_wire");
            bool hasPipette = IsComponentSelected("glass_pipette");
            bool hasRegolith = IsComponentSelected("regolith_sample");

            if (hasWire || hasPipette || hasRegolith)
            {
                feedbackMessage = "INCOMPATIBLE PARTS: Foreign laboratory tools or abrasive regolith cannot be used in a life-support airflow circuit!";
                OutpostEventChannel.RaiseFiltrationFeedbackPrompted(feedbackMessage, false);
                OutpostEventChannel.RaisePlaySfx("alarm_siren");
                OnSolutionSubmitted?.Invoke(false, feedbackMessage);
                return false;
            }

            if (!hasCanister)
            {
                feedbackMessage = "MISSING FILTER: You need the Square LiOH Canister to chemically absorb carbon dioxide!";
                OutpostEventChannel.RaiseFiltrationFeedbackPrompted(feedbackMessage, false);
                OutpostEventChannel.RaisePlaySfx("alarm_siren");
                OnSolutionSubmitted?.Invoke(false, feedbackMessage);
                return false;
            }

            if (!hasCardboard)
            {
                feedbackMessage = "GEOMETRIC MISMATCH: A square canister cannot mate to a round hole without the Cardboard Manual funnel!";
                OutpostEventChannel.RaiseFiltrationFeedbackPrompted(feedbackMessage, false);
                OutpostEventChannel.RaisePlaySfx("alarm_siren");
                OnSolutionSubmitted?.Invoke(false, feedbackMessage);
                return false;
            }

            if (!hasHose)
            {
                feedbackMessage = "NO AIRFLOW: The blower intake requires the flexible Suit Umbilical Hose to direct cabin air through the adapter!";
                OutpostEventChannel.RaiseFiltrationFeedbackPrompted(feedbackMessage, false);
                OutpostEventChannel.RaisePlaySfx("alarm_siren");
                OnSolutionSubmitted?.Invoke(false, feedbackMessage);
                return false;
            }

            if (!hasTape)
            {
                feedbackMessage = "AIR LEAK: Cardboard seams are leaking carbon dioxide! Wrap and seal all edges with Space Duct Tape!";
                OutpostEventChannel.RaiseFiltrationFeedbackPrompted(feedbackMessage, false);
                OutpostEventChannel.RaisePlaySfx("alarm_siren");
                OnSolutionSubmitted?.Invoke(false, feedbackMessage);
                return false;
            }

            // All 4 compatible components selected cleanly!
            IsChallengeResolved = true;
            feedbackMessage = "SUCCESS: Airtight emergency filtration adapter fitted! Blower intake is pulling cabin air smoothly through the LiOH canister. CO2 levels stabilizing!";

            OutpostEventChannel.RaiseFiltrationFeedbackPrompted(feedbackMessage, true);
            OutpostEventChannel.RaiseFiltrationChallengeCompleted(true);
            OutpostEventChannel.RaisePlaySfx("victory_chime");

            Resources.ResourceManager.Instance?.RepairFiltrationCanister();
            OnSolutionSubmitted?.Invoke(true, feedbackMessage);
            return true;
        }

        public void ResetPuzzle()
        {
            IsChallengeResolved = false;
            for (int i = 0; i < availableComponents.Count; i++)
            {
                availableComponents[i].IsSelected = false;
            }
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            OutpostEventChannel.RaiseFiltrationChallengeOpened();
            OutpostEventChannel.RaisePlaySfx("terminal_beep");
        }
    }
}
