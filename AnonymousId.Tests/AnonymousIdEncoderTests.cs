﻿using System;
using Xunit;

namespace Onots.AspNetCore.Identity.Anonymous.Tests
{
    public class AnonymousIdEncoderTests
    {
        [Fact]
        public void TestEncodeAndDecode()
        {
            AnonymousIdData originalValue = new AnonymousIdData(Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(5));
            string encodedValue = AnonymousIdEncoder.Encode(originalValue);
            AnonymousIdData decodedValue = AnonymousIdEncoder.Decode(encodedValue);

            Assert.Equal(originalValue.AnonymousId, decodedValue.AnonymousId);
            Assert.Equal(originalValue.ExpireDate, decodedValue.ExpireDate);
        }

        [Fact]
        public void TestEncodeExpired()
        {
            AnonymousIdData originalValue = new AnonymousIdData(Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(-5));
            Exception ex = Assert.Throws<ArgumentException>(() => AnonymousIdEncoder.Encode(originalValue));
        }
    }
}