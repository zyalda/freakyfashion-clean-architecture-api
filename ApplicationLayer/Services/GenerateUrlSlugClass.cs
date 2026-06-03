using ApplicationLayer.IServices;
using System.Text.RegularExpressions;

namespace ApplicationLayer.Services
{
    public class GenerateUrlSlugClass : IGenerateUrlSlugClass
    {
        public string GenerateUrlSlug(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            string slug = name.ToLowerInvariant();

            slug = slug.Replace("å", "a").Replace("ä", "a").Replace("ö", "o");

            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            slug = Regex.Replace(slug, @"[\s-]+", " ").Trim();

            slug = slug.Replace(" ", "-");

            return slug;
        }
    }
}
