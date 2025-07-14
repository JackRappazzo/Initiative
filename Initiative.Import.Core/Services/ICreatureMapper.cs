using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;

namespace Initiative.Import.Core.Services
{
    public interface ICreatureMapper
    {
        Creature MapToCreature(MonsterJson monsterJson);
        List<Creature> MapToCreatures(IEnumerable<MonsterJson> monsters);
    }
}