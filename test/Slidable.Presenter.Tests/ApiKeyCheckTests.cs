using System;
using Slidable.Presenter.Authentication;
using Xunit;

namespace Slidable.Presenter.Tests
{
    public class ApiKeyCheckTests
    {
        [Fact]
        public void GetUserName_Gets_User_Name_From_Well_Formed_String()
        {
            var header = "API-Key bob:fooble";
            var colon = header.IndexOf(':');
            var userName = ApiKeyCheck.GetUserName(header, colon);
            Assert.Equal("bob", new string(userName));
        }
        
        [Fact]
        public void GetKey_Gets_Key_From_Well_Formed_String()
        {
            var header = "API-Key bob:fooble";
            var colon = header.IndexOf(':');
            var key = ApiKeyCheck.GetKey(header, colon);
            Assert.Equal("fooble", new string(key));
        }
    }
}
