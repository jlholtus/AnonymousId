using Microsoft.AspNetCore.Http;
using ReturnTrue.AspNetCore.Identity.Anonymous;
using System;
using Xunit;

namespace AnonymousId.Tests
{
    public class AnonymousIdCookieOptionsBuilderTests
    {
        [Fact]
        public void TestDefaultValues()
        {
            CookieBuilder options = new AnonymousIdCookieOptionsBuilder().Build();

            Assert.Equal(options.Name, ".ASPXANONYMOUS");
            Assert.Equal(options.Path, "/");
            Assert.Equal(options.Expiration, TimeSpan.FromSeconds(100000));
            Assert.Equal(options.Domain, null);
            Assert.Equal(options.SecurePolicy, CookieSecurePolicy.SameAsRequest);
        }

        [Fact]
        public void TestCustomValues()
        {
            CookieBuilder options = new AnonymousIdCookieOptionsBuilder()
                .SetCustomCookieName("CUSTOMCOOKIENAME")
                .SetCustomCookiePath("/path")
                .SetCustomCookieExpiration(1000000)
                .SetCustomCookieDomain("www.example.com")
                .SetCustomCookieSecurePolicy(CookieSecurePolicy.Always)
                .Build();

            Assert.Equal("CUSTOMCOOKIENAME", options.Name);
            Assert.Equal("/path", options.Path);
            Assert.Equal(1000000, options.Expiration.Value.TotalSeconds);
            Assert.Equal("www.example.com", options.Domain);
            Assert.Equal(CookieSecurePolicy.Always, options.SecurePolicy);
        }

        [Fact]
        public void TestMinimumTimeout()
        {
            CookieBuilder options = new AnonymousIdCookieOptionsBuilder()
                .SetCustomCookieExpiration(0)
                .Build();
            
            Assert.Equal(1, options.Expiration.Value.TotalSeconds);
        }

        [Fact]
        public void TestMaximumTimeout()
        {
            CookieBuilder options = new AnonymousIdCookieOptionsBuilder()
                .SetCustomCookieExpiration(int.MaxValue)
                .Build();

            Assert.Equal(60 * 60 * 24 * 365 * 2, options.Expiration.Value.TotalSeconds);
        }
    }
}