using Microsoft.AspNetCore.Http;

namespace Onots.AspNetCore.Identity.Anonymous
{
    public class AnonymousIdCookieOptions : CookieOptions
    {
        public string Name { get; set; }
        public bool SlidingExpiration { get; set; } = true;
        public int Timeout { get; set; }
    }
}