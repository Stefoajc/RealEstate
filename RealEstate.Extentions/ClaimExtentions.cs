using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Microsoft.Owin.Security;

namespace RealEstate.Extentions
{
    public static class ClaimExtentions
    {
        public static Claim AddUpdateClaimInCookie(this IPrincipal currentPrincipal, string key, string value)
        {
            if (!(currentPrincipal.Identity is ClaimsIdentity identity))
                throw new NotSupportedException();

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            var claimToAdd = new Claim(key, value);
            identity.AddClaim(claimToAdd);
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });

            return claimToAdd;
        }

        public static Claim GetClaimFromCookie(this IPrincipal currentPrincipal, string key)
        {
            if (!(currentPrincipal.Identity is ClaimsIdentity identity))
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim;
        }

        public static void RemoveClaimFromCookie(this IPrincipal currentPrincipal, string key)
        {
            if (!(currentPrincipal.Identity is ClaimsIdentity identity))
                throw new NotSupportedException();

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);
        }
    }
}
