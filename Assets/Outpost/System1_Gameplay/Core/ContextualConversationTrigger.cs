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
    /// Prepares and dispatches contextual gameplay conversations between astronauts:
    /// Commander Elena, Engineer Marcus, Scientist Aisha, Medical Officer Daniel, and AI Assistant ORION.
    /// Emits ConversationSequence events to Computer 3's DialogueUI without rendering any visual UI.
    /// Conversations are triggered exclusively by actual gameplay milestones across Chapters 1 to 7.
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
            OutpostEventChannel.MissionCompleted += HandleMissionCompleted;
        }

        private void OnDisable()
        {
            OutpostEventChannel.EquipmentStateChanged -= HandleEquipmentStateChanged;
            OutpostEventChannel.LocationArrived -= HandleLocationArrived;
            OutpostEventChannel.ChapterChanged -= HandleChapterChanged;
            OutpostEventChannel.StrategicChoiceOpened -= HandleStrategicChoiceOpened;
            OutpostEventChannel.ResourceAlertTriggered -= HandleResourceAlertTriggered;
            OutpostEventChannel.EmergencyStateChanged -= HandleEmergencyStateChanged;
            OutpostEventChannel.MissionCompleted -= HandleMissionCompleted;
        }

        private void HandleLocationArrived(StationLocationId locationId)
        {
            switch (locationId)
            {
                case StationLocationId.MainHabitat:
                    TriggerOrionIntroConversation();
                    break;
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
            switch (chapter)
            {
                case MissionChapterId.Chapter1_Arrival:
                    TriggerArrivalBriefingConversation();
                    break;
                case MissionChapterId.Chapter3_FirstWarning:
                    TriggerScientificReportConversation();
                    break;
                case MissionChapterId.Chapter6_Recovery:
                    TriggerRecoveryStatusConversation();
                    break;
                case MissionChapterId.Chapter7_Ending:
                    TriggerMissionEndingConversation();
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

        private void HandleMissionCompleted(string _)
        {
            TriggerMissionEndingConversation(true);
        }

        #region Contextual Conversation Definitions & Dispatch

        /// <summary>
        /// Chapter 1: Crew arrival briefing at touchdown.
        /// </summary>
        public void TriggerArrivalBriefingConversation(bool force = false)
        {
            const string id = "conv_arrival_briefing";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Arrival on the Moon");
            seq.AddLine(CharacterId.Marcus, "Lander thrusters cooled down nicely, Elena. Landing gear shock absorbers handled the 1.6 m/s² touchdown perfectly.");
            seq.AddLine(CharacterId.Elena, "Good work Marcus. Daniel, how is Aisha adapting to the low-gravity transit?");
            seq.AddLine(CharacterId.Daniel, "Vitals are exceptional, Commander. Aisha was already calibrating her spectrometer before the dust even settled!");
            seq.AddLine(CharacterId.Aisha, "I can see the rim of Shackleton Crater from here! Billions of years of frozen history right in front of us.");

            DispatchConversation(seq);
        }

        /// <summary>
        /// Chapter 1: ORION AI Assistant introduction upon entering habitat.
        /// </summary>
        public void TriggerOrionIntroConversation(bool force = false)
        {
            const string id = "conv_orion_intro";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "O.R.I.O.N. Initialization");
            seq.AddLine(CharacterId.Orion, "Greetings, Commander Elena and crew. Onboard Robotic Intelligence and Operations Network is now fully active.");
            seq.AddLine(CharacterId.Elena, "ORION, report status of our central life-support bus and navigation sensors.");
            seq.AddLine(CharacterId.Orion, "All sensors nominal. Press [M] at any time to open the Outpost Geo Map. I am programmed to monitor environmental safety, navigation waypoints, and life-support stability.");
            seq.AddLine(CharacterId.Marcus, "Nice to have you watching our backs, ORION. Let's get this outpost operational!");

            DispatchConversation(seq);
        }

        /// <summary>
        /// Chapter 2: Crew discussion when solar panels are activated.
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
        /// Chapter 2: Elena asks Marcus about electrical output.
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
        /// Chapter 2: Aisha explains the greenhouse's scientific purpose.
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
        /// Chapter 2: Daniel explains the importance of water availability.
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
        /// Chapter 3: Aisha reports scientific observations of solar flare.
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
        /// Chapter 4: Daniel warns the crew about hypercapnia health risks.
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
        /// Chapter 5: Elena coordinates the repair choices with crew.
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

        /// <summary>
        /// Chapter 5: Marcus requests Elena's approval before external EVA repair.
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
        /// Chapter 6: Crew recovery and equipment restoration review.
        /// </summary>
        public void TriggerRecoveryStatusConversation(bool force = false)
        {
            const string id = "conv_recovery_status";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Outpost Stabilization");
            seq.AddLine(CharacterId.Elena, "All hands: primary power grid is stable. Marcus, begin powering on secondary sub-buses.");
            seq.AddLine(CharacterId.Marcus, "Substation breakers reset cleanly. Hydroponic grow lights are illuminated, and thermal radiator valves are cycling at nominal pressure.");
            seq.AddLine(CharacterId.Aisha, "Spectrometer sensors report zero radiation leakage inside the habitat shell. The regolith shielding held up beautifully.");
            seq.AddLine(CharacterId.Daniel, "Crew vitals are normalizing across the board. Oxygen is steady at 21%, and carbon dioxide is dropping back down to pristine baseline levels.");

            DispatchConversation(seq);
        }

        /// <summary>
        /// Chapter 7: Ending debriefing and Lunar Pioneer celebration.
        /// </summary>
        public void TriggerMissionEndingConversation(bool force = false)
        {
            const string id = "conv_mission_ending";
            if (!force && triggeredConversations.Contains(id)) return;
            triggeredConversations.Add(id);

            ConversationSequence seq = new ConversationSequence(id, "Mission 1 Completion");
            seq.AddLine(CharacterId.Elena, "Look at that Earthrise through the cupola window, crew. Outpost Alpha is running at full capacity, self-sustaining and secure.");
            seq.AddLine(CharacterId.Marcus, "Solar arrays tracking the ridge sun, scrubbers humming, and rover batteries fully topped off.");
            seq.AddLine(CharacterId.Aisha, "Now we can finally begin our scientific exploration of the ancient polar ice in Shackleton Crater!");
            seq.AddLine(CharacterId.Daniel, "Crew health is 100%. We didn't just survive an emergency—we demonstrated how humanity can live, work, and thrive beyond Earth.");
            seq.AddLine(CharacterId.Orion, "Outpost Alpha operational readiness: 100%. Commendation awarded: Lunar Pioneer Ribbon. Congratulations, astronauts.");

            DispatchConversation(seq);
        }

        public void TriggerConversationById(string conversationId, bool force = false)
        {
            switch (conversationId)
            {
                case "conv_arrival_briefing":
                    TriggerArrivalBriefingConversation(force);
                    break;
                case "conv_orion_intro":
                    TriggerOrionIntroConversation(force);
                    break;
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
                case "conv_recovery_status":
                    TriggerRecoveryStatusConversation(force);
                    break;
                case "conv_mission_ending":
                    TriggerMissionEndingConversation(force);
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
