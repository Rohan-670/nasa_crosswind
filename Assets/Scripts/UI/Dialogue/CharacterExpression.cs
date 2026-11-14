using System;

namespace Outpost.UI.Dialogue
{
    /// <summary>
    /// Identifiers for crew members and AI.
    /// </summary>
    public enum CharacterIdentifier
    {
        CommanderElena = 0,
        EngineerMarcus = 1,
        ScientistAisha = 2,
        MedicalDaniel = 3,
        ORION = 4,
        StationIntercom = 5
    }

    /// <summary>
    /// Emotional / expressive states for character dialogue portraits.
    /// </summary>
    public enum CharacterExpression
    {
        Neutral = 0,
        Happy = 1,
        Concerned = 2,
        Determined = 3,
        Serious = 4,
        Alert = 5
    }

    /// <summary>
    /// Type of trigger activating a conversation.
    /// </summary>
    public enum ConversationTriggerType
    {
        DirectInteraction = 0,
        MissionMilestone = 1,
        ProximityZone = 2,
        EmergencyAlert = 3,
        PreEventBriefing = 4,
        RadioChatter = 5
    }
}
