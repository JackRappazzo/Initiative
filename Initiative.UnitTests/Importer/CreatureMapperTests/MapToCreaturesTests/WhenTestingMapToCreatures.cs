using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreaturesTests
{
    public abstract class WhenTestingMapToCreatures : WhenTestingCreatureMapper
    {
        protected IEnumerable<MonsterJson> Monsters;
        protected List<Creature> Result;

        [When(DoNotRethrowExceptions: true)]
        public void MapToCreaturesIsCalled()
        {
            Result = CreatureMapper.MapToCreatures(Monsters);
        }
    }
}