using UnityEngine;
using UnityEngine.UI;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System3.Interface.Choices
{
    public class StrategicChoiceUI : MonoBehaviour
    {
        [Header("Modal Root")]
        [SerializeField] private GameObject choiceRoot;

        [Header("Choice Buttons")]
        [SerializeField] private Button astronautEvaButton;
        [SerializeField] private Button roboticArmButton;
        [SerializeField] private Button powerConserveButton;

        private void Awake()
        {
            if (astronautEvaButton != null) astronautEvaButton.onClick.AddListener(() => MakeChoice(RepairChoice.AstronautEVA));
            if (roboticArmButton != null) roboticArmButton.onClick.AddListener(() => MakeChoice(RepairChoice.RoboticMaintenance));
            if (powerConserveButton != null) powerConserveButton.onClick.AddListener(() => MakeChoice(RepairChoice.ConservePowerDelay));
        }

        private void Start()
        {
            if (choiceRoot != null) choiceRoot.SetActive(false);
        }

        private void OnEnable()
        {
            OutpostEventChannel.StrategicChoiceOpened += OpenChoiceModal;
        }

        private void OnDisable()
        {
            OutpostEventChannel.StrategicChoiceOpened -= OpenChoiceModal;
        }

        public void OpenChoiceModal()
        {
            if (choiceRoot != null) choiceRoot.SetActive(true);
            OutpostEventChannel.RaisePlaySfx("info_chime");
        }

        private void MakeChoice(RepairChoice choice)
        {
            if (choiceRoot != null) choiceRoot.SetActive(false);
            OutpostEventChannel.RaiseRepairChoiceMade(choice);
            OutpostEventChannel.RaisePlaySfx("ui_click");
        }
    }
}
