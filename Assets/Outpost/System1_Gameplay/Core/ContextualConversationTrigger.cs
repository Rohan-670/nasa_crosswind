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
    /// Conversations are triggered exclusively by actual gameplay events.
    /// </summary>
    public class ContextualConversationTrigger : MonoBehaviour
    {
        private readonly HashSet<string> triggeredConversations = new HashSet<string>();

        public static ContextualConversationTrigger Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this)
            {
                if (Application.isPlaying) Destroy(gameObject);
                else DestroyImmediate(gameObject);
                return;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void EnsureInitialized()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            OutpostEventChannel.EquipmentStateChanged += HandleEquipmentStateChanged;
            OutpostEventChannel.LocationArrived += HandleLocationArrived;
            OutpostEventChannel.ChapterChanged += HandleChapterChanged;
            OutpostEventChannel.StrategicChoiceOpened += HandleStrategicChoiceOpened;
            OutpostEventChannel.ResourceAlertTriggered += HandleResourceAlertTriggered;
            OutpostEventChannel.EmergencyStateChanged += HandleEmergencyStateChanged;
        }

        private void OnDisable()
        {
            OutpostEventChannel.EquipmentStateChanged -= HandleEquipmentStateChanged;
            OutpostEventChannel.LocationArrived -= HandleLocationArrived;
            OutpostEventChannel.ChapterChanged -= HandleChapterChanged;
            OutpostEventChannel.StrategicChoiceOpened -= HandleStrategicChoiceOpened;
            OutpostEventChannel.ResourceAlertTriggered -= HandleResourceAlertTriggered;
            OutpostEventChannel.EmergencyStateChanged -= HandleEmergencyStateChanged;
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

        private void HandleChapterChanged(MissionChapterId chapter, string title, string obj)
        {
            if (chapter == MissionChapterId.Chapter3_TheFirstWarning)
            {
                TriggerScientificReportConversation();
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

        private void HandleStrategicChoiceOpened()
        {
            TriggerRepairCoordinationConversation();
        }

        private void HandleResourceAlertTriggered(ResourceCategory cat, ResourceAlertLevel lvl, string msg)
        {
            if (cat == ResourceCategory.Oxygen && lvl >= ResourceAlertLevel.Warning)
            {
                TriggerHealthWarningConversation();
            }
        }

        private void HandleEmergencyStateChanged(EmergencyModel emergency)
        {
            if (emergency != null && emergency.Type == EmergencyType.SolarPanelDamage && emergency.IsActive)
            {
                TriggerEvaApprovalConversation();
            }
        }

        #region Contextual Conversation Definitions & Dispatch

        /// <summary>
        /// Phase 2 Example 1: Crew discussion when solar panels are activated.
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
        /// Phase 2 Example 2: Elena asks Marcus about electrical output.
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
        /// Phase 2 Example 3: Aisha explains the greenhouse's scientific purpose.
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
        /// Phase 2 Example 4: Daniel explains the importance of water availability.
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

        /// <summary>
        /// Phase 3 Example 1: Marcus requests Elena's approval before an external repair.
        /// </summary>
        public void TriggerEvaApprovalConversation(bool force = false)
        {
            const string id = "conv_eva_approval";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "EVA Repair Authorization");
            seq.AddLine(CharacterId.Marcus, "Commander, I've pre-breathed pure O2 and suited up. If I go out on EVA, I can manually splice the high-voltage relay in forty minutes.");
            seq.AddLine(CharacterId.Elena, "Marcus, radiation monitors are still elevated from the coronal flare. An external spacewalk carries genuine radiation exposure.");
            seq.AddLine(CharacterId.Marcus, "I know the risks, Elena. But if we don't restore the primary array, our life-support batteries will drain completely. I need your green light.");
            seq.AddLine(CharacterId.Elena, "Understood, Marcus. Review the radiation threshold with Daniel first. If dosimeter readings cross five millisieverts, abort immediately.");

            DispatchConversation(seq);
        }

        /// <summary>
        /// Phase 3 Example 2: Aisha reports scientific observations.
        /// </summary>
        public void TriggerScientificReportConversation(bool force = false)
        {
            const string id = "conv_aisha_science_report";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Solar Flare Spectrometry");
            seq.AddLine(CharacterId.Aisha, "Commander Elena, our Shackleton spectrometer readings just registered an anomalous flare in solar proton flux!");
            seq.AddLine(CharacterId.Elena, "Is the coronal mass ejection heading directly toward our coordinates, Aisha?");
            seq.AddLine(CharacterId.Aisha, "Yes. The solar wind velocity is over 600 kilometers per second. We have about fifteen minutes before secondary micrometeoroids and electromagnetic interference hit our ridge.");
            seq.AddLine(CharacterId.Daniel, "Crew radiation dosimeters are calibrated. Everyone inside the central habitat module now.");

            DispatchConversation(seq);
        }

        /// <summary>
        /// Phase 3 Example 3: Daniel warns the crew about health risks.
        /// </summary>
        public void TriggerHealthWarningConversation(bool force = false)
        {
            const string id = "conv_daniel_health_warning";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Hypercapnia Medical Alert");
            seq.AddLine(CharacterId.Daniel, "Attention crew: habitat atmospheric readings are deteriorating! Carbon dioxide has passed 700 parts per million.");
            seq.AddLine(CharacterId.Elena, "Daniel, what are the physiological thresholds?");
            seq.AddLine(CharacterId.Daniel, "At 1,000 ppm, we'll experience headaches, fatigue, and impaired decision-making. Above 1,500 ppm, hypercapnia causes dizziness and loss of consciousness. We must bring the scrubbers back online immediately!");
            seq.AddLine(CharacterId.Marcus, "Aisha and I are pulling spare parts from storage now, Doc. Hang tight!");

            DispatchConversation(seq);
        }

        /// <summary>
        /// Phase 3 Example 4: Elena coordinates the repair.
        /// </summary>
        public void TriggerRepairCoordinationConversation(bool force = false)
        {
            const string id = "conv_elena_repair_coordination";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Strategic Repair Briefing");
            seq.AddLine(CharacterId.Elena, "Team, we have three distinct pathways to restore outpost power. Option A: Marcus performs an external EVA repair. Option B: We deploy the rover's robotic arm to repair the relay remotely. Option C: We shed non-essential loads, conserve battery reserves, and wait out the solar storm.");
            seq.AddLine(CharacterId.Aisha, "The robotic option protects the crew from radiation, but the rover's manipulator arm has limited dexterity.");
            seq.AddLine(CharacterId.Daniel, "Conserving power avoids all EVA risks, but our life-support margins will be razor-thin.");
            seq.AddLine(CharacterId.Elena, "Every option has consequences. Player, evaluate our resources and choose our strategy.");

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
                case "conv_eva_approval":
                    TriggerEvaApprovalConversation(force);
                    break;
                case "conv_aisha_science_report":
                    TriggerScientificReportConversation(force);
                    break;
                case "conv_daniel_health_warning":
                    TriggerHealthWarningConversation(force);
                    break;
                case "conv_elena_repair_coordination":
                    TriggerRepairCoordinationConversation(force);
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
