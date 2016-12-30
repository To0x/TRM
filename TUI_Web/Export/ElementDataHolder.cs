using System;
namespace TUI_Web
{
	public static class ElementDataHolder
	{
		private static string imagePath = "/img.jpg";
		private static string textContent = "Lorem Ipsum";
		private static string topicContent = "Topic";

		/*
		private static string getHtmlData(ElementTypes type)
		{

			string data = null;
			switch (type)
			{
				case ElementTypes.None:
					data = "<div></div>";
					break;

				case ElementTypes.Graphic:
					data = "<img id=%d src=%s />";
					break;

				case ElementTypes.Text:
					data = "<div>%s</div>";
					break;

				case ElementTypes.Topic:
					data = "<h1>%s</h1>";
					break;

				default:
					data = String.Empty;
					break;
			}

			return data;
		}
		*/

		public static string getImageSourcePath()
		{
			return imagePath;
		}

		public static string getTopicContent()
		{
			return topicContent;
		}

		public static string getTextContent()
		{
			return textContent;
		}

		/*
		public static string getHtmlContent(ElementTypes type)
		{
			string data = null;
			switch (type)
			{
				case ElementTypes.Graphic:
					data =  imagePath;
					break;

				case ElementTypes.Text:
					data = textContent;
					break;

				case ElementTypes.Topic:
					data = topicContent;
					break;

				case ElementTypes.None:
					
				default:
					data = String.Empty;
					break;
			}

			return data;
		}
		*/
	}
}
