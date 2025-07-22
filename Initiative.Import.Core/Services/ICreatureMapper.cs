using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using System.Text.Json;

namespace Initiative.Import.Core.Services
{
    public interface ICreatureMapper
    {
        Creature MapToCreature(MonsterJson monsterJson);
        List<Creature> MapToCreatures(IEnumerable<MonsterJson> monsters);
    }
}