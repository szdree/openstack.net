﻿namespace OpenStackNet.Testing.Unit.Providers.Rackspace
{
    using System;
    using System.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Domain.Converters;
    using net.openstack.Providers.Rackspace.Objects.Request;
    using net.openstack.Providers.Rackspace.Objects.Response;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class JsonModelTests
    {
        /// <seealso cref="PasswordCredential"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        [TestMethod]
        public void TestPasswordCredential()
        {
            string json = @"{ ""username"" : ""test_user"", ""password"" : ""mypass"" }";
            PasswordCredential credentials = JsonConvert.DeserializeObject<PasswordCredential>(json);
            Assert.IsNotNull(credentials);
            Assert.AreEqual("test_user", credentials.Username);
            Assert.AreEqual("mypass", credentials.Password);
        }

        /// <seealso cref="PasswordCredentialResponse"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        [TestMethod]
        public void TestPasswordCredentialResponse()
        {
            string json = @"{ ""passwordCredentials"" : { username : ""test_user"", password : ""mypass"" } }";
            PasswordCredentialResponse response = JsonConvert.DeserializeObject<PasswordCredentialResponse>(json);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PasswordCredential);
            Assert.AreEqual("test_user", response.PasswordCredential.Username);
            Assert.AreEqual("mypass", response.PasswordCredential.Password);
        }

        /// <seealso href="http://docs.openstack.org/api/openstack-compute/2/content/ServerUpdate.html">Update Server (OpenStack Compute API v2 and Extensions Reference)</seealso>
        [TestMethod]
        public void TestUpdateServerRequest()
        {
            UpdateServerRequest request = new UpdateServerRequest("new-name", IPAddress.Parse("10.0.0.1"), IPAddress.Parse("2607:f0d0:1002:51::4"));
            string expectedJson = @"{""server"":{""name"":""new-name"",""accessIPv4"":""10.0.0.1"",""accessIPv6"":""2607:f0d0:1002:51::4""}}";
            string actual = JsonConvert.SerializeObject(request, Formatting.None);
            Assert.AreEqual(expectedJson, actual);
        }

        [TestMethod]
        public void TestIPAddressDetailsConverter()
        {
            IPAddressDetailsConverter converter = new IPAddressDetailsConverter();

            string json = @"{ ""version"" : 4, ""addr"" : ""10.0.0.1"" }";
            IPAddress address = JsonConvert.DeserializeObject<IPAddress>(json, converter);
            Assert.AreEqual(IPAddress.Parse("10.0.0.1"), address);

            json = @"{ ""version"" : 6, ""addr"" : ""::babe:4317:0A83"" }";
            address = JsonConvert.DeserializeObject<IPAddress>(json, converter);
            Assert.AreEqual(IPAddress.Parse("::babe:4317:0A83"), address);

            json = JsonConvert.SerializeObject(IPAddress.Parse("10.0.0.1"), converter);
            Assert.AreEqual(@"{""addr"":""10.0.0.1"",""version"":""4""}", json);

            json = JsonConvert.SerializeObject(IPAddress.Parse("::babe:4317:0A83"), converter);
            Assert.AreEqual(@"{""addr"":""::babe:4317:a83"",""version"":""6""}", json);
        }

        [TestMethod]
        public void TestIPAddressSimpleConverter()
        {
            IPAddressSimpleConverter converter = new IPAddressSimpleConverter();

            string json = @"""10.0.0.1""";
            IPAddress address = JsonConvert.DeserializeObject<IPAddress>(json, converter);
            Assert.AreEqual(IPAddress.Parse("10.0.0.1"), address);

            json = @"""::babe:4317:0A83""";
            address = JsonConvert.DeserializeObject<IPAddress>(json, converter);
            Assert.AreEqual(IPAddress.Parse("::babe:4317:0A83"), address);

            json = JsonConvert.SerializeObject(IPAddress.Parse("10.0.0.1"), converter);
            Assert.AreEqual(@"""10.0.0.1""", json);

            json = JsonConvert.SerializeObject(IPAddress.Parse("::babe:4317:0A83"), converter);
            Assert.AreEqual(@"""::babe:4317:a83""", json);
        }

        [TestMethod]
        public void TestPersonalityJsonModel()
        {
            string expectedPath = "/usr/lib/stuff";
            string expectedText = "Example text";
            Personality personality = new Personality(expectedPath, expectedText, Encoding.UTF8);
            Assert.AreEqual(expectedPath, personality.Path);
            Assert.AreEqual(expectedText, Encoding.UTF8.GetString(personality.Content));

            string json = JsonConvert.SerializeObject(personality);
            Personality personality2 = JsonConvert.DeserializeObject<Personality>(json);
            Assert.AreEqual(expectedPath, personality.Path);
            Assert.AreEqual(expectedText, Encoding.UTF8.GetString(personality.Content));

            // make sure the JSON was Base-64 encoded
            JObject personalityObject = JsonConvert.DeserializeObject<JObject>(json);
            Assert.IsInstanceOfType(personalityObject["contents"], typeof(JValue));
            byte[] encodedText = Convert.FromBase64String((string)((JValue)personalityObject["contents"]).Value);
            Assert.AreEqual(expectedText, Encoding.UTF8.GetString(encodedText));
            Assert.AreEqual(personality.Content.Length, encodedText.Length);
        }

        [TestMethod]
        public void TestDiskConfigurationConversions()
        {
            TestExtensibleEnumSerialization(DiskConfiguration.Auto, "OTHER", DiskConfiguration.FromName);
        }

        [TestMethod]
        public void TestImageState()
        {
            TestExtensibleEnumSerialization(ImageState.Active, "OTHER", ImageState.FromName);
        }

        [TestMethod]
        public void TestImageType()
        {
            TestExtensibleEnumSerialization(ImageType.Base, "OTHER", ImageType.FromName);
        }

        [TestMethod]
        public void TestRebootType()
        {
            TestExtensibleEnumSerialization(RebootType.Hard, "OTHER", RebootType.FromName);
        }

        [TestMethod]
        public void TestServerState()
        {
            TestExtensibleEnumSerialization(ServerState.Build, "OTHER", ServerState.FromName);
        }

        [TestMethod]
        public void TestSnapshotState()
        {
            TestExtensibleEnumSerialization(SnapshotState.Available, "OTHER", SnapshotState.FromName);
        }

        [TestMethod]
        public void TestVolumeState()
        {
            TestExtensibleEnumSerialization(VolumeState.Creating, "OTHER", VolumeState.FromName);
        }

        private void TestExtensibleEnumSerialization<T>(T standardItem, string nonStandardName, Func<string, T> fromName)
        {
            if (fromName == null)
                throw new ArgumentNullException("fromName");

            T obj = JsonConvert.DeserializeObject<T>("null");
            Assert.IsNull(obj);

            obj = JsonConvert.DeserializeObject<T>(@"""""");
            Assert.IsNull(obj);

            // matching case, predefined value
            obj = JsonConvert.DeserializeObject<T>('"' + standardItem.ToString() + '"');
            Assert.AreEqual(standardItem, obj);

            // different case, predefined value
            Assert.AreNotEqual(standardItem.ToString(), standardItem.ToString().ToLowerInvariant());
            obj = JsonConvert.DeserializeObject<T>('"' + standardItem.ToString().ToLowerInvariant() + '"');
            Assert.AreEqual(standardItem, obj);

            // new value
            obj = JsonConvert.DeserializeObject<T>('"' + nonStandardName + '"');
            Assert.AreEqual(fromName(nonStandardName), obj);

            // different case, same as value encountered before
            Assert.AreNotEqual(nonStandardName, nonStandardName.ToLowerInvariant());
            obj = JsonConvert.DeserializeObject<T>('"' + nonStandardName.ToLowerInvariant() + '"');
            Assert.AreEqual(fromName(nonStandardName), obj);

            string json = JsonConvert.SerializeObject(standardItem);
            Assert.AreEqual('"' + standardItem.ToString() + '"', json);
        }
    }
}