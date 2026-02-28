namespace SharedLibrary.DTOs.Responses.TMDB
{
    public class ReleaseInformationDto
    {
        public string Certification { get; set; } // e.g., "PG-13", "12", "M/12"
        public List<string> Descriptors { get; set; } // usually empty
        public string Iso_639_1 { get; set; } // language code, often empty
        public string Note { get; set; } // optional note about the release
        public DateTime Release_Date { get; set; } // release date
        public int Type { get; set; } // type of release
    }
}