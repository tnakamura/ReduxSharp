using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReduxSharp.Tests
{
    public class StandardActionTest
    {
        [Fact]
        public void Constructor_initialize_new_instance()
        {
            var action = new StandardAction("TEST");
            Assert.NotNull(action);
            Assert.Equal("TEST", action.Type);
            Assert.Null(action.Payload);
        }

        [Fact]
        public void Constructor_initialize_new_instance_with_payload()
        {
            var action = new StandardAction("TEST", new Dictionary<string, object>()
            {
                ["FOO"] = "BAR",
            });
            Assert.NotNull(action);
            Assert.Equal("TEST", action.Type);
            Assert.NotNull(action.Payload);
            Assert.Equal("BAR", action.Payload["FOO"]);
        }

        [Fact]
        public void Constructor_throws_ArgumentNullException_when_type_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new StandardAction(null);
            });
        }
    }
}
