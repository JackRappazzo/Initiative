using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public abstract class WhenTestingMapToCreature : WhenTestingCreatureMapper
    {
        protected MonsterJson MonsterJson;
        protected Creature Result;

        [When(DoNotRethrowExceptions: true)]
        public void MapToCreatureIsCalled()
        {
            Result = CreatureMapper.MapToCreature(MonsterJson);
        }
    }
}