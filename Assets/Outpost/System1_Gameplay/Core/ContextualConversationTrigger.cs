using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;
using Outpost.Core.Interfaces;
using Outpost.System1.Gameplay.Equipment;

namespace Outpost.System1.Gameplay.Core
{
    /// <summary>
    /// Prepares and dispatches contextual gameplay conversations between astronauts.
    /// Emits ConversationSequence events to Computer 3's DialogueUI without rendering any visual UI.
    /// </summary>
    public class ContextualConversationTrigger : MonoBehaviour
    {
        private readonly HashSet<string> triggeredConversations = new HashSet<string>();

        public static ContextualConversationTrigger Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            OutpostEventChannel.EquipmentStateChanged += HandleEquipmentStateChanged;
            OutpostEventChannel.LocationArrived += HandleLocationArrived;
        }

        private void OnDisable()
        {
            OutpostEventChannel.EquipmentStateChanged -= HandleEquipmentStateChanged;
            OutpostEventChannel.LocationArrived -= HandleLocationArrived;
        }

        private void HandleLocationArrived(StationLocationId locationId)
        {
            switch (locationId)
            {
                case StationLocationId.SolarFarm:
                    TriggerSolarActivationConversation();
                    break;
                case StationLocationId.Greenhouse:
                    TriggerGreenhouseScienceExplanation();
                    break;
                case StationLocationId.EquipmentStorage:
                    TriggerWaterImportanceDiscussion();
                    break;
            }
        }

        private void HandleEquipmentStateChanged(IStationEquipment equipment)
        {
            if (equipment == null) return;

            if (equipment is SolarArrayEquipment solar && solar.IsOperational && solar.CurrentGenerationKw > 40f)
            {
                TriggerElectricalOutputReport();
            }
        }

        #region Contextual Conversation Definitions & Dispatch

        /// <summary>
        /// Example 1: Crew discussion when solar panels are activated.
        /// </summary>
        public void TriggerSolarActivationConversation(bool force = false)
        {
            const string id = "conv_solar_activation";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Solar Array Deployment");
            seq.AddLine(CharacterId.Elena, "Solar array alignment complete! Marcus, are both photovoltaic wings tracking the low sun angle?");
            seq.AddLine(CharacterId.Marcus, "Locked on target, Commander! The lunar south pole ridge gives us near-permanent illumination. We're generating a steady 55 kW!");
            seq.AddLine(CharacterId.Orion, "Outpost power bus synchronized. Energy reserves are flowing to central life-support batteries.");

            DispatchConversation(seq);
        }

        /// <summary>
        /// Example 2: Elena asks Marcus about electrical output.
        /// </summary>
        public void TriggerElectricalOutputReport(bool force = false)
        {
            const string id = "conv_electrical_output";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Electrical Output Review");
            seq.AddLine(CharacterId.Elena, "Marcus, give me a status report on our electrical budget. Can the grid support running both the lab spectrometer and the greenhouse?");
            seq.AddLine(CharacterId.Marcus, "Base life support and thermal regulation draw about 18 kW. With solar generating 55 kW, we have plenty of headroom to run science experiments and bank surplus in the batteries!");
            seq.AddLine(CharacterId.Elena, "Good. Keep an eye on battery depth of discharge when night falls across the crater shadows.");

            DispatchConversation(seq);
        }

        /// <summary>
        /// Example 3: Aisha explains the greenhouse's scientific purpose.
        /// </summary>
        public void TriggerGreenhouseScienceExplanation(bool force = false)
        {
            const string id = "conv_greenhouse_science";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Biological Life Support");
            seq.AddLine(CharacterId.Aisha, "Commander, the hydroponic greenhouse is much more than food production. Closed-loop biological life support is essential for long-duration lunar stays.");
            seq.AddLine(CharacterId.Elena, "How are the dwarf wheat and salad crop chambers contributing to our atmosphere?");
            seq.AddLine(CharacterId.Aisha, "Through photosynthesis, the crops actively consume crew CO2 and produce fresh oxygen! Plus, fresh greens provide vital micronutrients and significant psychological benefits.");
            seq.AddLine(CharacterId.Marcus, "And nothing beats fresh crispy lettuce 384,000 kilometers from Earth!");

            DispatchConversation(seq);
        }

        /// <summary>
        /// Example 4: Daniel explains the importance of water availability.
        /// </summary>
        public void TriggerWaterImportanceDiscussion(bool force = false)
        {
            const string id = "conv_water_importance";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Water Loop Dynamics");
            seq.AddLine(CharacterId.Daniel, "Crew hydration check complete! In one-sixth gravity, our cardiovascular systems undergo fluid shifts, making adequate water intake critical to avoid kidney stones and blood pressure drops.");
            seq.AddLine(CharacterId.Marcus, "Good thing our closed-loop Water Recovery System recycles over 92% of all habitat moisture—from sweat condensation to hygiene greywater.");
            seq.AddLine(CharacterId.Daniel, "Exactly, Marcus. Every liter is precious. Without a functioning recycler, our 1,000-liter reserve would only sustain four astronauts for less than a month.");

            DispatchConversation(seq);
        }

        public void TriggerConversationById(string conversationId, bool force = false)
        {
            switch (conversationId)
            {
                case "conv_solar_activation":
                    TriggerSolarActivationConversation(force);
                    break;
                case "conv_electrical_output":
                    TriggerElectricalOutputReport(force);
                    break;
                case "conv_greenhouse_science":
                    TriggerGreenhouseScienceExplanation(force);
                    break;
                case "conv_water_importance":
                    TriggerWaterImportanceDiscussion(force);
                    break;
            }
        }

        private void DispatchConversation(ConversationSequence seq)
        {
            OutpostEventChannel.RaiseConversationSequencePrompted(seq);
        }

        #endregion
    }
}
