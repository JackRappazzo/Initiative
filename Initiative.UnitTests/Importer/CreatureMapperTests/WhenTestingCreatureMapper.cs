using Initiative.Import.Core.Services;
using Initiative.Import.Core.Services.Extractors;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.CreatureMapperTests
{
    public abstract class WhenTestingCreatureMapper : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected CreatureMapper CreatureMapper;

        [Dependency]
        protected IArmorClassExtractor ArmorClassExtractor;

        [Dependency]
        protected ISpeedExtractor SpeedExtractor;

        [Dependency]
        protected IConditionImmunitiesExtractor ConditionImmunitiesExtractor;

        [Dependency]
        protected IDamageResistancesExtractor DamageResistancesExtractor;

        [Dependency]
        protected IActionsExtractor ActionsExtractor;

        [Dependency]
        protected IInitiativeCalculator InitiativeCalculator;

        [Dependency]
        protected ISystemNameGenerator SystemNameGenerator;
    }
}