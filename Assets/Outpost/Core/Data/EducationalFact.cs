using System;

namespace Outpost.Core.Data
{
    public enum EducationalTopic
    {
        LunarGravity,
        SolarPower,
        OxygenGeneration,
        CarbonDioxideRemoval,
        WaterRecycling,
        FoodProduction,
        LunarGeology,
        SpaceCommunication,
        ResourceConservation,
        EmergencyEngineering
    }

    [Serializable]
    public class EducationalFact
    {
        public EducationalTopic Topic;
        public string Title;
        public string ShortSummary;
        public string DetailedExplanation;
        public string FunFact;

        public EducationalFact(EducationalTopic topic, string title, string shortSummary, string detailedExplanation, string funFact)
        {
            Topic = topic;
            Title = title;
            ShortSummary = shortSummary;
            DetailedExplanation = detailedExplanation;
            FunFact = funFact;
        }

        public static EducationalFact[] GetEducationalDatabase()
        {
            return new EducationalFact[]
            {
                new EducationalFact(
                    EducationalTopic.LunarGravity,
                    "Lunar Gravity: Bouncing on the Moon!",
                    "Gravity on the Moon is only about 1/6th of Earth's gravity (1.62 m/s²).",
                    "Because the Moon has much less mass than Earth, its gravitational pull is only about 16.6% as strong. If you weigh 36 kg (80 lbs) on Earth, you would only weigh 6 kg (13 lbs) on the Moon! Astronauts hop like kangaroos to move around efficiently.",
                    "Even though you feel lighter on the Moon, your mass and inertia stay the same, which means stopping when running still takes effort!"
                ),
                new EducationalFact(
                    EducationalTopic.SolarPower,
                    "Solar Power on the Ridge of Eternal Light",
                    "Tall solar panels capture sunlight that shines almost continuously near the lunar poles.",
                    "At the Moon's South Pole, mountain peaks and crater ridges receive near-continuous sunlight because the Moon's axial tilt is only 1.5 degrees. Outpost solar arrays track the low horizon sun to keep batteries fully charged.",
                    "Unlike Earth, the Moon has no atmosphere or clouds, so solar panels receive pure, unfiltered sunlight!"
                ),
                new EducationalFact(
                    EducationalTopic.OxygenGeneration,
                    "Making Breathable Air: Water Electrolysis",
                    "Electricity splits water molecules (H2O) into hydrogen and breathable oxygen (O2).",
                    "By running an electric current through water extracted from lunar ice, outpost life-support systems split water into Oxygen gas for astronauts to breathe and Hydrogen gas that can be used for rocket fuel.",
                    "One adult astronaut needs about 0.84 kilograms of pure oxygen every single day to stay healthy!"
                ),
                new EducationalFact(
                    EducationalTopic.CarbonDioxideRemoval,
                    "Scrubbing the Air: Removing Carbon Dioxide",
                    "Chemical filters absorb toxic CO2 gas so astronauts can safely breathe in enclosed habitats.",
                    "Every time astronauts exhale, they release Carbon Dioxide (CO2). If CO2 builds up inside the habitat, it causes headaches and dizziness. Canisters filled with Lithium Hydroxide (LiOH) chemically trap the CO2 molecules.",
                    "In 1970, Apollo 13 astronauts famously used plastic bags, cardboard flight manuals, and duct tape to adapt square command module filters into round lunar module sockets!"
                ),
                new EducationalFact(
                    EducationalTopic.WaterRecycling,
                    "Every Drop Counts: Closed-Loop Water Recovery",
                    "Outpost systems filter and purify 98% of all water, including humidity from astronaut breath.",
                    "Bringing water from Earth to the Moon is extremely expensive. The lunar habitat uses advanced multi-stage filtration, reverse osmosis, and ultraviolet sterilization to recycle every drop of water into fresh drinking water.",
                    "The water astronauts drink today on the space station was once their sweat or breath vapor from yesterday!"
                ),
                new EducationalFact(
                    EducationalTopic.FoodProduction,
                    "Greenhouse Farming Without Soil: Hydroponics",
                    "Crops grow in nutrient-rich water under specialized pink LED grow lights.",
                    "Plants like dwarf wheat, tomatoes, and lettuce provide fresh vitamins and crunch, while naturally converting carbon dioxide back into oxygen through photosynthesis. Hydroponic farming uses 90% less water than traditional soil farming.",
                    "Plants grow taller with thinner stems in lunar gravity, but their roots still know which way to grow by sensing moisture and nutrients!"
                ),
                new EducationalFact(
                    EducationalTopic.LunarGeology,
                    "Prehistoric Ice in Shackleton Crater",
                    "Permanently shadowed craters at the lunar poles hide trillions of kilograms of frozen water.",
                    "Because sunlight hits the lunar poles at a grazing angle, deep crater floors have been in pitch-black shadow for over two billion years. These 'cold traps' are as cold as -246°C (-410°F), preserving ancient ice that comets left behind.",
                    "Scientists believe lunar water-ice will be used not only for drinking, but also split into rocket fuel for future voyages to Mars!"
                ),
                new EducationalFact(
                    EducationalTopic.SpaceCommunication,
                    "Talking Across 384,000 Kilometers",
                    "Radio and laser signals take about 1.3 seconds to travel between the Moon and Earth.",
                    "Because radio waves travel at the speed of light (300,000 km/s), when an astronaut speaks to Mission Control in Houston, there is an unavoidable round-trip delay of approximately 2.6 seconds before hearing a response.",
                    "High-frequency laser communications can transmit high-definition video from the lunar surface to Earth in seconds!"
                ),
                new EducationalFact(
                    EducationalTopic.ResourceConservation,
                    "Battery Storage and Night-Time Management",
                    "Saving energy during shadows is vital when solar panels aren't generating peak power.",
                    "When the solar angle drops or external equipment is damaged, the outpost switches to emergency battery reserves. Non-essential scientific labs and luxury heaters are powered down to keep life-support scrubbers and oxygen running.",
                    "Smart electrical microgrids automatically prioritize heating, oxygen, and medical units above all other systems."
                ),
                new EducationalFact(
                    EducationalTopic.EmergencyEngineering,
                    "Teamwork & Creativity: The Apollo 13 Lesson",
                    "When an emergency strikes in deep space, engineers use whatever tools they have on hand.",
                    "Space missions require calm leadership, clear communication, and inventive thinking. If specialized parts fail or are damaged by micrometeoroids, astronauts and ground teams work together to solve unexpected challenges using creativity and physics.",
                    "Engineers call clever, emergency hardware solutions 'jerry-rigging' or 'Apollo 13 ingenuity'!"
                )
            };
        }
    }
}
