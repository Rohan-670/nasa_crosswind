using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System3.Interface.Dialogue
{
    public class AutonomousCrewDialogue : MonoBehaviour
    {
        [Header("Autonomous Conversation Cadence")]
        [SerializeField] private float minIntervalSeconds = 45f;
        [SerializeField] private float maxIntervalSeconds = 90f;

        private float conversationTimer = 0f;
        private float nextConversationTarget = 30f;
        private MissionChapterId currentChapter = MissionChapterId.Chapter1_ArrivalOnTheMoon;

        private void Start()
        {
            nextConversationTarget = Random.Range(minIntervalSeconds, maxIntervalSeconds);
        }

        private void OnEnable()
        {
            OutpostEventChannel.ChapterChanged += HandleChapterChanged;
        }

        private void OnDisable()
        {
            OutpostEventChannel.ChapterChanged -= HandleChapterChanged;
        }

        private void HandleChapterChanged(MissionChapterId id, string title, string obj)
        {
            currentChapter = id;
            // Play an autonomous crew interaction tailored to the new chapter
            StartCoroutine(PlayChapterAutonomousDialogue(id));
        }

        private void Update()
        {
            conversationTimer += Time.deltaTime;
            if (conversationTimer >= nextConversationTarget)
            {
                conversationTimer = 0f;
                nextConversationTarget = Random.Range(minIntervalSeconds, maxIntervalSeconds);
                TriggerContextualCrewChatter();
            }
        }

        private IEnumerator PlayChapterAutonomousDialogue(MissionChapterId chapter)
        {
            yield return new WaitForSeconds(5.0f); // Wait briefly after chapter intro

            switch (chapter)
            {
                case MissionChapterId.Chapter1_ArrivalOnTheMoon:
                    PlaySequence(
                        new DialogueLine(CharacterId.Marcus, "Lander thrusters cooled down nicely, Elena. Landing gear shock absorbers absorbed the 1.6 m/s touchdown perfectly."),
                        new DialogueLine(CharacterId.Elena, "Good work Marcus. Daniel, how is Aisha adapting to the low-gravity transit?"),
                        new DialogueLine(CharacterId.Daniel, "Vitals are exceptional, Commander. Aisha was already calibrating her spectrometer before the dust settled!"),
                        new DialogueLine(CharacterId.Aisha, "I can see the rim of Shackleton Crater from here! Billions of years of frozen history right in front of us.")
                    );
                    break;

                case MissionChapterId.Chapter2_EstablishingTheOutpost:
                    PlaySequence(
                        new DialogueLine(CharacterId.Aisha, "Elena, spectrometer data from the solar ridge confirms an ideal albedo. We should expect 98% panel efficiency."),
                        new DialogueLine(CharacterId.Elena, "Excellent. Marcus, make sure the water recycling loop is synchronized with the hydroponic greenhouse."),
                        new DialogueLine(CharacterId.Marcus, "Already on it, Commander. Daniel just tested the drinking water filtration—cleaner than Earth spring water!"),
                        new DialogueLine(CharacterId.Daniel, "Can confirm! 98% recovery efficiency. Our closed loop is fully operational.")
                    );
                    break;

                case MissionChapterId.Chapter3_TheFirstWarning:
                    PlaySequence(
                        new DialogueLine(CharacterId.Marcus, "Elena, did you see the telemetry spike? Solar array current fluctuated by twelve percent."),
                        new DialogueLine(CharacterId.Elena, "ORION picked it up too, Marcus. A coronal mass ejection heading toward our orbital quadrant."),
                        new DialogueLine(CharacterId.Daniel, "Habitat radiation shielding is rated for solar storms, but our external equipment is exposed."),
                        new DialogueLine(CharacterId.Marcus, "I'll double check the relay circuit breakers before debris reaches our sector.")
                    );
                    break;

                case MissionChapterId.Chapter4_TheLifeSupportEmergency:
                    PlaySequence(
                        new DialogueLine(CharacterId.Marcus, "Elena, Relay Four just blew! Micrometeoroid punctured the secondary power conduit!"),
                        new DialogueLine(CharacterId.Elena, "Daniel, what's the habitat atmosphere reading?"),
                        new DialogueLine(CharacterId.Daniel, "Scrubber pumps have halted. Carbon dioxide is rising—we have less than twenty minutes before levels become toxic!"),
                        new DialogueLine(CharacterId.Aisha, "We have spare lithium hydroxide canisters in Storage, but Marcus said their mounting brackets aren't compatible!")
                    );
                    break;

                case MissionChapterId.Chapter5_TheFiltrationChallenge:
                    PlaySequence(
                        new DialogueLine(CharacterId.Marcus, "Square canister, round hole. Classic engineering nightmare!"),
                        new DialogueLine(CharacterId.Daniel, "Marcus, what about the spacesuit umbilical hoses and cardboard covers from Aisha's scientific manuals?"),
                        new DialogueLine(CharacterId.Marcus, "Daniel, you're a genius! If we wrap the cardboard guide with heavy-duty grey duct tape, we can seal the air leak!"),
                        new DialogueLine(CharacterId.Elena, "Make it happen, team. Just like Apollo 13—calm heads and smart hands.")
                    );
                    break;

                case MissionChapterId.Chapter6_SaveTheOutpost:
                    PlaySequence(
                        new DialogueLine(CharacterId.Elena, "Filtration is holding steady. Now, Marcus, what are our options for that severed solar conduit?"),
                        new DialogueLine(CharacterId.Marcus, "We can either suit up for an immediate EVA, send the rover's robotic arm, or shut down non-essential pods to conserve power."),
                        new DialogueLine(CharacterId.Daniel, "An EVA burns human calories and oxygen, but the robot arm is slower. Every choice has trade-offs."),
                        new DialogueLine(CharacterId.Elena, "Let's review the station telemetry and decide.")
                    );
                    break;

                case MissionChapterId.Chapter7_ANewBeginning:
                    PlaySequence(
                        new DialogueLine(CharacterId.Elena, "Look at that Earthrise through the cupola window, crew. Outpost Alpha is running at full capacity."),
                        new DialogueLine(CharacterId.Marcus, "Solar grid is humming, scrubbers are pristine, and rover batteries are fully topped off."),
                        new DialogueLine(CharacterId.Aisha, "Now we can finally begin deep-drilling in Shackleton Crater!"),
                        new DialogueLine(CharacterId.Daniel, "Crew health is one hundred percent. We survived, and we learned how to thrive beyond Earth.")
                    );
                    break;
            }
        }

        private void TriggerContextualCrewChatter()
        {
            int roll = Random.Range(0, 3);
            switch (roll)
            {
                case 0:
                    PlaySequence(
                        new DialogueLine(CharacterId.Elena, "Marcus, confirm status of our external telemetry antennas."),
                        new DialogueLine(CharacterId.Marcus, "Signal strength to Houston Deep Space Network is solid, Elena. Zero packet loss.")
                    );
                    break;
                case 1:
                    PlaySequence(
                        new DialogueLine(CharacterId.Daniel, "Elena, crew vitals check: resting heart rates are optimal across all four astronauts."),
                        new DialogueLine(CharacterId.Elena, "Thank you, Daniel. Your hydration reminders are keeping everyone in top shape.")
                    );
                    break;
                case 2:
                    PlaySequence(
                        new DialogueLine(CharacterId.Aisha, "Marcus, the rover's inductive charging pad is performing wonderfully."),
                        new DialogueLine(CharacterId.Marcus, "Glad to hear, Aisha! That custom lunar dust seal is doing its job.")
                    );
                    break;
            }
        }

        private void PlaySequence(params DialogueLine[] lines)
        {
            ConversationSequence seq = new ConversationSequence("autonomous_crew", "Crew Communications");
            foreach (var l in lines)
            {
                seq.Lines.Add(l);
            }

            if (DialogueUI.Instance != null)
            {
                DialogueUI.Instance.PlayConversation(seq);
            }
        }
    }
}
