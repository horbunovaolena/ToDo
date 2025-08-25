namespace ToDo.Api
{
    public static class TagExtensions
    {
        public static string NormalizeTag(this string tag)
        {
            return tag.Trim().ToLowerInvariant();
        }

        public static void AddTag(this List<string> tags, string tag)
        {
            var normalizedTag = tag.NormalizeTag();
            if (!string.IsNullOrEmpty(normalizedTag) && !tags.Contains(normalizedTag))
            {
                tags.Add(normalizedTag);
            }
        }

        public static void RemoveTag(this List<string> tags, string tag)
        {
            var normalizedTag = tag.NormalizeTag();
            tags.Remove(normalizedTag);
        }

        public static bool HasTag(this List<string> tags, string tag)
        {
            var normalizedTag = tag.NormalizeTag();
            return tags.Contains(normalizedTag);
        }

        public static List<string> GetUniqueTagsFrom(this IEnumerable<Todo> todos)
        {
            return todos.SelectMany(t => t.Tags)
                       .Distinct()
                       .OrderBy(tag => tag)
                       .ToList();
        }
    }
}