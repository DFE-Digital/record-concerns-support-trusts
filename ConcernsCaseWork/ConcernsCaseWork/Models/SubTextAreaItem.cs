namespace ConcernsCaseWork.Models
{
	public class SubTextAreaItem
	{
		public SubTextAreaItem(int elementRootId, string name, string label, string hint, string text)
		{
			ElementRootId = elementRootId;
			Name = name;
			Label = label;
			HintText = hint;
			Text = text;
		}

		public int ElementRootId { get; set; }
		public string Name { get; set; }
		public string Label { get; set; }
		public string HintText { get; set; }
		public string Text { get; set; }

		public bool? Required { get; set; }
	}
}
