﻿using NthDimension.Procedural.Text.WordRepos;

namespace NthDimension.Procedural.Text
{

    #region WordBank
    public class WordBank
    {
        //http://stackoverflow.com/questions/38855909/alternative-to-enum-in-design-pattern

        protected readonly string Name;
        protected readonly WordType Value;
        protected readonly WordRepository Repo;

        /// <summary>
        /// A metric butt-ton of nouns, ranging from common to...not so common.
        /// </summary>
        public static readonly WordBank Nouns = new WordBank(WordType.Noun, "Nouns", new NounsRepository());

        /// <summary>
        /// A sturdy list of adjectives. Common and uncommon.
        /// </summary>
        public static readonly WordBank Adjectives = new WordBank(WordType.Adjective, "Adjectives", new AdjectivesRepository());

        /// <summary>
        // A list of first names. All genders.
        /// </summary>
        public static readonly WordBank FirstNames = new WordBank(WordType.FirstName, "FirstNames", new FirstNamesRepository());

        /// <summary>
        /// A list of last names.
        /// </summary>
        public static readonly WordBank LastNames = new WordBank(WordType.LastName, "LastNames", new LastNamesRepository());

        /// <summary>
        /// An arbitrary list of titles. All genders:
        /// "President", "Mrs", "Doctor", etc...
        /// </summary>
        public static readonly WordBank Titles = new WordBank(WordType.Title, "Titles", new TitleRepository());

        /// <summary>
        /// An arbitrary list of male titles:
        /// "Master", "Sir", "Lord", etc...
        /// </summary>
        public static readonly WordBank MaleTitles = new WordBank(WordType.MaleTitle, "MaleTitles", new MaleTitleRepository());

        /// <summary>
        /// An arbitrary list of female titles:
        /// "Madam", "Mrs", "Grandma", etc...
        /// </summary>
        public static readonly WordBank FemaleTitles = new WordBank(WordType.FemaleTitle, "FemaleTitles", new FemaleTitleRepository());

        /// <summary>
        /// The days of the week.
        /// </summary>
        public static readonly WordBank Days = new WordBank(WordType.Day, "Days", new DaysRepository());

        /// <summary>
        /// A list of female first names. Western origin.
        /// </summary>
        public static readonly WordBank FemaleFirstNames = new WordBank(WordType.FemaleFirstName, "FemaleFirstNames", new FemaleFirstNameRepository());

        /// <summary>
        /// A list of male first names. Western origin.
        /// </summary>
        public static readonly WordBank MaleFirstNames = new WordBank(WordType.MaleFirstName, "MaleFirstNames", new MaleFirstNameRepository());

        /// <summary>
        /// The months of the year.
        /// </summary>
        public static readonly WordBank Months = new WordBank(WordType.Month, "Months", new MonthRepository());

        /// <summary>
        /// The state names for all 50 U.S. states.
        /// </summary>
        public static readonly WordBank StateNames = new WordBank(WordType.StateName, "StateNames", new StateNamesRepository());

        /// <summary>
        /// A list of occupations. Some of the job titles contain multiple words.
        /// </summary>
        public static readonly WordBank JobTitles = new WordBank(WordType.JobTitle, "JobTitles", new JobTitlesRepository());

        /// <summary>
        /// A list of countries on Earth. Likely outdated.
        /// </summary>
        public static readonly WordBank Countries = new WordBank(WordType.Country, "Countries", new CountriesRepository());

        /// <summary>
        /// An EXPANSIVE list of cities on Earth. Contains ~50K cities.
        /// </summary>
        public static readonly WordBank Cities = new WordBank(WordType.City, "Cities", new CitiesRepository());

        /// <summary>
        /// A giant list of adverbs.
        /// </summary>
        public static readonly WordBank Adverbs = new WordBank(WordType.Adverb, "Adverbs", new AdverbsRepository());

        /// <summary>
        /// A humongous list of verbs. This is my favorite!
        /// </summary>
        public static readonly WordBank Verbs = new WordBank(WordType.Verb, "Verbs", new VerbsRepository());


        public WordBank(WordType value, string name, WordRepository repo)
        {
            Name = name;
            Value = value;
            Repo = repo;
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator WordType(WordBank @enum)
        {
            return @enum.Value;
        }

        public static implicit operator string(WordBank @enum)
        {
            return @enum.Name;
        }

        public string[] Get()
        {
            return Repo.Get();
        }
    }
    #endregion WordBank
}
