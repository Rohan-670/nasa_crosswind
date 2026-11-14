namespace Outpost.Core.Data
{
    public enum CharacterId
    {
        Elena,      // Commander Elena: Calm, disciplined, responsible, decisive
        Marcus,     // Engineer Marcus: Practical, confident, technical, humorous
        Aisha,      // Scientist Aisha: Curious, intelligent, analytical, observant
        Daniel,     // Medical Officer Daniel: Compassionate, careful, reassuring
        Orion       // ORION: AI Assistant (Onboard Robotic Intelligence and Operations Network)
    }

    [System.Serializable]
    public class CharacterProfile
    {
        public CharacterId Id;
        public string FullName;
        public string Role;
        public string Personality;
        public string Responsibilities;
        public UnityEngine.Color ThemeColor;

        public CharacterProfile(CharacterId id, string fullName, string role, string personality, string responsibilities, UnityEngine.Color color)
        {
            Id = id;
            FullName = fullName;
            Role = role;
            Personality = personality;
            Responsibilities = responsibilities;
            ThemeColor = color;
        }

        public static CharacterProfile GetProfile(CharacterId id)
        {
            switch (id)
            {
                case CharacterId.Elena:
                    return new CharacterProfile(
                        id,
                        "Commander Elena",
                        "Mission Commander",
                        "Calm, disciplined, responsible, and decisive.",
                        "Crew leadership, mission coordination, and critical emergency decisions.",
                        new UnityEngine.Color(0.95f, 0.75f, 0.2f) // Command Gold
                    );
                case CharacterId.Marcus:
                    return new CharacterProfile(
                        id,
                        "Engineer Marcus",
                        "Systems Engineer",
                        "Practical, confident, technical, and occasionally humorous.",
                        "Repairs, electrical grid, mechanical equipment, and rover maintenance.",
                        new UnityEngine.Color(0.2f, 0.55f, 0.95f) // Engineering Blue
                    );
                case CharacterId.Aisha:
                    return new CharacterProfile(
                        id,
                        "Scientist Aisha",
                        "Lunar Scientist & Geologist",
                        "Curious, intelligent, analytical, and observant.",
                        "Lunar geology, ice core extraction, scientific research, and environmental analysis.",
                        new UnityEngine.Color(0.15f, 0.85f, 0.85f) // Science Cyan
                    );
                case CharacterId.Daniel:
                    return new CharacterProfile(
                        id,
                        "Medical Officer Daniel",
                        "Medical Officer & Life Support",
                        "Compassionate, careful, and reassuring.",
                        "Crew health, physiological survival monitoring, and medical supplies.",
                        new UnityEngine.Color(0.3f, 0.85f, 0.4f) // Medical Green
                    );
                case CharacterId.Orion:
                default:
                    return new CharacterProfile(
                        id,
                        "O.R.I.O.N.",
                        "AI Assistant",
                        "Analytical, vigilant, objective, and supportive.",
                        "System telemetry, hazard warnings, navigation, and educational STEM explanations.",
                        new UnityEngine.Color(0.7f, 0.4f, 1.0f) // AI Violet/Holo
                    );
            }
        }
    }
}
