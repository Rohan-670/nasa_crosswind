using UnityEngine;
using UnityEngine.UI;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Emergency;

namespace Outpost.System3.Interface.Minigames
{
    public class FiltrationPuzzleUI : MonoBehaviour
    {
        [Header("Puzzle Panel Root")]
        [SerializeField] private GameObject puzzleRoot;

        [Header("Part Buttons & Indicators")]
        [SerializeField] private Button canisterButton;
        [SerializeField] private Image canisterCheckmark;
        [SerializeField] private Button cardboardButton;
        [SerializeField] private Image cardboardCheckmark;
        [SerializeField] private Button hoseButton;
        [SerializeField] private Image hoseCheckmark;
        [SerializeField] private Button tapeButton;
        [SerializeField] private Image tapeCheckmark;

        [Header("Assembly & Feedback")]
        [SerializeField] private Button assembleButton;
        [SerializeField] private Text statusText;
        [SerializeField] private Button closeButton;

        private FiltrationChallengeLogic puzzleLogic = new FiltrationChallengeLogic();

        private void Awake()
        {
            if (canisterButton != null) canisterButton.onClick.AddListener(() => ToggleComponent("canister"));
            if (cardboardButton != null) cardboardButton.onClick.AddListener(() => ToggleComponent("cardboard"));
            if (hoseButton != null) hoseButton.onClick.AddListener(() => ToggleComponent("hose"));
            if (tapeButton != null) tapeButton.onClick.AddListener(() => ToggleComponent("tape"));

            if (assembleButton != null) assembleButton.onClick.AddListener(AttemptAssembly);
            if (closeButton != null) closeButton.onClick.AddListener(ClosePuzzle);
        }

        private void Start()
        {
            if (puzzleRoot != null) puzzleRoot.SetActive(false);
            UpdateUI();
        }

        private void OnEnable()
        {
            OutpostEventChannel.FiltrationChallengeOpened += OpenPuzzle;
        }

        private void OnDisable()
        {
            OutpostEventChannel.FiltrationChallengeOpened -= OpenPuzzle;
        }

        public void OpenPuzzle()
        {
            puzzleLogic.Reset();
            UpdateUI();
            if (puzzleRoot != null) puzzleRoot.SetActive(true);
            if (statusText != null) statusText.text = "MISSION: Adapt square LiOH canister into round scrubber socket using hoses, cardboard, and duct tape!";
        }

        public void ClosePuzzle()
        {
            if (puzzleRoot != null) puzzleRoot.SetActive(false);
        }

        private void ToggleComponent(string part)
        {
            puzzleLogic.TogglePart(part);
            OutpostEventChannel.RaisePlaySfx("ui_click");
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (canisterCheckmark != null) canisterCheckmark.color = puzzleLogic.HasCanister ? Color.green : Color.gray;
            if (cardboardCheckmark != null) cardboardCheckmark.color = puzzleLogic.HasCardboardGuide ? Color.green : Color.gray;
            if (hoseCheckmark != null) hoseCheckmark.color = puzzleLogic.HasSuitHose ? Color.green : Color.gray;
            if (tapeCheckmark != null) tapeCheckmark.color = puzzleLogic.HasDuctTape ? Color.green : Color.gray;
        }

        private void AttemptAssembly()
        {
            if (puzzleLogic.IsAdapterCompleted)
            {
                if (statusText != null)
                {
                    statusText.text = "SUCCESS! Airtight seal confirmed! CO2 scrubbers are drawing air through the LiOH canister!";
                    statusText.color = Color.green;
                }

                OutpostEventChannel.RaisePlaySfx("victory_chime");
                Invoke(nameof(CompleteChallenge), 1.5f);
            }
            else
            {
                if (statusText != null)
                {
                    statusText.text = "INCOMPLETE: Air leak detected! You must connect the canister, cardboard funnel, suit hose, and seal with duct tape.";
                    statusText.color = Color.red;
                }

                OutpostEventChannel.RaisePlaySfx("alarm_siren");
            }
        }

        private void CompleteChallenge()
        {
            ClosePuzzle();
            OutpostEventChannel.RaiseFiltrationChallengeCompleted(true);
        }
    }
}
