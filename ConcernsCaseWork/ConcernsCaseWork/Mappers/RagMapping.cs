using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Mappers
{
	public static class RagMapping
	{
		private static readonly Dictionary<string, Tuple<int, IList<string>>> Rags = new Dictionary<string, Tuple<int, IList<string>>>(7)
		{
			{"n/a", new Tuple<int, IList<string>>(0, new List<string> { "-" })}, 
			{"Red-Plus", new Tuple<int, IList<string>>(1, new List<string> { "Red Plus" })}, 
			{"Red", new Tuple<int, IList<string>>(2, new List<string> { "Red" })}, 
			{"Red-Amber", new Tuple<int, IList<string>>(3, new List<string> { "Red", "Amber" })},
			{"Amber-Green", new Tuple<int, IList<string>>(4, new List<string> { "Amber", "Green" })},
			{"Amber",  new Tuple<int, IList<string>>(5, new List<string> { "Amber" })},
			{"Green",  new Tuple<int, IList<string>>(6, new List<string> { "Green" })}
		};
		private static readonly Dictionary<string, IList<string>> RagsCss = new Dictionary<string, IList<string>>(7)
		{
			{"n/a", new List<string> { "" }}, 
			{"Red-Plus", new List<string> { "ragtag__redplus" }}, 
			{"Red", new List<string> { "ragtag__red" }}, 
			{"Red-Amber", new List<string> { "ragtag__red", "ragtag__amber" }}, 
			{"Amber-Green", new List<string> { "ragtag__amber", "ragtag__green" }},
			{"Amber", new List<string> { "ragtag__amber" }},
			{"Green", new List<string> { "ragtag__green" }}
		};
		
		public static Tuple<int, IList<string>> FetchRag(string rating)
		{
			var defaultRating = new Tuple<int, IList<string>>(0, new List<string> { "n/a" });
			return Rags.TryGetValue(rating ?? "n/a", out var rag) ? rag : defaultRating;
		}
		
		public static IList<string> FetchRagCss(string rating)
		{
			var defaultRating = new List<string> { "n/a" };
			return RagsCss.TryGetValue(rating ?? "n/a", out var ragCss) ? ragCss : defaultRating;
		}
	}
}