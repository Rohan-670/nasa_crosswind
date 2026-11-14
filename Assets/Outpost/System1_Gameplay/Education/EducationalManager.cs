using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Education
{
    public class EducationalManager : MonoBehaviour, IEducationalService
    {
        private Dictionary<EducationalTopic, EducationalFact> facts = new Dictionary<EducationalTopic, EducationalFact>();

        public event Action<EducationalFact> OnFactPresented;

        public static EducationalManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializeFactDatabase();
        }

        private void OnEnable()
        {
            OutpostEventChannel.ChapterChanged += HandleChapterChanged;
        }

        private void OnDisable()
        {
            OutpostEventChannel.ChapterChanged -= HandleChapterChanged;
        }

        private void InitializeFactDatabase()
        {
            EducationalFact[] list = EducationalFact.GetEducationalDatabase();
            foreach (EducationalFact f in list)
            {
                facts[f.Topic] = f;
            }
        }

        private void HandleChapterChanged(MissionChapterId id, string title, string obj)
        {
            switch (id)
            {
                case MissionChapterId.Chapter1_ArrivalOnTheMoon:
                    TriggerFactPopup(EducationalTopic.LunarGravity);
                    break;
                case MissionChapterId.Chapter2_EstablishingTheOutpost:
                    TriggerFactPopup(EducationalTopic.SolarPower);
                    break;
                case MissionChapterId.Chapter3_TheFirstWarning:
                    TriggerFactPopup(EducationalTopic.ResourceConservation);
                    break;
                case MissionChapterId.Chapter4_TheLifeSupportEmergency:
                    TriggerFactPopup(EducationalTopic.CarbonDioxideRemoval);
                    break;
                case MissionChapterId.Chapter5_TheFiltrationChallenge:
                    TriggerFactPopup(EducationalTopic.EmergencyEngineering);
                    break;
                case MissionChapterId.Chapter7_ANewBeginning:
                    TriggerFactPopup(EducationalTopic.LunarGeology);
                    break;
            }
        }

        public EducationalFact GetFact(EducationalTopic topic)
        {
            facts.TryGetValue(topic, out EducationalFact fact);
            return fact;
        }

        public void TriggerFactPopup(EducationalTopic topic)
        {
            if (facts.TryGetValue(topic, out EducationalFact fact))
            {
                OnFactPresented?.Invoke(fact);
                OutpostEventChannel.RaiseEducationalFactPrompted(fact);
                OutpostEventChannel.RaisePlaySfx("info_chime");
            }
        }
    }
}
