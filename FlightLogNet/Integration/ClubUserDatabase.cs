namespace FlightLogNet.Integration
{
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    using Microsoft.Extensions.Configuration;

    using RestSharp;

    public class ClubUserDatabase(IConfiguration configuration) : IClubUserDatabase
    {
        private static IList<PersonModel> cachedUsers;
        private static readonly object cacheLock = new();

        private readonly string baseUrl = configuration.GetValue<string>("ClubUsersApi") ?? "http://vyuka.profinit.eu:8080/";

        public bool TryGetClubUser(long memberId, out PersonModel personModel)
        {
            personModel = this.GetClubUsers().FirstOrDefault(person => person.MemberId == memberId);

            return personModel != null;
        }

        public IList<PersonModel> GetClubUsers()
        {
            if (cachedUsers != null)
            {
                return cachedUsers;
            }

            lock (cacheLock)
            {
                if (cachedUsers != null)
                {
                    return cachedUsers;
                }

                cachedUsers = this.TransformToPersonModel(this.ReceiveClubUsers());
                return cachedUsers;
            }
        }

        private List<ClubUser> ReceiveClubUsers()
        {
            var client = new RestClient(this.baseUrl);
            var request = new RestRequest("club/user", Method.Get);
            var response = client.Execute<List<ClubUser>>(request);

            return response.Data ?? [];
        }

        private List<PersonModel> TransformToPersonModel(IList<ClubUser> users)
        {
            if (users == null)
            {
                return [];
            }

            return users.Select(user => user.ToPersonModel()).ToList();
        }
    }
}
