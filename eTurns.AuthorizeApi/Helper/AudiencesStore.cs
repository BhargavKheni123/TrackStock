using eTurns.AuthorizeApi.Entities;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace eTurns.AuthorizeApi.Helper
{
    public static class AudiencesStore
    {
        public static UserMasterDAL obj = new UserMasterDAL();
        public static ConcurrentDictionary<string, Audience> AudiencesList = new ConcurrentDictionary<string, Audience>();

        static AudiencesStore()
        {
            UserMasterDAL objUserMasterDAL = new Helper.UserMasterDAL();
            List<Audience> lstA = objUserMasterDAL.getAllAudience();
            if (lstA != null && lstA.Count() > 0)
            {
                foreach (var item in lstA)
                {
                    AudiencesList.TryAdd(item.ClientId,
                                new Audience
                                {
                                    ClientId = item.ClientId,
                                    Base64Secret = item.Base64Secret,
                                    Name = item.Name
                                });
                }
            }

        }

        public static Audience AddAudience(string name)
        {
            UserMasterDAL objUserMasterDAL = new Helper.UserMasterDAL();
            var clientId = Guid.NewGuid().ToString("N");

            var key = new byte[32];
            RNGCryptoServiceProvider.Create().GetBytes(key);
            var base64Secret = TextEncodings.Base64Url.Encode(key);

            Audience newAudience = new Audience { ClientId = clientId, Base64Secret = base64Secret, Name = name };
            newAudience = objUserMasterDAL.AddAudience(newAudience);
            AudiencesList.TryAdd(clientId, newAudience);
            return newAudience;
        }

        public static Audience FindAudience(string clientId)
        {
            Audience audience = null;
            if (AudiencesList.TryGetValue(clientId, out audience))
            {
                return audience;
            }
            return null;
        }
    }
}