using AutoFixture;

namespace DietAssistant.UnitTests
{
    public class TestsBase
    {
        protected IFixture _fixture;

        protected TestsBase()
        {
            _fixture = new Fixture();
        }
    }
}
