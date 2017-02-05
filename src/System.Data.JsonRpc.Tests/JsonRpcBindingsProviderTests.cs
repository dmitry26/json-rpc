using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcBindingsProviderTests
    {
        [Fact]
        public void FlowWhenIdIsNumber()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Equal(0, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.SetBinding(100L, "test_method");

            Assert.Equal(1, jsonRpcBindingsProvider.Count);

            Assert.True(jsonRpcBindingsProvider.TryGetBinding(100L, out var method));
            Assert.Equal("test_method", method);

            jsonRpcBindingsProvider.RemoveBinding(100L);

            Assert.False(jsonRpcBindingsProvider.TryGetBinding(100L, out method));
            Assert.Equal(0, jsonRpcBindingsProvider.Count);
        }

        [Fact]
        public void FlowWhenIdIsString()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Equal(0, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.SetBinding("100", "test_method");

            Assert.Equal(1, jsonRpcBindingsProvider.Count);

            Assert.True(jsonRpcBindingsProvider.TryGetBinding("100", out var method));
            Assert.Equal("test_method", method);

            jsonRpcBindingsProvider.RemoveBinding("100");

            Assert.False(jsonRpcBindingsProvider.TryGetBinding("100", out method));
            Assert.Equal(0, jsonRpcBindingsProvider.Count);
        }

        [Fact]
        public void FlowWhenIdIsMixed()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Equal(0, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.SetBinding(100L, "test_method_number");

            Assert.Equal(1, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.SetBinding("100", "test_method_string");

            Assert.Equal(2, jsonRpcBindingsProvider.Count);

            Assert.True(jsonRpcBindingsProvider.TryGetBinding(100L, out var method_number));
            Assert.Equal("test_method_number", method_number);

            Assert.True(jsonRpcBindingsProvider.TryGetBinding("100", out var method_string));
            Assert.Equal("test_method_string", method_string);

            jsonRpcBindingsProvider.RemoveBinding(100L);

            Assert.False(jsonRpcBindingsProvider.TryGetBinding(100L, out method_number));
            Assert.Equal(1, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.RemoveBinding("100");

            Assert.False(jsonRpcBindingsProvider.TryGetBinding("100", out method_string));
            Assert.Equal(0, jsonRpcBindingsProvider.Count);
        }

        [Fact]
        public void ClearWhenIdIsNumber()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Equal(0, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.SetBinding(100L, "test_method_number");

            Assert.Equal(1, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.ClearBindings();

            Assert.False(jsonRpcBindingsProvider.TryGetBinding(100L, out var method_number));

            Assert.Equal(0, jsonRpcBindingsProvider.Count);
        }

        [Fact]
        public void ClearWhenIdIsString()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Equal(0, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.SetBinding("100", "test_method_string");

            Assert.Equal(1, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.ClearBindings();

            Assert.False(jsonRpcBindingsProvider.TryGetBinding("100", out var method_string));

            Assert.Equal(0, jsonRpcBindingsProvider.Count);
        }

        [Fact]
        public void ClearWhenIdIsMixed()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Equal(0, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.SetBinding(100L, "test_method_number");
            jsonRpcBindingsProvider.SetBinding("100", "test_method_string");

            Assert.Equal(2, jsonRpcBindingsProvider.Count);

            jsonRpcBindingsProvider.ClearBindings();

            Assert.False(jsonRpcBindingsProvider.TryGetBinding(100L, out var method_number));
            Assert.False(jsonRpcBindingsProvider.TryGetBinding("100", out var method_string));

            Assert.Equal(0, jsonRpcBindingsProvider.Count);
        }

        [Fact]
        public void SetBindingWhenIdIsNumberAndMethodIsNull()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcBindingsProvider.SetBinding(100L, default(string)));
        }

        [Fact]
        public void SetBindingWhenIdIsNumberAndMethodIsEmptyString()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentException>(() =>
                jsonRpcBindingsProvider.SetBinding(100L, string.Empty));
        }

        [Fact]
        public void SetBindingWhenIdIsStringAndIdIsNull()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcBindingsProvider.SetBinding(default(string), "test_method"));
        }

        [Fact]
        public void SetBindingWhenIdIsStringAndIdIsEmptyString()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentException>(() =>
                jsonRpcBindingsProvider.SetBinding(string.Empty, "test_method"));
        }

        [Fact]
        public void SetBindingWhenIdIsStringAndMethodIsNull()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcBindingsProvider.SetBinding("100", default(string)));
        }

        [Fact]
        public void SetBindingWhenIdIsStringAndMethodIsEmptyString()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentException>(() =>
                jsonRpcBindingsProvider.SetBinding("100", string.Empty));
        }

        [Fact]
        public void TryGetBindingWhenIdIsStringAndIdIsNull()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcBindingsProvider.TryGetBinding(default(string), out var method));
        }

        [Fact]
        public void TryGetBindingWhenIdIsStringAndIdIsEmptyString()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentException>(() =>
                jsonRpcBindingsProvider.TryGetBinding(string.Empty, out var method));
        }

        [Fact]
        public void RemoveBindingWhenIdIsStringAndIdIsNull()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcBindingsProvider.RemoveBinding(default(string)));
        }

        [Fact]
        public void RemoveBindingWhenIdIsStringAndIdIsEmptyString()
        {
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            Assert.Throws<ArgumentException>(() =>
                jsonRpcBindingsProvider.RemoveBinding(string.Empty));
        }
    }
}