using System;
using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    public interface IEducationalService
    {
        EducationalFact GetFact(EducationalTopic topic);
        void TriggerFactPopup(EducationalTopic topic);
        event Action<EducationalFact> OnFactPresented;
    }
}
