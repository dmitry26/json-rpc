using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace System.Data.JsonRpc.Tests
{
    public sealed partial class JsonRpcSerializerTests
    {
        private readonly ITestOutputHelper _output;

        public JsonRpcSerializerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private void CompareJsonStrings(string expected, string actual)
        {
            var expectedToken = JToken.Parse(expected);
            var actualToken = JToken.Parse(actual);

            _output.WriteLine(actualToken.ToString(Formatting.Indented));

            Assert.True(JToken.DeepEquals(expectedToken, actualToken), "Actual JSON string differs from expected");
        }
   }
}