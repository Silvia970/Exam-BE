using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;
using System.Text.Json;


namespace Story_Spoiler_Exam
{
    [TestFixture]
    public class StorySpoilerTests
    {
        private RestClient client;
        private static string createdStoryId;
        private const string baseUrl = "https://d3s5nxhwblsjbi.cloudfront.net";

        [OneTimeSetUp]
        public void Setup()
        {
            string token = GetJwtToken("User0077", "123456");

            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };

            client = new RestClient(options);
        }
        
        private string GetJwtToken(string username, string password)
        {
            var loginClient = new RestClient(baseUrl);
            var request = new RestRequest("/api/User/Authentication", Method.Post);

            request.AddJsonBody(new { username, password });

            var response = loginClient.Execute(request);

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);

            return json.GetProperty("accessToken").GetString() ?? string.Empty;

        }

        [Test, Order(1)]
        public void CreateStorySpoiler_ShouldReturnCreated()
        {
            var story = new
            {
                Title = "Brand New Spoiler",
                Description = "We have a brand new spoiler",
                Url = ""
            };

            var request = new RestRequest("/api/Story/Create", Method.Post);
            request.AddJsonBody(story);

            var response = client.Execute(request);

            Console.WriteLine("Status: " + response.StatusCode);
            Console.WriteLine("Content: " + response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            createdStoryId = json.GetProperty("storyId").GetString() ?? string.Empty;

            Assert.That(response.Content, Does.Contain("Successfully created!"));

        }

        [Test, Order(2)]

        public void EditStorySpoiler_ShouldReturnOk()
        {
            var editedStory = new
            {
                Title = "Updated Story Spoiler",
                Description = "We have updated spoiler",
                Url = ""
            };

            var request = new RestRequest($"/api/Story/Edit/{createdStoryId}", Method.Put);
            request.AddJsonBody(editedStory);

            var response = client.Execute(request);

            Console.WriteLine("Status: " + response.StatusCode);
            Console.WriteLine("Content: " + response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Does.Contain("Successfully edited"));

        }

        [Test, Order(3)]

        public void GetAllStorySpoilers_ShouldReturnList()
        {
            var request = new RestRequest("/api/Story/All", Method.Get);

            var response = client.Execute(request);

            Console.WriteLine("Status: " + response.StatusCode);
            Console.WriteLine("Content: " + response.Content);

            Assert.That(response.StatusCode, Is.EqualTo((HttpStatusCode)HttpStatusCode.OK));

            var stories = JsonSerializer.Deserialize<List<object>>(response.Content);

            Assert.That(stories, Is.Not.Empty);
        }

        [Test, Order(4)]

        public void DeleteStorySpoiler_ShoudReturnOk()
        {
            var request = new RestRequest($"/api/Story/Delete/{createdStoryId}", Method.Delete);

            var response = client.Execute(request);

            Console.WriteLine("Status: " + response.StatusCode);
            Console.WriteLine("Content: " + response.Content);

            Assert.That(response.StatusCode, Is.EqualTo((HttpStatusCode)HttpStatusCode.OK));
            Assert.That(response.Content, Does.Contain("Deleted successfully!"));
        }
        

        [Test, Order(5)]

        public void CreateStorySpoiler_WithoutRequiredFields_ShouldReturnBadRequest()
        {
            var story = new
            {
                Title = "",
                Description = "",
            };

            var request = new RestRequest("/api/Story/Create ", Method.Post);
            request.AddJsonBody(story);

            var response = client.Execute(request);

            Console.WriteLine("Status: " + response.StatusCode);
            Console.WriteLine("Content: " + response.Content);

            Assert.That(response.StatusCode, Is.EqualTo((HttpStatusCode)HttpStatusCode.BadRequest));
        }


        [Test, Order(6)]

        public void EditNonExistingStorySpoiler_ShouldReturnNotFound()
        {
            string fakeID = "11111";
            var editedStory = new
            {
                Title = "Some Title",
                Description = "Some description",
                Url = ""
            };

            var request = new RestRequest($"/api/Story/Edit/{fakeID} ", Method.Put);
            request.AddJsonBody(editedStory);

            var response = client.Execute(request);

            Console.WriteLine("Status: " + response.StatusCode);
            Console.WriteLine("Content: " + response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Content, Does.Contain("No spoilers..."));


        }

        [Test, Order(7)]

        public void DeleteNonExistingStorySpoiler_ShouldReturnBadRequest()
        {
            string fakeID = "11111";
            var request = new RestRequest($"/api/Story/Delete/{fakeID}", Method.Delete);

            var response = client.Execute(request);

            Console.WriteLine("Status: " + response.StatusCode);
            Console.WriteLine("Content: " + response.Content);

            Assert.That(response.StatusCode, Is.EqualTo((HttpStatusCode)HttpStatusCode.BadRequest));
            Assert.That(response.Content, Does.Contain("Unable to delete this story spoiler!"));
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            client?.Dispose();
        }
    }
}