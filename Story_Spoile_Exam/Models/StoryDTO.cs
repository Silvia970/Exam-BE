using System.Text.Json.Serialization;

namespace Story_Spoiler_Exam.Models
{
    internal class StoryDTO
    {

        [JsonPropertyName("title")]

        public string? Title { get; set; }

        [JsonPropertyName("description")]

        public string? Description { get; set; }

        [JsonPropertyName("url")]

        public string? Url { get; set; }
    }

}
