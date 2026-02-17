using System;

namespace Abim.Platform.Program.WebApi.Testing.Setup.Builders
{
    /// <summary>
    /// Constructs a fake email address
    /// </summary>
    public class EmailBuilder
    {
        /// <summary>
        /// Gets or sets the local portion.
        /// </summary>
        /// <value>
        /// The local.
        /// </value>
        public string Local { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        public string Extension { get; set; }

        /// <summary>
        /// Random.
        /// </summary>
        /// <value>
        /// The random.
        /// </value>
        protected Random Random { get; set; }

        /// <summary>
        /// The possible extensions
        /// </summary>
        public static readonly string[] PossibleExtensions = new string[]{"com", "org", "net", "biz", "gov", "edu"};

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailBuilder"/> class.
        /// </summary>
        public EmailBuilder()
        {
            Random = new Random();
        }

        /// <summary>
        /// Gets a random entry.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        protected string GetRandomEntry(string[] array)
        {
            int upperBound          = array.GetUpperBound(0);
            int randomEntryPosition = Random.Next(array.GetLowerBound(0), upperBound);
            string randomEntry      = array[randomEntryPosition];
            return randomEntry;
        }

        /// <summary>
        /// Fills in the local name of the email address (the X's in XXX@YYY.ZZZ) with the supplied string
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public EmailBuilder WithLocalName(string name)
        {
            Local = name;
            return this;
        }
        
        /// <summary>
        /// Picks a fake full personal name to turn into the local name portion of the email address (the X's in XXX@YYY.ZZZ)
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public EmailBuilder WithLocalName()
        {
            var name = "";
            Local = name.Replace(" ", "_").ToLower();
            return this;
        }
        
        /// <summary>
        /// Randomizes the local name of the email address (the X's in XXX@YYY.ZZZ)
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public EmailBuilder WithRandomLocalName()
        {
            var name = RandomString.Build();
            Local = name.Replace(" ", "_").ToLower();
            return this;
        }
        
        /// <summary>
        /// Fills in the domain name (but not extension) of the email address (the Y's in XXX@YYY.ZZZ) with the supplied string
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public EmailBuilder WithDomainName(string name)
        {
            Domain = name;
            return this;
        }
        
        /// <summary>
        /// Picks a fake full personal name to turn into the domain name portion (but not extension) of the email address (the Y's in XXX@YYY.ZZZ)
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public EmailBuilder WithDomainName()
        {
            var name = Faker.Name.Last();
            Local = name.Replace(" ", "_").ToLower();
            return this;
        }
        
        /// <summary>
        /// Randomizes the domain name (but not extension) of the email address (the Y's in XXX@YYY.ZZZ) with the supplied string
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public EmailBuilder WithRandomDomainName()
        {
            var name = RandomString.Build();
            Domain = name.Replace(" ", "_").ToLower();
            return this;
        }
        
        /// <summary>
        /// Fills in the domain extension of the email address (the Z's in XXX@YYY.ZZZ) with the supplied string
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public EmailBuilder WithExtension(string extension)
        {
            Extension = extension;
            return this;
        }
        
        /// <summary>
        /// Randomizes the domain extension of the email address (the Z's in XXX@YYY.ZZZ)
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public EmailBuilder WithRandomExtension()
        {
            Extension = RandomString.Build();
            return this;
        }
        
        /// <summary>
        /// Picks a random extension from the EmailBuilder.PossibleExtensions array, and uses it as the domain extension of this email address
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public EmailBuilder WithRandomValidExtension()
        {
            Extension = GetRandomEntry(PossibleExtensions);
            return this;
        }

        /// <summary>
        /// Builds a random valid email address
        /// </summary>
        public EmailBuilder ValidEmail()
        {
            return WithRandomLocalName()
                       .WithRandomDomainName()
                       .WithRandomValidExtension();
        }

        /// <summary>
        /// Builds the email address string that has been specified.
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            return string.Format("{0}@{1}.{2}", Local ?? "", Domain ?? "", Extension ?? "");
        }
    }
}
