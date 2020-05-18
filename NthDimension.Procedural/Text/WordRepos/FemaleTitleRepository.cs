namespace NthDimension.Procedural.Text.WordRepos
{
    public class FemaleTitleRepository : WordRepository
    {
        public FemaleTitleRepository() : base(new string[] {
            "Master",
            "Miss",
            "Mrs",
            "Lady",
            "Madam",
            "Dr",
            "Elder",
            "Grandma",
            "President",
            "Queen",
            "Princess",
            "Aunt"}) { }
    }
}
